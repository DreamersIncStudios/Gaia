using System;
using DreamersIncStudio.FactionSystem;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    public struct GaiaLife: IComponentData
    {
        public TimesOfDay ActiveTimeOfDay;
        public uint HomeBiomeID;
        public Role Role; // Move to IAUS Brain 
        public FactionNames FactionID;
        public GaiaLife(TimesOfDay activeHour, uint HomeBiomeID, FactionNames factionID)
        {
            ActiveTimeOfDay = activeHour;
            this.HomeBiomeID = HomeBiomeID;
            Role = Role.Combat;
            FactionID = factionID;
        }
    }

    public struct GaiaControl: IComponentData
    {
        public NativeParallelMultiHashMap<uint, AgentInfo> entityMapTesting;

        public GaiaControl(int capacity = 0)
        {
            entityMapTesting = new NativeParallelMultiHashMap<uint, AgentInfo>(capacity, Allocator.Persistent);
        }
    }

    [Flags]
    public enum TimesOfDay
    {
        Daybreak = 1 << 0,
        Midday = 1 << 2,
        Sunset = 1 << 3,
        Night = 1 << 4
    }
}