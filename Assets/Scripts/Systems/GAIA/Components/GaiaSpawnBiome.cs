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
        private uint qtySpawned;
        public bool IsSatisfied => qtySpawned >= Qty;
            public bool Respawn => respawnTime <= 0.0f;
        private float respawnTime;
        [Range(0,20)]
        public int RespawnInterval;
            public void Spawn(uint HomeBiomeID)
            {
                var entities = new List<Entity>();
                var cnt = Qty - qtySpawned;
                for (var i = 0; i < cnt; i++)
                {
                    new CharacterBuilder("spawn", out var entity)
                        .WithActiveHour(ActiveHours,HomeBiomeID)
                        .Build();
                    qtySpawned++;
                    entities.Add(entity);
                }
                ResetRespawn();
         
            }

            public void ResetRespawn()
            {
                var interval = 60.0f * RespawnInterval;
                respawnTime = Random.Range(.855f*interval, 1.075f* interval);
                
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