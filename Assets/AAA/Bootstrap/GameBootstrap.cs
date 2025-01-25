using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

[Preserve]
public class GameBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        var lobbyMode = SceneManager.GetActiveScene().name == "Lobby";

        if (lobbyMode)
        {
            AutoConnectPort = 0;
            CreateLocalWorld(defaultWorldName);
        }
        else
        {
            AutoConnectPort = 7979;
            CreateDefaultClientServerWorlds();
        }

        return true;
    }
}
