using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Collections;
using Unity.Tiny.UIControls;
using RigidBodySystems;

[UpdateBefore(typeof(RigidBodyGroup))]
public class AddForceTestSystem : ComponentSystem
{
    EntityQueryDesc TestButtonDesc;

    EntityQuery TestButtonQuery;

    EntityQueryDesc GlassDesc;

    EntityQuery GlassQuery;

    EntityQueryDesc GageDesc;

    EntityQuery GageQuery;

    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        TestButtonDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(TestButtonTag), typeof(PointerInteraction) },
        };

        GlassDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GlassTag), typeof(RigidBody),typeof(GlassComponent) },
        };

        GageDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GageComponent)},
        };

        TestButtonQuery = GetEntityQuery(TestButtonDesc);
        GlassQuery = GetEntityQuery(GlassDesc);
        GageQuery = GetEntityQuery(GageDesc);
    }
    protected override void OnUpdate()
    {
        NativeArray<PointerInteraction> MoveButtons = TestButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);
        NativeArray<GageComponent> GageDatas = GageQuery.ToComponentDataArray<GageComponent>(Allocator.TempJob);

        if (MoveButtons.Length <= 0 || GageDatas.Length<=0)
        {
            MoveButtons.Dispose();
            GageDatas.Dispose();
            return;
        }

        bool InputButton = false;

        for (int i = 0; i < MoveButtons.Length; i++)
        {
            InputButton = InputButton || MoveButtons[i].clicked;
        }


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
                    Rigid.Velocity.x += GlassData.NowValue;
                }
                else
                {

                    GlassData.NowValue += GlassData.AddSpeed * DltTime;

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

        MoveButtons.Dispose();
        GageDatas.Dispose();
    }
}
