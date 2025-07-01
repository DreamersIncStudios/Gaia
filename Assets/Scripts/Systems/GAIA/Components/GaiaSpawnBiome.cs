using System.Collections.Generic;
using Systems.Bestiary;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Biome = DreamersIncStudio.GAIACollective.GaiaSpawnBiome;
namespace DreamersIncStudio.GAIACollective.Authoring
{
    public class GaiaSpawnBiome : MonoBehaviour
    {    
        public uint BiomeID;
        public List<SpawnData> SpawnData;
        public class Baker : Baker<GaiaSpawnBiome>
        {
            public override void Bake(GaiaSpawnBiome authoring)
            {
                var entity = GetEntity(TransformUsageFlags.WorldSpace);   
                AddComponent(entity, new Biome(authoring));
            }
        }
    
    }
}
namespace DreamersIncStudio.GAIACollective
{
    public struct GaiaSpawnBiome: IComponentData
    {
        public uint BiomeID;
        public FixedList512Bytes<SpawnData> SpawnData;
        public GaiaSpawnBiome( Authoring.GaiaSpawnBiome gaiaSpawnBiome)
        {
            BiomeID = gaiaSpawnBiome.BiomeID;
            SpawnData = new FixedList512Bytes<SpawnData>();
            foreach (var spawn in gaiaSpawnBiome.SpawnData)
            {
                SpawnData.Add(spawn);
            }
        }
    }
  

    [System.Serializable]
    public struct SpawnData
    {
        public uint SpawnID;
        public TimesOfDay ActiveHours;
        public uint Qty;
        public uint qtySpawned;
        public bool IsSatisfied => qtySpawned >= Qty;
            public bool Respawn => respawnTime <= 0.0f;
        public float respawnTime;
        public int RespawnInterval;
            public void Spawn()
            {
                var cnt = Qty - qtySpawned;
                for (var i = 0; i < cnt; i++)
                {
                    new CharacterBuilder("spawn", out var entity)
                        .WithActiveHour(ActiveHours)
                        .Build();
                    qtySpawned++;
                }
                ResetRespawn();
            }

            public void ResetRespawn()
            {
                respawnTime = Random.Range(.855f*400.0f, 1.075f* 400.0f);
                
            }

            public void Countdown(float time)
            {
                respawnTime -= time;
            }

            public void SpawnKiller()
            {
                if(qtySpawned==0) return;
                qtySpawned--;
            }
    }
}