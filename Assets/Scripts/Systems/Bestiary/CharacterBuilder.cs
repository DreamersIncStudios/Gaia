using DreamersIncStudio.GAIACollective;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Bestiary
{
    public struct CharacterBuilder
    {
        
        private Entity entity;
        private EntityManager manager;
        private EntityCommandBuffer buffer;
        public CharacterBuilder(string entityName, EntityCommandBuffer endBuffer, out Entity spawnedEntity)
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var baseEntityArch = manager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld)
            );
            var baseDataEntity = endBuffer.CreateEntity(baseEntityArch);
            endBuffer.SetName(baseDataEntity, entityName != string.Empty ? entityName : "NPC Data");
            endBuffer.SetComponent(baseDataEntity, new LocalTransform() { Scale = 1 });
            spawnedEntity = entity = baseDataEntity;
            buffer = endBuffer;
        }

        public CharacterBuilder WithActiveHour(TimesOfDay activeHour)
        {
            buffer.AddComponent(entity, new GaiaLife(activeHour));
            return this;
        }


        public Entity Build()
        {

            return entity;
        }
    }
}