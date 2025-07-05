using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    [UpdateInGroup(typeof(GaiaUpdateGroup))]
    [UpdateAfter(typeof(GaiaSpawnSystem))]
    public partial struct GaiaPackSystem : ISystem
    {
        private EntityQuery packQuery;
        private EntityQuery agentsQuery;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RunningTag>();
            state.RequireForUpdate<GaiaTime>();
            packQuery = state.GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalTransform)),ComponentType.ReadWrite(typeof(Pack)) }
            });
            
            agentsQuery = state.GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] {ComponentType.ReadOnly(typeof(LocalTransform)), ComponentType.ReadOnly(typeof(GaiaLife)) }
            });
        }

        public void OnUpdate(ref SystemState state)
        {
            var control = SystemAPI.GetSingleton<GaiaControl>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            new FindLeader()
            {
                PackEntities = packQuery.ToEntityArray(Allocator.TempJob),
                PackLookup = state.GetComponentLookup<Pack>(false),
                ecb = ecb.CreateCommandBuffer(state.WorldUnmanaged)
                
            }.Schedule();
        }
        
    }

    [WithNone(typeof(PackMember))]

    public partial struct FindLeader: IJobEntity
    {
        public EntityCommandBuffer ecb;
        public NativeArray<Entity> PackEntities;
        public ComponentLookup<Pack> PackLookup;
        private void Execute(Entity entity,[ChunkIndexInQuery] int chunkIndex, PassportAspect aspect)
        {
            foreach (var packEntity in PackEntities)
            {
                var pack = PackLookup[packEntity];
                if (pack.LeaderEntity != Entity.Null) continue;
                if (pack.Role != aspect.Role) continue;
                pack.LeaderEntity = entity;
                pack.MemberCount++;
                ecb.AddComponent(entity, new PackMember(packEntity ));;
                PackLookup[packEntity]= pack;
            }
        }
    }
}