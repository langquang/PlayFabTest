using System.Collections;
using System.Collections.Generic;
using IAHNetCoreServer.Client;
using PlayFab.ClientModels;
using PlayFabCustom;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    private NetClient _netClient;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Try to connect to PlayFab...");
        PlayFabAuthService.Instance.OnLoginSuccess += OnLoginSuccess;
        PlayFabAuthService.Instance.Start();
    }

    private void OnLoginSuccess(LoginResult arg1, CreateParams arg2)
    {
        Debug.Log("Try to connect to Game Server...");
        _netClient = new NetClient();
        _netClient.Start("127.0.0.1", 8000, "ButinABC");
    }

    // Update is called once per frame
    void Update()
    {
        if( _netClient != null )
            _netClient.Update();
    }
}
