using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    private RoomInfo _roomInfo;

    public void SetUp(RoomInfo roomInfo) 
    {
        _roomInfo = roomInfo;

        _text.text = _roomInfo.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(_roomInfo);
    }
}
