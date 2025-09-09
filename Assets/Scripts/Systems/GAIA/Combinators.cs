using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AISenses;
using DreamersIncStudio.FactionSystem;
using DreamersIncStudio.GAIACollective;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Combinators
{
    public interface IContext
    {
        public float3 Origin { get; }
        public  float ViewAngle{ get; }
        public float3 Forward {get; }
        public float r2 {get; }
    }

    public readonly struct TargetCtx: IContext
    {
        public  float3 Origin { get;  }
        public  float ViewAngle{ get;  }
        public  float3 Forward{ get;  }
        public float r2{ get;  }
        public readonly CollisionFilter Filter;
        public readonly CollisionWorld World;
        public readonly IEnumerable<Relationship> Relationships;
        public TargetCtx(LocalTransform transform, Vision vision, FactionNames factionID, CollisionWorld world,
            DynamicBuffer<Factions> factionsBuffer, CollisionFilter filter)
        {
            this.Origin = transform.Position;
            Forward = transform.Forward();
            this.ViewAngle = vision.ViewAngle;
            this.World = world;
            this.r2 = vision.ViewRadius * vision.ViewRadius;
            this.Filter = filter;
            Relationships = new List<Relationship>();
            foreach (var factions in factionsBuffer)
            {
                if (factions.Faction != factionID) continue;
                Relationships = factions.Relationships;
                break;
            }
        }

    
    }
    interface IPred
    {
        bool Test(Stats stat, LocalTransform transform, in TargetCtx ctx);
        List<TargetQuadrantData> Test(List<TargetQuadrantData> targets, LocalTransform transform, in TargetCtx ctx);
    }
    
//Combinators for Predicates
            readonly struct And<A,B>: IPred where A:IPred where B:IPred
            {
                public readonly A a;
                public readonly B b;
                public And(A a, B b)
                {
                    this.a = a;
                    this.b = b;
                }
                [method:MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool Test(Stats stat, LocalTransform transform, in TargetCtx ctx)
                {
                    return a.Test(stat, transform, in ctx) && b.Test(stat, transform, in ctx);
                }
                
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public List<TargetQuadrantData> Test(List<TargetQuadrantData> targets, LocalTransform transform,
                    in TargetCtx ctx)
                {
                    var afterA = a.Test(targets, transform, in ctx);
                    return b.Test(afterA, transform, in ctx);
                }
            }
            // Fluent Builder
            readonly struct Chain<TPred> where TPred : struct, IPred
            {
                public readonly TPred pred;

                public Chain(TPred pred)
                {
                    this.pred = pred;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Chain<And<TPred, Tnext>> And<Tnext>(Tnext n) where Tnext : struct, IPred =>
                    new(new And<TPred, Tnext>(pred, n));
                public TPred Build() => pred;
            }

            static class PredChain
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Chain<TLeaf> Start<TLeaf>(TLeaf leaf) where TLeaf : struct, IPred =>
                    new(leaf);
            }
            // Predicates

           
}
