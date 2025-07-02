using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace DreamersIncStudio.GAIACollective
{
    public struct Pack : IComponentData
    {
        public FixedList128Bytes<Entity> Members;
        public FixedList128Bytes<PackRole>  Requirements;
    }

    public struct PackRole
    {
        public Role Role;
        public int2 QtyRange;
    }

    public enum Role
    {
        Recon,
        Combat, 
       Scavengers,
       Transport,
       Acquisition,
        Support 
        
    }
}