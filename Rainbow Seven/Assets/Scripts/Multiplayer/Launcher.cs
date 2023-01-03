using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using CustomExtensions;

// TODO: Maybe is still possible to duplicate rooms
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance { get; private set; }

    [SerializeField] private TitleScreen _titleScreen;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private Transform _playerListContent;
    [SerializeField] private GameObject _roomListItemPrefab;
    [SerializeField] private GameObject _playerListItemPrefab;
    [SerializeField] private GameObject _startGameButton;

    private static Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Network: Connecting to Master");
        MenuManager.Instance.OpenMenu("Title");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Network: Joined Lobby");

        _titleScreen.CanStart = true;
        PhotonNetwork.NickName = $"Player {Random.Range(0, 2000)}";
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text)) {
            return;
        }

        PhotonNetwork.CreateRoom(_roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        _playerListContent.DestroyChildren();

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++) {
            Instantiate(_playerListItemPrefab, _playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = $"Room creation failed: {message}";
        MenuManager.Instance.OpenMenu("Error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
        _cachedRoomList.Clear();
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Main");
        _cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomListContent.DestroyChildren();

        for (int i = 0; i < roomList.Count; i++) {
            RoomInfo info = roomList[i];

            if (info.RemovedFromList) {
                _cachedRoomList.Remove(info.Name);
            }
            else {
                _cachedRoomList[info.Name] = info;
            }

            foreach (KeyValuePair<string, RoomInfo> entry in _cachedRoomList) {
                Instantiate(_roomListItemPrefab, _roomListContent).GetComponent<RoomListItem>().SetUp(_cachedRoomList[entry.Key]);
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(_playerListItemPrefab, _playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
