using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitleScreen _titleScreen;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Text _errorText;

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

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text)) {
            return;
        }

        PhotonNetwork.CreateRoom(_roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = $"Room creation failed: {message}";
        MenuManager.Instance.OpenMenu("Error");
    }
}
