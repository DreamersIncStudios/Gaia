using System.Collections.Generic;

namespace DreamersIncStudio.GAIACollective
{
    public interface ISpawnSpecial
    {
        public uint BiomeID { get; }
        public SpawnScenario SpawnScenario { get; }
        public List<SpawnData> SpawnData { get; }
        
        public void LoadSpawnData();
    }
}