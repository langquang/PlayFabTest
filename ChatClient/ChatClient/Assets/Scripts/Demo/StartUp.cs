using MessagePack;
using MessagePack.Resolvers;
using PlayFab.ClientModels;
using PlayFabCustom;
using Share.APIServer.Request;
using UnityClientLib.Logic.Client.ResponseHandler;
using UnityEditor;
using UnityEngine;

namespace Demo
{
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

        private void OnLoginSuccess(LoginResult loginResult, CreateParams createParams)
        {
            Debug.Log("Try to connect to Game Server...");
            _playFabLoginResult = loginResult;
            APINetworkHandler.Instance.StartConnect(_playFabLoginResult.PlayFabId, _playFabLoginResult.SessionTicket, createParams);
        }

        // Update is called once per frame
        void Update()
        {
            APINetworkHandler.Instance?.Update();
        }

        private void OnDestroy()
        {
            APINetworkHandler.Instance?.Stop();
        }

        static bool serializerRegistered = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (!serializerRegistered)
            {
                StaticCompositeResolver.Instance.Register(
                    GeneratedResolver.Instance,
                    StandardResolver.Instance
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
            APINetworkHandler.Instance.SendRequest<TestResponse>(testRequest, (response, player) =>
            {
                Debug.Log($"TestResponse from server: {response.msg}");
            }, null);
        }

        public void SwitchToServer(int server)
        {
            Debug.Log($"SwitchToServer {server}");
            if (PlayFabAuthService.Instance.IsContainsAccount(server))
            {
                Debug.Log($"Already have a account at {server}");
                PlayFabAuthService.Instance.SwitchServer(server);
            }
            else
            {
                Debug.Log($"Try to ask server to create a node at {server}");
                var request = new CheckCreateNodeAccountRequest(server, PlayFabAuthService.Instance.getMasterId());
                APINetworkHandler.Instance.SendRequest<TestResponse>(request,
                    (response, player) =>
                    {
                        Debug.Log("Ask response from server: OK");
                    },
                    i =>
                    {
                        Debug.Log($"Ask response from server: Error ={i}");
                    });
            }
        }
    }
}