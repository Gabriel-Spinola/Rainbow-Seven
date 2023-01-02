using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitleScreen _titleScreen;

    void Start()
    {
        Debug.Log("Network: Connecting to Master");
        MenuManager.Instance.OpenMenu("Title");

        PhotonNetwork.ConnectUsingSettings();  
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _titleScreen.CanStart = true;
        Debug.Log("Network: Joined Lobby");
    }
}
