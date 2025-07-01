using System.Collections.Generic;
using Systems.Bestiary;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    public partial class GaiaUpdateGroup : ComponentSystemGroup
    {
        public GaiaUpdateGroup()
        {
            RateManager = new RateUtils.VariableRateManager(80, true);
        }
    }

    public partial class GaiaSystem : SystemBase
    {
        private NativeParallelMultiHashMap<uint, AgentInfo> entityMapTesting;
        protected override void OnCreate()
        {
            RequireForUpdate<RunningTag>();
            entityMapTesting = new NativeParallelMultiHashMap<uint, AgentInfo>(0, Allocator.Persistent);
        }

    
        protected override void OnUpdate()
        {
            var gaiaTime =  SystemAPI.GetSingletonRW<GaiaTime>();
            var gaiaSettings = SystemAPI.GetSingleton<GaiaLightSettings>();
            gaiaTime.ValueRW.UpdateTime(SystemAPI.Time.DeltaTime);
            var timeOfDay = gaiaTime.ValueRO.TimeOfDay;
            #region  Lighting
            Entities.WithoutBurst().ForEach((Light light) =>
            {
                var rotationPivot = light.transform;
                float timePercent = gaiaTime.ValueRO.TimeOfDay/24f;
                float xRotation = (timePercent * 360f) - 90f;
                if (rotationPivot.localRotation.eulerAngles.x != xRotation || rotationPivot.localRotation.eulerAngles.y != -30)
                {
                    rotationPivot.localRotation = Quaternion.Euler(new Vector3(xRotation, -30, 0));
                }
                
                TimeLightingSettings from, to;
                float blend;
                switch (timeOfDay)
                {
                    case < 6f:
                        from = gaiaSettings.Night; to = gaiaSettings.Daybreak; blend = timeOfDay/ 6f;
                        break;
                    case < 12f:
                        from = gaiaSettings.Daybreak; to = gaiaSettings.Midday; blend = (timeOfDay- 6f) / 6f;
                        break;
                    case < 18f:
                        from = gaiaSettings.Midday; to = gaiaSettings.Sunset; blend = (timeOfDay - 12f) / 6f;
                        break;
                    default:
                        from = gaiaSettings.Sunset; to = gaiaSettings.Night; blend = (timeOfDay - 18f) / 6f;
                        break;
                }
                RenderSettings.ambientLight = Color.Lerp(from.ambientColor, to.ambientColor, blend);
                light.color = Color.Lerp(from.sunColor, to.sunColor, blend);
                light.intensity = Mathf.Lerp(from.sunIntensity, to.sunIntensity, blend);
                light.shadowStrength = Mathf.Lerp(from.shadowStrength, to.shadowStrength, blend);
                
                if (gaiaSettings.EnableFogControl && RenderSettings.fog)
                {
                    RenderSettings.fogColor = Color.Lerp(from.fogColor, to.fogColor, blend);
                    RenderSettings.fogDensity = Mathf.Lerp(from.fogDensity, to.fogDensity, blend);
                }
            }).Run();
            #endregion
            
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var endBuffer = ecb.CreateCommandBuffer(World.Unmanaged);
            #region Spawning
             var   entities = new List<AgentInfo>();
             var existingEntities = new HashSet<AgentInfo>();
            Entities.WithoutBurst().ForEach((ref GaiaSpawnBiome biome) =>
            {
                for (var index = 0; index < biome.SpawnData.Length; index++)
                {
                    var spawn = biome.SpawnData[index];
                    switch (spawn)
                    {
                        case { IsSatisfied: true, Respawn: false }:
                            break;
                        case { Respawn: true, IsSatisfied: true }:
                            spawn.ResetRespawn();
                            break;
                        default:
                            spawn.Spawn(endBuffer);
                            break;
                    }
                    spawn.Countdown(SystemAPI.Time.DeltaTime);
                    biome.SpawnData[index] = spawn;
                }
            }).Run();
            if (entities.Count > 0)
            {
                entityMapTesting.Clear();
                entityMapTesting.Capacity= entities.Count; 
                foreach (var t in entities)
                    entityMapTesting.Add(0, t);
            }
            
            entities.Clear();
            
            #endregion

        }

        public struct AgentInfo
        {
            public uint BiomeID;
            public uint SpawnID;

            public AgentInfo(uint biomeBiomeID, uint spawnDataSpawnID)
            {
               BiomeID = biomeBiomeID;
                SpawnID = spawnDataSpawnID;
            }
        }
    }
}