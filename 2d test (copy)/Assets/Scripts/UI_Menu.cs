using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.SceneManagement;

public class UI_Menu : MonoBehaviour
{
    public void Leave()
    {
        SceneManager.LoadScene(0);
        NetworkManager.Singleton.StopClient();
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Connect()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        if (NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.StopHost();
        else if (NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.StopClient();
    }

    public void StartGame()
    {
        NetworkSceneManager.SwitchScene("Arena");
    }
}
