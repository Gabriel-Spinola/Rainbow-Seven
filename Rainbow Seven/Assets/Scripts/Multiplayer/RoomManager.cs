using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance) {
            Destroy(gameObject);

            return;
        }

        DontDestroyOnLoad(gameObject);  
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) { // Checking if we're in the game scene
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FPRE_PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
