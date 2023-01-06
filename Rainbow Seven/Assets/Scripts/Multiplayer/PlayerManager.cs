using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    [Header("Debug")]
    public bool IsDebugging;
    public bool ShouldRespawn;

    private PhotonView _photonView;
    private GameObject _controller;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_photonView.IsMine) {
            CreateController();
        }
    }

    private void CreateController()
    {
        _controller = PhotonNetwork.Instantiate(
            prefabName: Path.Combine("PhotonPrefabs", "FPRE_PlayerController"),
            position: new Vector3(17.63f, 15f, .45f),
            rotation: Quaternion.identity,
            group: 0,
            data: new object[] {
                _photonView.ViewID
            }
        );
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_controller);

        if (IsDebugging) {
            CreateController();
        }
    }
}
