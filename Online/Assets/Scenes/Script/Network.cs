using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SocketIO;

public class Network : MonoBehaviour
{
    static SocketIOComponent socket;
    public UIControll uiControll;
    public GameControll gamecontroll;

    JSONObject JSONobject;

    public bool isLogin;

    public string message;

    public string message2;
    

    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        uiControll = GameObject.Find("UIControll").GetComponent<UIControll>();

        socket.On("Connection To Server", OnConnected);
        

        socket.On("login success", OnLogin);
        socket.On("other player login", OnOtherLogin);

        socket.On("logout success", OnLogout);
        socket.On("other player logout", OnOtherLogout);

        socket.On("checkReady", checkReady);
        socket.On("CheckAnss", CheckAns);

        socket.On("GameEND", GameEND);
        
        socket.On("ShowWinner", ShowWinner);
        
    }

    private void OnConnected(SocketIOEvent obj) // Connection To Server
    {
        uiControll.serverStatus.text = "Server Online";
    }

    //////////////////////////////////////////////////////

    public void LoginToServer(string name) 
    {
        if(isLogin == false)
        {
            isLogin = true;

            JSONobject = new JSONObject(JSONObject.Type.OBJECT);
            JSONobject.AddField("playerName",name);

            socket.Emit("login",JSONobject);
        }
    }
    private void OnLogin(SocketIOEvent obj) // login success
    {
        uiControll.inputBox.SetActive(false);
        uiControll.buttonSingIn.SetActive(false);
        uiControll.buttonSingOut.SetActive(true);
        uiControll.gameobjectReadyButton.SetActive(true);

        JSONobject = obj.data;

        uiControll.playerID.text = JSONobject["id"].str;
        uiControll.playerName.text = JSONobject["name"].str;

        //Debug.Log(JSONobject["numberplayer"].n);

        uiControll.textNumberPlayerInGame.text = "Player In Game " + JSONobject["numberplayer"].n;
        uiControll.textNumberPlayerReady.text = "Player Have Ready " + JSONobject["numberplayerReady"].n;
    }
    public void LogoutOutServer(bool data) 
    {
        if(isLogin == true)
        {
            isLogin = false;

            JSONobject = new JSONObject(JSONObject.Type.OBJECT);
            JSONobject.AddField("checkReady",data);
            Debug.Log(data);
            socket.Emit("logout",JSONobject);

            uiControll.inputBox.SetActive(true);
            uiControll.buttonSingIn.SetActive(true);
            uiControll.buttonSingOut.SetActive(false);
        }
    }
    private void OnLogout(SocketIOEvent obj) // logout success
    { 
        uiControll.inputBox.SetActive(true);
        uiControll.buttonSingIn.SetActive(true);
        uiControll.buttonSingOut.SetActive(false);
        uiControll.gameobjectReadyButton.SetActive(false);

        gamecontroll.buttonHigher.SetActive(false);
        gamecontroll.buttonLower.SetActive(false);

        gamecontroll.goHowToPlay.SetActive(false);

        gamecontroll.gotextMessage1.SetActive(false);
        gamecontroll.gotextMessage2.SetActive(false);

        gamecontroll.goShowNum.SetActive(false);

        uiControll.playerID.text = "";
        uiControll.playerName.text = "";

        uiControll.textNumberPlayerInGame.text = "";
        
    }
    void OnApplicationQuit()
    {
        if(isLogin == true)
        {
            socket.Emit("logout");
        }
    }

    private void OnOtherLogin(SocketIOEvent obj) // other player login
    {
        JSONobject = obj.data;
        //string id = JSONobject["numberplayer"].str;
        uiControll.textNumberPlayerInGame.text = "Player In Game " + JSONobject["numberplayer"].n;
    }
    private void OnOtherLogout(SocketIOEvent obj) // other player logout
    {
        JSONobject = obj.data;
        uiControll.textNumberPlayerInGame.text = "Player In Game " + JSONobject["numberplayer"].n;
    }
    
    public void OnReadyToPlay(bool isReady) 
    {
        JSONobject = new JSONObject(JSONObject.Type.OBJECT);
        JSONobject.AddField("checkReady",isReady);

        socket.Emit("CheckReady",JSONobject);
    }
    private void checkReady(SocketIOEvent obj) // 
    {
        JSONobject = obj.data;
        uiControll.textNumberPlayerReady.text = JSONobject["status"].str;

        if(uiControll.textNumberPlayerReady.text == "All Player Ready")
        {
            uiControll.gameStart = true;
            gamecontroll.textMessage2.text = "";
            gamecontroll.textShowNum.text = "";
            gamecontroll.GameSetUp();
        }
    }

    

    public void SendAns(bool ans)
    {
        gamecontroll.playerSendAns = true;

        JSONobject = new JSONObject(JSONObject.Type.OBJECT);
        JSONobject.AddField("Ans",ans);
        JSONobject.AddField("id",uiControll.playerID.text);
        //Debug.Log(ans);
        socket.Emit("CheckAns",JSONobject);
    }

    private void CheckAns(SocketIOEvent obj) 
    {
        JSONobject = obj.data;
        message = JSONobject["Message"].str;

        Debug.Log(JSONobject["id"].str);
        Debug.Log("AAA");

        if(JSONobject["id"].str == uiControll.playerID.text)
        {
            Debug.Log("AAA");
            if(JSONobject["Con"].b == true)
            {
                gamecontroll.isLose = false;
                message2 = "You Guess True";
            }
            else if(JSONobject["Con"].b == false)
            {
                gamecontroll.isLose = true;
                message2 = "You Guess False";
            }
        }
        
        if(message == "No One Win")
        {
           StartCoroutine(NoOneWinDelay());
        }
        else if(message == "Have A Winner")
        {
            
            StartCoroutine(HaveAWinner());
            
        }
        else if(message == "All Player Answer" && gamecontroll.isLoseCheck == false)
        {
            gamecontroll.textMessage2.text = message;
            
            StartCoroutine(waitAns());
        }
        else if(gamecontroll.isLoseCheck == false)
        {
            gamecontroll.textMessage2.text = message;
            Debug.Log(message);
        }
    }

    public IEnumerator waitAns()
    {
        yield return new WaitForSeconds(2.0f);
        gamecontroll.caculateConclude(JSONobject,message2);
    }

    public IEnumerator NoOneWinDelay()
    {
        yield return new WaitForSeconds(2.0f);
        gamecontroll.textMessage1.text = "Wait For Conclude";
        gamecontroll.textMessage2.text = "";
        yield return new WaitForSeconds(2.0f);
        gamecontroll.textMessage1.text = "All Player Lose";
        gamecontroll.textMessage2.text = "You Guess False";
        gamecontroll.textShowNum.text = "Dice Is " + JSONobject["Roll"].n;
        yield return new WaitForSeconds(2.0f);
        socket.Emit("Remath");
    }

    public IEnumerator HaveAWinner()
    {
        yield return new WaitForSeconds(2.0f);
        gamecontroll.textMessage1.text = "Wait For Conclude";
        gamecontroll.textMessage2.text = "";
        yield return new WaitForSeconds(2.0f);
        gamecontroll.textMessage1.text = JSONobject["Message"].str;
        gamecontroll.textMessage2.text = JSONobject["Con"].str;
        gamecontroll.textShowNum.text = "Dice Is " + JSONobject["Roll"].n;
        yield return new WaitForSeconds(2.0f);
        checkWinner();
    }

    private void GameEND(SocketIOEvent obj) 
    {
        uiControll.gameobjectReadyButton.SetActive(true);

        gamecontroll.buttonHigher.SetActive(false);
        gamecontroll.buttonLower.SetActive(false);

        //gamecontroll.goHowToPlay.SetActive(false);
        uiControll.textNumberPlayerReady.text = "Player Have Ready " + "0";

        //gamecontroll.gotextMessage1.SetActive(false);
        //gamecontroll.gotextMessage2.SetActive(false);

        //gamecontroll.goShowNum.SetActive(false);

        uiControll.gameobjectReadyButton.SetActive(true);

        gamecontroll.isLose = false;
        gamecontroll.isLoseCheck = false;

        gamecontroll.playerSendAns = false;

        uiControll.gameStart = false;

        uiControll.isReady = false;
        uiControll.isReadyText.text = "Not Ready";
    }

    public void checkWinner()
    {
        if(message2 == "You Guess True")
        {
            JSONobject = new JSONObject(JSONObject.Type.OBJECT);
            JSONobject.AddField("name",uiControll.playerName.text);
            JSONobject.AddField("id",uiControll.playerID.text);
            

            socket.Emit("ShowWinner",JSONobject);
        }
            
    }

    private void ShowWinner(SocketIOEvent obj) 
    {
        JSONobject = obj.data;


        gamecontroll.textMessage1.text = "Winner Is " + JSONobject["name"].str + " ID " + JSONobject["id"].str;
        //gamecontroll.textMessage2.text = "You Guess False";
        //gamecontroll.textShowNum.text = "Dice Is " + JSONobject["Roll"].n;

        uiControll.gameobjectReadyButton.SetActive(true);

        gamecontroll.buttonHigher.SetActive(false);
        gamecontroll.buttonLower.SetActive(false);

        //gamecontroll.goHowToPlay.SetActive(false);
        uiControll.textNumberPlayerReady.text = "Player Have Ready " + "0";

        //gamecontroll.gotextMessage1.SetActive(false);
        //gamecontroll.gotextMessage2.SetActive(false);

        //gamecontroll.goShowNum.SetActive(false);

        uiControll.gameobjectReadyButton.SetActive(true);

        gamecontroll.isLose = false;
        gamecontroll.isLoseCheck = false;

        gamecontroll.playerSendAns = false;

        uiControll.gameStart = false;

        uiControll.isReady = false;
        uiControll.isReadyText.text = "Not Ready";
            
    }


    public void NextRound()
    {
        gamecontroll.playerSendAns = false;
        socket.Emit("Next Round");
        
    }
    
    

}