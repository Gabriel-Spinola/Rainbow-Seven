using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public bool CanStart;

    void Update()
    {
        if (Input.anyKey && CanStart) {
            MenuManager.Instance.OpenMenu("Main");

            Debug.Log("Should start");
            CanStart = false;
        }
    }
}
