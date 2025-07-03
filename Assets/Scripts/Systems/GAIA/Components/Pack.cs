
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace DreamersIncStudio.GAIACollective
{
    public struct Pack : IComponentData
    {
        public FixedList128Bytes<PackRole>  Requirements;
        public Entity LeaderEntity;
        public float CohesionFactor;
        public float SeparationFactor;
        public float AlignmentFactor;
        public float3 HerdCenter; // Central point for the herd
        public int MemberCount;
        

        public Pack AssaultTeam() => new Pack()
        {
            Requirements =  new FixedList128Bytes<PackRole>()
            {
                new PackRole(Role.Recon, new int2(1,1)),
                new PackRole(Role.Combat, new int2(1,1)),
                new PackRole(Role.Scavengers, new int2(1,1)),
                new PackRole(Role.Transport, new int2(1,1)),
                new PackRole(Role.Acquisition, new int2(1,1)),
                new PackRole(Role.Support, new int2(1,1)),
            },
            CohesionFactor = 1.0f,
            SeparationFactor = 2.0f,
            AlignmentFactor = 0.5f,
        };
        
        
        public Pack Support() => new Pack()
        {
            Requirements =  new FixedList128Bytes<PackRole>()
            {
                new PackRole(Role.Recon, new int2(1,3)),
                new PackRole(Role.Combat, new int2(1,2)),
                new PackRole(Role.Scavengers, new int2(0,0)),
                new PackRole(Role.Transport, new int2(1,1)),
                new PackRole(Role.Acquisition, new int2(0,0)),
                new PackRole(Role.Support, new int2(2,3)),
            },
            CohesionFactor = 1.0f,
            SeparationFactor = 2.0f,
            AlignmentFactor = 0.5f,
        };
    }

    public struct PackMember : IComponentData
    {
        public Entity PackEntity;
    }

    public struct PackRole
    {
        public Role Role;
        public int2 QtyRange;

        public PackRole(Role role, int2 qtyRange)
        {
           Role = role;
           QtyRange = qtyRange;
        }
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