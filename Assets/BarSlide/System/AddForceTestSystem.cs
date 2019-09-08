using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Collections;
using Unity.Tiny.UIControls;
using RigidBodySystems;
using Unity.Tiny.Input;

[UpdateBefore(typeof(RigidBodyGroup))]
public class AddForceTestSystem : ComponentSystem
{
    EntityQueryDesc GlassDesc;

    EntityQuery GlassQuery;

    EntityQueryDesc GageDesc;

    EntityQuery GageQuery;

    EntityQueryDesc PointerDesc;
    EntityQuery PointerQuery;

    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        GlassDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GlassTag), typeof(RigidBody),typeof(GlassComponent) },
        };

        GageDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GageComponent)},
        };

        GlassQuery = GetEntityQuery(GlassDesc);
        GageQuery = GetEntityQuery(GageDesc);
    }
    protected override void OnUpdate()
    {
        NativeArray<GageComponent> GageDatas = GageQuery.ToComponentDataArray<GageComponent>(Allocator.TempJob);

       var Input =EntityManager.World.GetExistingSystem<InputSystem>();

        if (GageDatas.Length<=0)
        {
            GageDatas.Dispose();

            return;
        }

        bool InputButton = false;

        InputButton = Input.GetMouseButtonDown(0);

        Entities.With(GlassQuery).ForEach((ref RigidBody Rigid, ref GlassComponent GlassData) =>
        {
            if(GlassData.Active)
            {
                return;
            }

            float DltTime = World.TinyEnvironment().fixedFrameDeltaTime;
            if (GlassData.charging)
            {
                if (InputButton)
                {
                    GlassData.charging = false;
                    GlassData.Active = true;
                    Rigid.Velocity.x += GlassData.NowValue*2.4f;
                }
                else
                {

                    GlassData.NowValue += GlassData.AddSpeed * DltTime*3;

                    if (GlassData.NowValue >= GlassData.MaxValue)
                    {
                        GlassData.NowValue = GlassData.MaxValue;
                        GlassData.AddSpeed *= -1;
                    }
                    else if (0 >= GlassData.NowValue)
                    {
                        GlassData.NowValue = 0;
                        GlassData.AddSpeed *= -1;
                    }

                }
            }
            else
            {
                if (InputButton)
                {
                    GlassData.charging = true;
                    GlassData.NowValue = 0;
                    GlassData.AddSpeed = GlassData.AddSpeed < 0 ? GlassData.AddSpeed * -1 : GlassData.AddSpeed;
                }
            }

            var Tmp = GageDatas[0];
            Tmp.NowValue = GlassData.NowValue;
            GageDatas[0] = Tmp;
        });

        GageQuery.CopyFromComponentDataArray(GageDatas);

        GageDatas.Dispose();
    }
}
