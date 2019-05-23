using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIControll : MonoBehaviour
{
    public Network network;
    public Text serverStatus;

    public Text playerID;
    public Text playerName;

    public Text nameSingIn;
    public GameObject inputBox;
    public GameObject buttonSingIn;

    public GameObject buttonSingOut;

    public Text textNumberPlayerInGame;
    public Text textNumberPlayerReady;

    public bool isReady;
    public Text isReadyText;
    public GameObject gameobjectReadyButton;

    public bool gameStart;

    void Start()
    {
        network = GameObject.Find("Network").GetComponent<Network>();
        serverStatus = GameObject.Find("ServerStatus").GetComponent<Text>();
    }
    
    public void singInButton()
    {
        network.LoginToServer(nameSingIn.text);
    }

    public void singOutButton()
    {
        network.LogoutOutServer(isReady);
    }

    public void readyButton()
    {
        if(isReady && !gameStart)
        {
            
            isReady = false;
            network.OnReadyToPlay(isReady);
            isReadyText.text = "Not Ready";
        }
        else if(!isReady && !gameStart)
        {
            
            isReady = true;
            network.OnReadyToPlay(isReady);
            isReadyText.text = "Ready";
        }
        
    }
}