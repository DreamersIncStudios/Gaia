using System;
using DreamersIncStudio.GAIACollective;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Systems.Bestiary
{
    public struct CharacterBuilder
    {
        
        private Entity entity;
        private EntityManager manager;
        public CharacterBuilder(string entityName,out Entity spawnedEntity)
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var baseEntityArch = manager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld)
            );
            var baseDataEntity = manager.CreateEntity(baseEntityArch);
            manager.SetName(baseDataEntity, entityName != string.Empty ? entityName : "NPC Data");
            manager.SetComponentData(baseDataEntity, new LocalTransform() { Scale = 1 });
            spawnedEntity = entity = baseDataEntity;
          
        }

        public CharacterBuilder WithActiveHour(TimesOfDay activeHour, uint HomeBiomeID)
        {
            manager.AddComponentData(entity, new GaiaLife(activeHour, HomeBiomeID));
            return this;
        }


        public Entity Build()
        {

            return entity;
        }

        public CharacterBuilder AtLevel(int2 range, uint playerLevel)
        {
           int spawnLevel;
           var random = Random.CreateFromIndex((uint)DateTime.Now.Ticks);
           if (random.NextFloat() < 0.65f) // 70% chance to favor player's level ± 2
           {
               spawnLevel = random.NextInt(
                   math.max((int)playerLevel - 2, range.x),
                   math.min((int)playerLevel + 2, range.y) + 1
               );
           }
           else // 30% chance to spawn in the extended range
           {
               spawnLevel = random.NextInt(range.x, range.y + 1);
           }
           manager.AddComponentData(entity, new Stats() { Level = (uint)spawnLevel });
           return this;
        }
    }
}