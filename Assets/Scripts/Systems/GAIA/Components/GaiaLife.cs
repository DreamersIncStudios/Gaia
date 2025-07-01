using System;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    public struct GaiaLife: IComponentData
    {
        public TimesOfDay ActiveTimeOfDay;

        public GaiaLife(TimesOfDay activeHour)
        {
            ActiveTimeOfDay = activeHour;
            
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