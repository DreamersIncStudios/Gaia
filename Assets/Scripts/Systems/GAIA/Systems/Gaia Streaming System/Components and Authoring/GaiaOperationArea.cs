using Unity.Entities;
using Unity.Mathematics;

namespace DreamersIncStudio.GAIACollective.Streaming.SceneManagement.SectionMetadata
{
    public struct GaiaOperationArea : IComponentData
    {
        public float Radius; // Proximity radius within which to consider loading a section
        public float3 Center;

        public int Layer => Radius switch
        {
            750 => 6,
            500 => 9,
            250 => 26,
            100 => 27,
            85 => 28,
            _ => 0
        };

        public GaiaOperationArea(Authoring.GaiaSpawnBiome authoring)
        {
            Center = authoring.transform.position;
            Radius = authoring.gameObject.layer switch
            {
                6 => 750,
                9 or 10 or 11 => 500,
                26 => 250,
                27 => 100,
                28 => 85,
                _ => 2250
            };
        }
    }
}