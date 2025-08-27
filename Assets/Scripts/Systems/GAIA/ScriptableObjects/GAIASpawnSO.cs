using System.Collections.Generic;

namespace DreamersIncStudio.GAIACollective
{
    public class GAIASpawnSO: SubSceneBakingSystem, ISpawnSpecial
    {
        public uint BiomeID { get; }
        public SpawnScenario SpawnScenario { get; }
        public List<SpawnData> SpawnData { get; }
        
        public void LoadSpawnData()
        {
            
        }
    }
}