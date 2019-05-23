using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameControll : MonoBehaviour
{
    public Network network;
    public UIControll uiControll;

    public GameObject buttonHigher;
    public GameObject buttonLower;

    public GameObject goHowToPlay;

    public Text textMessage1;
    public GameObject gotextMessage1;

    public Text textMessage2;
    public GameObject gotextMessage2;

    public Text textShowNum;
    public GameObject goShowNum;

    public bool playerSendAns = true;
    public string ansplayer;

    public bool conclude = false;

    public bool isLose = false;

    public bool isLoseCheck = false;
    

    void Start()
    {
        network = GameObject.Find("Network").GetComponent<Network>();

        buttonHigher.SetActive(false);
        buttonLower.SetActive(false);

        //goHowToPlay.SetActive(false);

        gotextMessage1.SetActive(false);
        gotextMessage2.SetActive(false);

        uiControll.gameobjectReadyButton.SetActive(false);

        goShowNum.SetActive(false);
    }

    public void GameSetUp()
    {
        buttonHigher.SetActive(true);
        buttonLower.SetActive(true);

        goHowToPlay.SetActive(true);

        gotextMessage1.SetActive(true);
        gotextMessage2.SetActive(true);
        
        uiControll.gameobjectReadyButton.SetActive(false);

        goShowNum.SetActive(true);

        StartCoroutine(contDownToStart());
    }

    public IEnumerator contDownToStart()
    {
        textMessage1.text = "Wait for roll dice";
        yield return new WaitForSeconds(2.0f);
        textMessage1.text = "Dice : Higher or Lower";
        textMessage2.text = "No Player Answer";

        

        playerSendAns = false;
    }

    public void seleceHigher()
    {
        
        if(!playerSendAns && isLose == false)
        {
            textMessage1.text = "You Are Selece Higher";
            ansplayer = "Higher";
            network.SendAns(true);
        }
    }

    public void seleceLower()
    {
        if(!playerSendAns && isLose == false)
        {
            textMessage1.text = "You Are Selece Lower";
            ansplayer = "Lower";
            network.SendAns(false);
        }
    }

    public void caculateConclude(JSONObject JSONobject,string con)
    {
        StartCoroutine(contDownConclude(JSONobject,con));
    }

    public IEnumerator contDownConclude(JSONObject JSONobject,string con)
    {
        textMessage1.text = "Wait For Conclude";
        textMessage2.text = "";

        yield return new WaitForSeconds(2.0f);
        textShowNum.text = "Dice Is " + JSONobject["Roll"].n;
        
        textMessage1.text = con;
        
        textMessage2.text = "Left Player " + JSONobject["whatLeft"].n;

        yield return new WaitForSeconds(2.0f);

        if(isLose == true)
        {
            textShowNum.text = "Wait For The Winner";
            textMessage2.text = "You Ard Lose";

            buttonHigher.SetActive(false);
            buttonLower.SetActive(false);

            if(!isLoseCheck)
            {
                isLoseCheck = true;
            }
        }
        else // when win
        {
            textMessage1.text = "Wait for roll dice";
            yield return new WaitForSeconds(2.0f);
            textMessage1.text = "Dice : Higher or Lower";
            textMessage2.text = "No Player Answer";
            textShowNum.text = "";
            
            network.NextRound();
            yield return new WaitForSeconds(1.0f);
            playerSendAns = false;
        }
    }
}