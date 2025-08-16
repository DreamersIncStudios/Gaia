using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    public partial class GaiaUpdateGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<RunningTag>();
            RequireForUpdate<GaiaTime>();
            RequireForUpdate<WorldManager>();
        }
        public GaiaUpdateGroup()
        {
            RateManager = new RateUtils.VariableRateManager(80, true);
        }
    }
    [UpdateInGroup(typeof(GaiaUpdateGroup))]
    public partial class GaiaSpawnSystem : SystemBase
    {
        
        protected override void OnUpdate()
        {
   
            if (!SystemAPI.TryGetSingleton<GaiaControl>(out _))
            {
                var gaiaEntity = SystemAPI.GetSingletonEntity<GaiaTime>();
                EntityManager.AddComponentData(gaiaEntity, new GaiaControl(10));
            }
            var worldManager = SystemAPI.GetSingleton<WorldManager>();

            #region Spawning

            var updateHashMap = false;
            Entities.WithStructuralChanges().ForEach((ref GaiaSpawnBiome biome) =>
            {
                for (var index = 0; index < biome.SpawnData.Length; index++)
                {
                    var spawn = biome.SpawnData[index];
                    spawn.Countdown(SystemAPI.Time.DeltaTime);
                    if (spawn.IsSatisfied)
                    {
                        if(spawn.Respawn)
                            spawn.ResetRespawn();
                    }
                    else if (spawn.Respawn)
                    {
                        spawn.Spawn(ref biome.SpawnRequests,biome.BiomeID,biome.LevelRange*(int)worldManager.WorldLevel, worldManager.PlayerLevel);
                        Debug.Log("Spawning ");
                        updateHashMap = true;
                    }

       
                    biome.SpawnData[index] = spawn;
                }

                #region Pack Spawn
                for (var i = 0; i < biome.PacksToSpawn.Length; i++)
                {
                    var packInfo = biome.PacksToSpawn[i];
                    if(packInfo.Satisfied) continue;
                    // ReSharper disable once Unity.BurstFunctionSignatureContainsManagedTypes
                    var baseEntityArch = EntityManager.CreateArchetype(
                        new ComponentType[]
                        {
                            typeof(LocalTransform),
                            typeof(LocalToWorld)
                        }
                    );
                    var baseDataEntity = EntityManager.CreateEntity(baseEntityArch);

                    switch (packInfo.PackType)
                    {
                        case PackType.Assault:
                            EntityManager.AddComponentData(baseDataEntity, Pack.AssaultTeam(biome.BiomeID));
                           
                            break;
                        case PackType.Support:
                            EntityManager.AddComponentData(baseDataEntity, Pack.Support(biome.BiomeID));

                            break;
                        case PackType.Transport:
                            break;
                        case PackType.Scavengers:
                            break;
                        case PackType.Recon:
                            break;
                        case PackType.Combat:
                            break;
                        case PackType.Acquisition:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    packInfo.Qty++;
                    biome.PacksToSpawn[i] = packInfo;
                }
                #endregion
                
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