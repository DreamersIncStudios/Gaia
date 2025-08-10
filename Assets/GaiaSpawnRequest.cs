using DreamersIncStudio.GAIACollective;
using Systems.Bestiary;
using Unity.Entities;
using UnityEngine;

public partial class GaiaSpawnRequest: SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref GaiaSpawnBiome b)=>{
            if(b.SpawnRequests.IsEmpty) return;
            
            foreach (var request in b.SpawnRequests)
            {
                for (var i = 0; i < request.Qty; i++)
                {
                    new CharacterBuilder("spawn", out var entity)
                        .WithActiveHour(request.ActiveHours, request.HomeBiomeID)
                        .AtLevel(request.LevelRange, request.PlayerLevel)
                        .Build();
                }
            }
            b.SpawnRequests.Clear();
        }).Run();
    }
}
