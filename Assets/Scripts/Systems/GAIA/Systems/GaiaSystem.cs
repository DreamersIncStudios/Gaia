using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{

    public partial class GaiaSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<RunningTag>();
        }

        protected override void OnUpdate()
        {
            var gaiaTime =  SystemAPI.GetSingletonRW<GaiaTime>();
            var gaiaSettings = SystemAPI.GetSingleton<GaiaLightSettings>();
            gaiaTime.ValueRW.UpdateTime(SystemAPI.Time.DeltaTime);
            var timeOfDay = gaiaTime.ValueRO.TimeOfDay;
            #region  Lighting
            Entities.WithoutBurst().ForEach((Light light) =>
            {
                var rotationPivot = light.transform;
                float timePercent = gaiaTime.ValueRO.TimeOfDay/24f;
                float xRotation = (timePercent * 360f) - 90f;
                if (rotationPivot.localRotation.eulerAngles.x != xRotation || rotationPivot.localRotation.eulerAngles.y != 0)
                {
                    rotationPivot.localRotation = Quaternion.Euler(new Vector3(xRotation, 0, 0));
                }
                
                TimeLightingSettings from, to;
                float blend;
                switch (timeOfDay)
                {
                    case < 6f:
                        from = gaiaSettings.Night; to = gaiaSettings.Daybreak; blend = timeOfDay/ 6f;
                        break;
                    case < 12f:
                        from = gaiaSettings.Daybreak; to = gaiaSettings.Midday; blend = (timeOfDay- 6f) / 6f;
                        break;
                    case < 18f:
                        from = gaiaSettings.Midday; to = gaiaSettings.Sunset; blend = (timeOfDay - 12f) / 6f;
                        break;
                    default:
                        from = gaiaSettings.Sunset; to = gaiaSettings.Night; blend = (timeOfDay - 18f) / 6f;
                        break;
                }
                RenderSettings.ambientLight = Color.Lerp(from.ambientColor, to.ambientColor, blend);
                light.color = Color.Lerp(from.sunColor, to.sunColor, blend);
                light.intensity = Mathf.Lerp(from.sunIntensity, to.sunIntensity, blend);
                light.shadowStrength = Mathf.Lerp(from.shadowStrength, to.shadowStrength, blend);
                
                if (gaiaSettings.EnableFogControl && RenderSettings.fog)
                {
                    RenderSettings.fogColor = Color.Lerp(from.fogColor, to.fogColor, blend);
                    RenderSettings.fogDensity = Mathf.Lerp(from.fogDensity, to.fogDensity, blend);
                }
            }).Run();
            #endregion

            #region Spawning

            Entities.ForEach((ref GaiaSpawnBiome biome) =>
            {
                switch (timeOfDay)
                {
                    case < 6f:
                        if(biome.Daybreak.IsSatisfied)return;
                        Debug.Log("Daybreak Spawn");
                        biome.Daybreak.IsSatisfied = true;
                        biome.Midday.IsSatisfied = false;
                        biome.Sunset.IsSatisfied = false;
                        biome.Night.IsSatisfied = false;
                        break;
                    case < 12f:
                        if(biome.Midday.IsSatisfied)return;
                        Debug.Log("Midday Spawn");
                        biome.Midday.IsSatisfied = true;
                        biome.Daybreak.IsSatisfied = false;
                        biome.Sunset.IsSatisfied = false;
                        biome.Night.IsSatisfied = false;
                        break;
                    case < 18f:
                        if(biome.Sunset.IsSatisfied)return;
                        Debug.Log("Sunset Spawn");
                        biome.Sunset.IsSatisfied = true;
                        biome.Daybreak.IsSatisfied = false;
                        biome.Midday.IsSatisfied = false;
                        biome.Night.IsSatisfied = false;
                        break;
                    default:
                        if(biome.Night.IsSatisfied)return;
                        Debug.Log("Night Spawn");
                        biome.Night.IsSatisfied = true;
                        biome.Midday.IsSatisfied = false;
                        biome.Sunset.IsSatisfied = false;
                        biome.Daybreak.IsSatisfied = false;
                        break;
                }
            }).Schedule();

            #endregion

        }
        
    }
}