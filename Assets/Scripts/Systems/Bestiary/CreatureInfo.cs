using System.Collections.Generic;
using System.Linq;
using DreamersIncStudio.GAIACollective;
using Sirenix.OdinInspector;
using Systems.Bestiary;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DreamersInc.BestiarySystem
{
    [CreateAssetMenu(fileName = "Creature Info", menuName = "Bestiary/Creature Info", order = 1)]
    public class CreatureInfo : ScriptableObject
    {
        [SerializeField] private uint creatureID;
        public uint ID { get { return creatureID; } }
        public int2 LevelRange { get; set; }

        public string Name;
        public uint ClassLevel;
        [EnumToggleButtons]public TimesOfDay ActiveTimesOfDay;
        public Role Role;

    }

    public static partial class BestiaryDB
    {
        private static List<CreatureInfo> creatures;
        private static bool IsLoaded { get; set; }
        private static void ValidateDatabase()
        {
            if (creatures == null || !IsLoaded)
            {
                creatures = new();
                IsLoaded = false;
            }
            else
            {
                IsLoaded = true;
            }
        }

        public static void LoadDatabase(bool ForceLoad = false)
        {

            if (IsLoaded && !ForceLoad)
                return;
            creatures = new List<CreatureInfo>();
            CreatureInfo[] creatureSO = Resources.LoadAll<CreatureInfo>("Bestiary/Creatures");
            foreach (var item in creatureSO)
            {
                if (!creatures.Contains(item))
                    creatures.Add(item);
            }
        }
        
        public static void ClearDatabase()
        {
            IsLoaded = false;
            creatures.Clear();

        }

        private static CreatureInfo GetCreature(uint id)
        {
            ValidateDatabase();
            LoadDatabase();
            return MonoBehaviour.Instantiate( creatures.FirstOrDefault(creature => creature.ID == id));
        }
 
    }
}