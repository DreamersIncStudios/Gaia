using Global.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


public struct Interactable :  IBufferElementData
{
    
}

public partial class InteractableManagement : SystemBase
{
    private EntityQuery interactableQuery;
    private NativeParallelMultiHashMap<int, TargetQuadrantData> quadrantMultiHashMap;
    private const int QuadrantYMultiplier = 1000;
    private const int QuadrantCellSize = 50;
    private EntityQuery query;
    
    private static int GetPositionHashMapKey(float3 position)
    {
        return (int)(Mathf.Floor(position.x / QuadrantCellSize) +
                     (QuadrantYMultiplier * Mathf.Floor(position.z / QuadrantCellSize)));
    }
    
    protected override void OnCreate()
    {
        RequireForUpdate<PhysicsWorldSingleton>();
        interactableQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadOnly(typeof(AITarget)), }
        });
            quadrantMultiHashMap = new NativeParallelMultiHashMap<int, TargetQuadrantData>(0, Allocator.Persistent);
        
    }

    protected override void OnUpdate()
    {
        UpdateQuadrantHashMap();

        EntityManager.CompleteDependencyBeforeRO<PhysicsWorldSingleton>();
        var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        quadrantMultiHashMap.Dispose();
    }

    partial struct InteractableSort : IJobEntity
    {
        public NativeArray<LocalToWorld> targetsTransform;
        public NativeArray<AITarget> targets;
        void Execute(Entity entity, DynamicBuffer<Interactable> interactables, in LocalToWorld transform)
        {
            
        }
    }
    void UpdateQuadrantHashMap()
    {

        if (query.CalculateEntityCount() != quadrantMultiHashMap.Capacity)
        {
            quadrantMultiHashMap.Clear();
            quadrantMultiHashMap.Capacity = query.CalculateEntityCount() + 1;
        }

        new SetQuadrantDataHashMapJob()
        {
            QuadrantMap = quadrantMultiHashMap.AsParallelWriter()
        }.ScheduleParallel(query);
    }

    [BurstCompile]

    partial struct SetQuadrantDataHashMapJob : IJobEntity
    {
        public NativeParallelMultiHashMap<int, TargetQuadrantData>.ParallelWriter QuadrantMap;

        private void Execute(Entity entity, [ReadOnly] in LocalTransform transform, in AITarget target)
        {
            var hashMapKey = GetPositionHashMapKey(transform.Position);
            QuadrantMap.Add(hashMapKey, new TargetQuadrantData
            {
                Entity = entity,
                Position = transform.Position,
                TargetInfo = target
            });
        }
    }
    public struct TargetQuadrantData
    {
        public Entity Entity;
        public float3 Position;
        public AITarget TargetInfo;
        public float Distance { get; set; }
    }
}


