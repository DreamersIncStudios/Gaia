using DreamersIncStudio.GAIACollective;
using Unity.Entities;
using Unity.Transforms;

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

        public CharacterBuilder WithActiveHour(TimesOfDay activeHour)
        {
            manager.AddComponentData(entity, new GaiaLife(activeHour));
            return this;
        }


        public Entity Build()
        {

            return entity;
        }
    }
}