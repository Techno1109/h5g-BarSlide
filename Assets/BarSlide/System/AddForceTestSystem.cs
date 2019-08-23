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

    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        TestButtonDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(TestButtonTag), typeof(PointerInteraction) },
        };

        GlassDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GlassTag), typeof(RigidBody) },
        };

        TestButtonQuery = GetEntityQuery(TestButtonDesc);
        GlassQuery = GetEntityQuery(GlassDesc);
    }
    protected override void OnUpdate()
    {
        NativeArray<PointerInteraction> MoveButtons = TestButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);

        if (MoveButtons.Length <= 0)
        {
            MoveButtons.Dispose();
            return;
        }

        bool InputButton = false;

        for (int i = 0; i < MoveButtons.Length; i++)
        {
            InputButton = InputButton || MoveButtons[i].clicked;
        }

        Entities.With(GlassQuery).ForEach((ref RigidBody Rigid) =>
        {
            if (InputButton == true && Rigid.IsActive)
            {
                Rigid.Velocity.x += 3f;
            }

        });

    }
}
