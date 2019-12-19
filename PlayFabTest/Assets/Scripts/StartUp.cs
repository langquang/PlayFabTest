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
    private NetClient _netClient;

    private NetPlayer _player;

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
        _netClient = new NetClient();
        _netClient.Listener.PeerConnectedEvent += peer =>
        {
            Debug.Log("Try to Login to server");
            var loginRequest = new LoginRequest(_playFabLoginResult.PlayFabId, _playFabLoginResult.SessionTicket);
            _player = new NetPlayer(peer);
            _player.Request(loginRequest);
        };
        _netClient.Start("127.0.0.1", 8000, "ButinABC");
    }

    // Update is called once per frame
    void Update()
    {
        _netClient?.Update();
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
        _player.Request(testRequest);
    }
}