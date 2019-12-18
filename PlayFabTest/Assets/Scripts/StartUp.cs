using System.Collections;
using System.Collections.Generic;
using IAHNetCoreServer.Client;
using MessagePack;
using MessagePack.Resolvers;
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
    
    static bool serializerRegistered = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (!serializerRegistered)
        {
            StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }
    }

#if UNITY_EDITOR


    [UnityEditor.InitializeOnLoadMethod]
    static void EditorInitialize()
    {
        Initialize();
    }

#endif
}
