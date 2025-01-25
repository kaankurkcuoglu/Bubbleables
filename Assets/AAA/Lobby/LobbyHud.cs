using System;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

namespace AAA.Lobby
{
    public class LobbyHud : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private TMP_InputField _ipAddressInput;
        
        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
        }

        private void OnJoinClicked()
        {
            if (CheckIfAlreadyHostedOrConnected())
            {
                return;
            }
            
            var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            
            if (World.DefaultGameObjectInjectionWorld == null)
                World.DefaultGameObjectInjectionWorld = client;
            
            var ip = _ipAddressInput.text;
            var ep = NetworkEndpoint.Parse(ip, 40900);
            {
                using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
            }
        }

        private void OnHostClicked()
        {
            Debug.Log("Host clicked");

            if (CheckIfAlreadyHostedOrConnected())
            {
                return;
            }
            
            var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");
            
            
            if (World.DefaultGameObjectInjectionWorld == null)
                World.DefaultGameObjectInjectionWorld = server;

            ushort port = 40900;

            NetworkEndpoint ep = NetworkEndpoint.AnyIpv4.WithPort(port);
            {
                using var drvQuery = server.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());

                var driver = drvQuery.GetSingletonRW<NetworkStreamDriver>();
                driver.ValueRW.RequireConnectionApproval = false;
                var success = driver.ValueRW.Listen(ep);
                
                if (!success)
                {
                    Debug.LogError("Failed to listen on port " + port);
                    return;
                }
            }

            ep = NetworkEndpoint.LoopbackIpv4.WithPort(port);
            {
                using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
            }
            
            Debug.Log("Hosted");
        }

        private bool CheckIfAlreadyHostedOrConnected()
        {
            if (ClientServerBootstrap.ClientWorld != null && ClientServerBootstrap.ServerWorld != null)
            {
                Debug.Log("Already hosted");
                return true;
            }
            else if (ClientServerBootstrap.ClientWorld != null)
            {
                Debug.Log("Already connected");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}