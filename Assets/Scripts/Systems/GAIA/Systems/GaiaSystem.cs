using System.Collections.Generic;
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

            #region Spawning
             var   entities = new List<AgentInfo>();
             var existingEntities = new HashSet<AgentInfo>();
            Entities.WithoutBurst().ForEach((ref GaiaSpawnBiome biome) =>
            {   
                if(entityMapTesting.TryGetFirstValue(biome.BiomeID, out var value, out var iterator))
                    do
                    {
                        existingEntities.Add(value);
                    }while (entityMapTesting.TryGetNextValue(out value, ref iterator));

                switch (timeOfDay)
                {
                    case < 6f:
                        if(biome.Daybreak.IsSatisfied)return;
                        Debug.Log("Daybreak Spawn");
                        foreach (var spawnData in biome.Daybreak.SpawnData)
                        {
                            for (int i = 0; i < spawnData.Qty; i++)
                            {
                                var agent = new AgentInfo(biome.BiomeID, spawnData.SpawnID);
                                if(existingEntities.Contains(agent))continue;
                                entities.Add(new AgentInfo(biome.BiomeID,spawnData.SpawnID));
                            }
                        }

                        biome.Daybreak.IsSatisfied = true;
                        biome.Midday.IsSatisfied = false;
                        biome.Sunset.IsSatisfied = false;
                        biome.Night.IsSatisfied = false;
                        break;
                    case < 12f:
                        if(biome.Midday.IsSatisfied)return;
                        Debug.Log("Midday Spawn");
                        foreach (var spawnData in biome.Daybreak.SpawnData)
                        {
                            for (int i = 0; i < spawnData.Qty; i++)
                            {
                                var agent = new AgentInfo(biome.BiomeID, spawnData.SpawnID);
                                if(existingEntities.Contains(agent))continue;
                                entities.Add(new AgentInfo(biome.BiomeID,spawnData.SpawnID));
                            }
                        }
                        biome.Midday.IsSatisfied = true;
                        biome.Daybreak.IsSatisfied = false;
                        biome.Sunset.IsSatisfied = false;
                        biome.Night.IsSatisfied = false;
                        break;
                    case < 18f:
                        if(biome.Sunset.IsSatisfied)return;
                        Debug.Log("Sunset Spawn");
                        foreach (var spawnData in biome.Daybreak.SpawnData)
                        {
                            for (int i = 0; i < spawnData.Qty; i++)
                            {
                                var agent = new AgentInfo(biome.BiomeID, spawnData.SpawnID);
                                if(existingEntities.Contains(agent))continue;
                                entities.Add(new AgentInfo(biome.BiomeID,spawnData.SpawnID));
                            }
                        }
                        biome.Sunset.IsSatisfied = true;
                        biome.Daybreak.IsSatisfied = false;
                        biome.Midday.IsSatisfied = false;
                        biome.Night.IsSatisfied = false;
                        break;
                    default:
                        if(biome.Night.IsSatisfied)return;
                        Debug.Log("Night Spawn");
                        foreach (var spawnData in biome.Daybreak.SpawnData)
                        {
                            for (int i = 0; i < spawnData.Qty; i++)
                            {
                                var agent = new AgentInfo(biome.BiomeID, spawnData.SpawnID);
                                if(existingEntities.Contains(agent))continue;
                                entities.Add(new AgentInfo(biome.BiomeID,spawnData.SpawnID));
                            }
                        }
                        biome.Night.IsSatisfied = true;
                        biome.Midday.IsSatisfied = false;
                        biome.Sunset.IsSatisfied = false;
                        biome.Daybreak.IsSatisfied = false;
                        break;
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