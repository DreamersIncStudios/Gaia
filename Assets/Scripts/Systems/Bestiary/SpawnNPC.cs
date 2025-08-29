using DreamersIncStudio.GAIACollective;
using Systems.Bestiary;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.BestiarySystem
{
    public static partial class BestiaryDB
    {
        public static void Spawn(uint ID, uint HomeBiomeID, uint PlayerLevel, Entity parentToLink)
        {
            var info = GetCreature(ID);
            new CharacterBuilder(info.Name, out var entity)
                .WithActiveHour(info.ActiveTimesOfDay,HomeBiomeID)
                .AtLevel(info.LevelRange, PlayerLevel)
                .WithParent(parentToLink)
                .Build();
        }
    }
}