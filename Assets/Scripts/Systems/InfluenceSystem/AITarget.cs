using DreamersInc.InfluenceMapSystem;
using DreamersIncStudio.FactionSystem;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Global.Component
{
    [System.Serializable]
    public struct AITarget : IComponentData
    {
        public TargetType Type;
        public uint level {get; set; }
        public FactionNames FactionID;
        public int NumOfEntityTargetingMe;
        [HideInInspector] public int GetInstanceID;
        public bool CanBeTargeted => NumOfEntityTargetingMe < 2;
        [HideInInspector] public int MaxNumberOfTarget; // base off of InfluenceValue Level
        public bool CanBeTargetByPlayer;
        public float3 CenterOffset;
        
        public float detectionScore;

    }
    [System.Serializable]
    public enum TargetType
    {
        None, Character, Location, Vehicle
    }




    public partial class UpdateAITarget : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref AITarget target, ref Perceptibility perceptibility) =>
            {
                target.detectionScore = perceptibility.Score;
            }).Schedule();
        }
    }
    
}