using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    public RoomInfo RoomInfo;

    public void SetUp(RoomInfo roomInfo) 
    {
        RoomInfo = roomInfo;

        _text.text = RoomInfo.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(RoomInfo);
    }
}
