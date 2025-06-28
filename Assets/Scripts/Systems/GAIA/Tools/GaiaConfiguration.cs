using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    [CreateAssetMenu(fileName = "Config", menuName = "GAIA/Configuration", order = 1)]
    public class GaiaConfiguration : ScriptableObject
    {
        public float CycleSpeed => cycleSpeed;
        [SerializeField] private float cycleSpeed =.2f;
        public bool EnableFogControl => enableFogControl;
        [SerializeField] bool enableFogControl = true;
        [Header("Daybreak Settings")]
        public TimeSettings Daybreak;

        [Header("Midday Settings")]
        public TimeSettings Midday;

        [Header("Sunset Settings")]
        public TimeSettings Sunset;

        [Header("Night Settings")]
        public TimeSettings Night;
    
    }
    [System.Serializable]
    public struct TimeSettings
    {
        [InspectorName("Scene Ambient")] public Color ambientColor;
        [InspectorName("Sun Color")] public Color sunColor;
        [InspectorName("Camera Background")] public Color backgroundColor;
        [InspectorName("Sun Intensity")] public float sunIntensity;
        [InspectorName("Shadow Strength"), Range(0f, 1f)] public float shadowStrength;

        [Header("Fog Settings")]
        [InspectorName("Fog Color")] public Color fogColor;
        [InspectorName("Fog Density")] public float fogDensity;

        [Header("Water Settings")]
        [InspectorName("Water Color")] public Color waterColor;
    }
}