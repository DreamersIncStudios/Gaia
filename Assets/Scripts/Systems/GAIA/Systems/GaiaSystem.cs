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
            var gaiaSettings = SystemAPI.GetSingleton<GaiaSettings>();
            gaiaTime.ValueRW.UpdateTime(SystemAPI.Time.DeltaTime);
            Entities.WithoutBurst().ForEach((Light light) =>
            {
                var rotationPivot = light.transform;
                float timePercent = gaiaTime.ValueRO.TimeOfDay/24f;
                float xRotation = (timePercent * 360f) - 90f;
                if (rotationPivot.localRotation.eulerAngles.x != xRotation || rotationPivot.localRotation.eulerAngles.y != 0)
                {
                    rotationPivot.localRotation = Quaternion.Euler(new Vector3(xRotation, 0, 0));
                }
                
                TimeSettings from, to;
                float blend;
                switch (gaiaTime.ValueRO.TimeOfDay)
                {
                    case < 6f:
                        from = gaiaSettings.Night; to = gaiaSettings.Daybreak; blend = gaiaTime.ValueRO.TimeOfDay / 6f;
                        break;
                    case < 12f:
                        from = gaiaSettings.Daybreak; to = gaiaSettings.Midday; blend = (gaiaTime.ValueRO.TimeOfDay- 6f) / 6f;
                        break;
                    case < 18f:
                        from = gaiaSettings.Midday; to = gaiaSettings.Sunset; blend = (gaiaTime.ValueRO.TimeOfDay - 12f) / 6f;
                        break;
                    default:
                        from = gaiaSettings.Sunset; to = gaiaSettings.Night; blend = (gaiaTime.ValueRO.TimeOfDay - 18f) / 6f;
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
            
       
        }
        
    }
}