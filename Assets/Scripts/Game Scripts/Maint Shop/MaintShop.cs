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
        public MaintShopSetup(uint authoringWorkerCount)
        {
            NumberOfWorkers = authoringWorkerCount;
        }
    }
}