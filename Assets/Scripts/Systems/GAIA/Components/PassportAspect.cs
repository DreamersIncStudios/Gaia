using DreamersIncStudio.GAIACollective;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    [WithNone(typeof(PackMember)) ]
    public readonly partial struct PassportAspect : IAspect
    {
        private readonly RefRO<GaiaLife> life;
        private readonly RefRO<Stats> stat;
        public uint ID => life.ValueRO.HomeBiomeID;
        public Role Role => life.ValueRO.Role;
        public int Level => (int)stat.ValueRO.Level;
    }
}