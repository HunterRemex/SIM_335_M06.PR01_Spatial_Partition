using Unity.Entities;
using Unity.Mathematics;


[UpdateBefore(typeof(FriendlyUpdateGrid))]
public partial class EnemyClosestMatSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has

        
        
        
        Entities.ForEach(( ref EnemyMaterialColor eMatColor, in Enemy en) => {

            // if (en.soldierData.bIsClosest)
            {
                eMatColor.Value = new float4(1, 1, 1, 1);
            }
            
        }).WithBurst().ScheduleParallel();
    }
}