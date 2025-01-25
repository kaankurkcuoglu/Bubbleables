using System;
using Unity.Entities;
using UnityEngine;

namespace AAA.Lobby
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial class LobbySystem : SystemBase
    {
        protected override void OnUpdate()
        {
           Debug.Log("LobbySystem.OnUpdate");
        }
    }
}