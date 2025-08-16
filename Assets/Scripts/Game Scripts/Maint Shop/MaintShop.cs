using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DreamersIncStudio.Moonshoot
{
    public struct MaintShop : IComponentData
    {
        public float Range;

    }
    public struct MaintShopSetup : IComponentData
    {
        public readonly uint NumberOfWorkers;
        public float3 SpawnPoint;
        public MaintShopSetup(uint authoringWorkerCount, float3 spawnPoint)
        {
            NumberOfWorkers = authoringWorkerCount;
            SpawnPoint = spawnPoint;
        }
    }
}