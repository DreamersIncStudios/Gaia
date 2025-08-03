using Unity.Entities;
using Unity.Mathematics;

namespace DreamersIncStudio.GAIACollective.Streaming.SceneManagement.SectionMetadata
{
    public struct GaiaOperationArea : IComponentData
    {
        public float Radius; // Proximity radius within which to consider loading a section
        public float3 Center;
    }
}