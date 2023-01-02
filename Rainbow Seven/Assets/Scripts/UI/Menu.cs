using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string MenuName;
    public bool IsOpen;

    public void Open()
    {
        gameObject.SetActive(true);

        Debug.Log($"Open: {MenuName}");

        IsOpen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);

        Debug.Log($"Close: {MenuName}");

        IsOpen = false;
    }
}
