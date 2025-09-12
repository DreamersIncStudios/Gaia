using System.Collections.Generic;
using DreamersIncStudio.FactionSystem;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Global.Component;
// ReSharper disable FunctionRecursiveOnAllPaths

namespace AISenses
{
    
    public interface IInteractables
    {
        public float Dist { get; }

    }

    [InternalBufferCapacity(0)]
    public struct Enemies : IBufferElementData,IInteractables
    {
        public Target target;
        public float Dist { get; }

        public static implicit operator Target(Enemies e) { return e; }
        public static implicit operator Enemies(Target e) { return new Enemies { target = e }; }
    }
    [InternalBufferCapacity(0)]
    public struct Allies : IBufferElementData,IInteractables
    {
        public Target target;
        public float Dist { get; }

        public static implicit operator Target(Allies e) { return e; }
        public static implicit operator Allies(Target e) { return new Allies() { target = e }; }
    }
    [InternalBufferCapacity(0)]
    public struct PlacesOfInterest : IBufferElementData,IInteractables
    {
        public Target target;
        public float Dist { get; }

        public static implicit operator Target(PlacesOfInterest e) { return e; }
        public static implicit operator PlacesOfInterest(Target e) { return new PlacesOfInterest() { target = e }; }
    }
    [InternalBufferCapacity(0)]
    public struct Resources : IBufferElementData,IInteractables
    {
        public Target target;
        public float Dist { get; }

        public static implicit operator Target(Resources e) { return e; }
        public static implicit operator Resources(Target e) { return new Resources() { target = e }; }
    }

    public struct SortScanPositionByDistance : IComparer<Enemies>
    {
        public int Compare(Enemies x, Enemies y)
        {
            return x.Dist.CompareTo(y.Dist);
        }
    }

    public struct HitDistanceComparer : IComparer<Enemies>
    {
        public int Compare(Enemies lhs, Enemies rhs)
        {
            return lhs.Dist.CompareTo(rhs.Dist);
        }
    }

    public struct Target
    {
        public Entity Entity;
        public Affinity Affinity;
        public AITarget TargetInfo;
        public float DistanceTo;
        public float3 LastKnownPosition;
        public bool CanSee;
        public int LookAttempt;
        public bool CantFind => LookAttempt > 3;
        public float PerceptilabilityScore;
    }

 
}