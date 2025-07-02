using System.Collections.Generic;
using System.Linq;
using Systems.Bestiary;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
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
            RequireForUpdate<GaiaTime>();
            entityMapTesting = new NativeParallelMultiHashMap<uint, AgentInfo>(0, Allocator.Persistent);

        }


        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton<GaiaControl>(out _))
            {
                var gaiaEntity = SystemAPI.GetSingletonEntity<GaiaTime>();
                EntityManager.AddComponentData(gaiaEntity, new GaiaControl(10));
            }

            var gaiaTime = SystemAPI.GetSingletonRW<GaiaTime>();
            var gaiaSettings = SystemAPI.GetSingleton<GaiaLightSettings>();
            gaiaTime.ValueRW.UpdateTime(SystemAPI.Time.DeltaTime);
            var timeOfDay = gaiaTime.ValueRO.TimeOfDay;

            #region Lighting

            Entities.WithoutBurst().ForEach((Light light) =>
            {
                var rotationPivot = light.transform;
                float timePercent = gaiaTime.ValueRO.TimeOfDay / 24f;
                float xRotation = (timePercent * 360f) - 90f;
                if (rotationPivot.localRotation.eulerAngles.x != xRotation ||
                    rotationPivot.localRotation.eulerAngles.y != -30)
                {
                    rotationPivot.localRotation = Quaternion.Euler(new Vector3(xRotation, -30, 0));
                }

                TimeLightingSettings from, to;
                float blend;
                switch (timeOfDay)
                {
                    case < 6f:
                        from = gaiaSettings.Night;
                        to = gaiaSettings.Daybreak;
                        blend = timeOfDay / 6f;
                        break;
                    case < 12f:
                        from = gaiaSettings.Daybreak;
                        to = gaiaSettings.Midday;
                        blend = (timeOfDay - 6f) / 6f;
                        break;
                    case < 18f:
                        from = gaiaSettings.Midday;
                        to = gaiaSettings.Sunset;
                        blend = (timeOfDay - 12f) / 6f;
                        break;
                    default:
                        from = gaiaSettings.Sunset;
                        to = gaiaSettings.Night;
                        blend = (timeOfDay - 18f) / 6f;
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

            var updateHashMap = false;
            Entities.WithStructuralChanges().ForEach((ref GaiaSpawnBiome biome) =>
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
                       spawn.Spawn(biome.BiomeID);
                       updateHashMap = true;
                            break;
                    }

                    spawn.Countdown(SystemAPI.Time.DeltaTime);
                    biome.SpawnData[index] = spawn;
                }

            }).Run();
            if (!updateHashMap) return;
            var control = SystemAPI.GetSingleton<GaiaControl>();
            Entities.ForEach((Entity entity, ref GaiaLife life) =>
            {
                if (control.entityMapTesting.IsCreated)
                {
                    control.entityMapTesting.Clear();
                }
                control.entityMapTesting.Add(life.HomeBiomeID, new AgentInfo(entity));
            }).Schedule();


            #endregion

        }
    }

    public struct AgentInfo
    {
       
        public Entity AgentEntity;

        public AgentInfo( Entity entity = default)
        {
            
            AgentEntity = entity;
        }
    }

}