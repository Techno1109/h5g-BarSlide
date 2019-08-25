using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Tiny.HitBox2D;
using RigidBodySystems;
using Unity.Collections;

public class OKCheckSystem : ComponentSystem
{
    EntityQueryDesc GlassDesc;
    EntityQuery GlassQuery;

    EntityQueryDesc CheckLineDesc;
    EntityQuery CheckLineQuery;

    EntityQueryDesc OkAreaDesc;
    EntityQuery OkAreaQuery;

    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        GlassDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GlassTag), typeof(RigidBody), typeof(GlassComponent),typeof(Translation) },
        };

        CheckLineDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] {typeof(CheckLineTag) },
        };

        OkAreaDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(OKAreaTag) },
        };

        GlassQuery = GetEntityQuery(GlassDesc);
        CheckLineQuery = GetEntityQuery(CheckLineDesc);
        OkAreaQuery = GetEntityQuery(OkAreaDesc);
    }

    protected override void OnUpdate()
    {
        NativeArray<Entity> OkArea = OkAreaQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> CheckLine = CheckLineQuery.ToEntityArray(Allocator.TempJob);

        Entities.With(GlassQuery).ForEach((Entity ThisEntity,ref GlassComponent GlassData,ref RigidBody RigidData,ref Translation Trans)=>
        {
            if(!GlassData.Active||GlassData.EndTag)
            {
                return;
            }

            if(RigidData.Velocity.x!=0)
            {
                return;
            }

            var HitEntity = EntityManager.GetBuffer<HitBoxOverlap>(CheckLine[0]);

            bool HitResultFlag = false;


            for (int k = 0; k < HitEntity.Length; k++)
            {
                //サーチ対象のEntityが含まれていた場合、当たっていることになる。
                if (OkArea[0]== HitEntity[k].otherEntity)
                {
                    //当たった場合、一度フラグをTrueにしてBreak；
                    HitResultFlag = true;
                    break;
                }
            }

            if(HitResultFlag==true)
            {
                //成功
                Trans.Value.x = 0;
                GlassData.Active = false;
            }
            else
            {
                //失敗
                GlassData.EndTag = true;
            }
        });

        OkArea.Dispose();
        CheckLine.Dispose();
    }
}
