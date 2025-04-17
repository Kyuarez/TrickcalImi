using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyBackground : MonoBehaviour
{
    private Sprite[] backgroundLobbyMains;
    private Image backgroundImage;

    private void Awake()
    {
        //casting
        backgroundImage = GetComponentInChildren<Image>();

        //backgroundImage Load
        Sprite[] arr = Resources.LoadAll<Sprite>(Define.Res_UI_LobbyBackground);

        if(arr == null && arr.Length == 0)
        {
            Debug.AssertFormat(false, $"Lobby Background Image couldn't loaded! by Resources");
            return;
        }

        Array.Copy(backgroundLobbyMains, arr, arr.Length);
        SetBackgroundLobbyMain();
    }

    public void SetBackgroundLobbyMain()
    {
        int rand = UnityEngine.Random.Range(0, backgroundLobbyMains.Length);
        backgroundImage.sprite = backgroundLobbyMains[rand];
    }

}
