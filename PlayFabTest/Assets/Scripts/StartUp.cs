using IAHNetCoreServer.Logic.Client.ResponseHandler;
using IAHNetCoreServer.Share.NetworkV2;
using IAHNetCoreServer.Share.TransportData;
using MessagePack;
using MessagePack.Resolvers;
using PlayFab.ClientModels;
using PlayFabCustom;
using UnityEditor;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    private LoginResult _playFabLoginResult;

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
        _playFabLoginResult = arg1;
        APINetworkHandler.Instance.StartConnect(_playFabLoginResult.PlayFabId, _playFabLoginResult.SessionTicket);
    }

    // Update is called once per frame
    void Update()
    {
        APINetworkHandler.Instance?.Update();
    }

    static bool serializerRegistered = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (!serializerRegistered)
        {
            StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }
    }

#if UNITY_EDITOR


    [InitializeOnLoadMethod]
    static void EditorInitialize()
    {
        Initialize();
    }

#endif

    public void SendTestRequest()
    {
        Debug.Log("Try to send TestRequest");
        var testRequest = new TestRequest("TestRequest from client");
        APINetworkHandler.Instance.Player.SendRequest<TestResponse>(testRequest, (response, player) =>
        {
            Debug.Log($"TestResponse from server: {response.msg}");
        }, null, null);
    }
}