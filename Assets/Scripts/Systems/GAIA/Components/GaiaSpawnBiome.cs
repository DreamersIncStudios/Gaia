using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Biome = DreamersIncStudio.GAIACollective.GaiaSpawnBiome;
namespace DreamersIncStudio.GAIACollective.Authoring
{
    public class GaiaSpawnBiome : MonoBehaviour
    {    
        public uint BiomeID;
        public TimeSpawnSettings Daybreak;
        public TimeSpawnSettings Midday;
        public TimeSpawnSettings Sunset;
        public TimeSpawnSettings Night;
        public class Baker : Baker<GaiaSpawnBiome>
        {
            public override void Bake(GaiaSpawnBiome authoring)
            {
                var entity = GetEntity(TransformUsageFlags.WorldSpace);   
                AddComponent(entity, new Biome(authoring));
            }
        }
        [System.Serializable]
        public struct TimeSpawnSettings
        {
            public List<SpawnData> SpawnData; 
            public bool IsSatisfied;
        }
    }
}
namespace DreamersIncStudio.GAIACollective
{
    public struct GaiaSpawnBiome: IComponentData
    {
        public uint BiomeID;
        public TimeSpawnSettings Daybreak;
        public TimeSpawnSettings Midday;
        public TimeSpawnSettings Sunset;
        public TimeSpawnSettings Night;
    
        public GaiaSpawnBiome( Authoring.GaiaSpawnBiome gaiaSpawnBiome)
        {
            Daybreak.SpawnData = new FixedList512Bytes<SpawnData>();
            Daybreak.IsSatisfied = false;
            
            Midday.SpawnData = new FixedList512Bytes<SpawnData>();
            Midday.IsSatisfied = false;
            
            Night.SpawnData = new FixedList512Bytes<SpawnData>();
            Night.IsSatisfied = false;
            
            Sunset.SpawnData = new FixedList512Bytes<SpawnData>();
            Sunset.IsSatisfied = false;

            foreach(var spawnData in gaiaSpawnBiome.Daybreak.SpawnData)
                Daybreak.SpawnData.Add(spawnData);
            foreach(var spawnData in gaiaSpawnBiome.Midday.SpawnData)
                Midday.SpawnData.Add(spawnData);
            foreach(var spawnData in gaiaSpawnBiome.Night.SpawnData)
                Night.SpawnData.Add(spawnData);
            foreach(var spawnData in gaiaSpawnBiome.Sunset.SpawnData)
                Sunset.SpawnData.Add(spawnData);
            BiomeID = gaiaSpawnBiome.BiomeID;
        }
    }
    [System.Serializable]
    public struct TimeSpawnSettings
    {
       public FixedList128Bytes<SpawnData> SpawnData; 
       public bool IsSatisfied;
    }

    [System.Serializable]
    public struct SpawnData
    {
        public uint SpawnID;
        public uint Qty;
            public bool IsSatisfied;
    }
}