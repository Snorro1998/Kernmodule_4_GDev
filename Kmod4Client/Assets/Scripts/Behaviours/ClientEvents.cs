using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Networking.Transport;

public class ClientEvents : MonoBehaviour
{
    public static ClientEvents Instance;

    /*
    public Dictionary<string, System.Action> actions = new Dictionary<string, Action>();


    public void StartEvent(string name)
    {
        Action a = actions[name];
        a?.Invoke();
    }*/
    private void Awake()
    {
        Instance = this;
        
        //actions.Add("lobbyStart", onLobbyStartEnter);
    }
    /*
    private void Start()
    {
        onLobbyStartEnter += TestClientBehaviour.Instance.startLobby;
    }*/

    public void StartEvent(Action a)
    {
        a?.Invoke();
    }

    //public event Action onLobbyStartEnter;
    public Action onLobbyStartEnter;
    public event Action onImageSendEnter;
    
    public void LobbyStartEnter()
    {
        onLobbyStartEnter?.Invoke();
    }

    public void ImageSendEnter()
    {
        onImageSendEnter?.Invoke();
    }
}
