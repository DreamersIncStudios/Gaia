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
            
        }
    }
}