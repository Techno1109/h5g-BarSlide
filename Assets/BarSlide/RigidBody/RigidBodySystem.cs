using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Tiny.Core;
using Unity.Collections;

namespace RigidBodySystems
{

    public class RigidBodySystem : ComponentSystem
    {
        const float Gravity = 0.5f;
        EntityQueryDesc RigidBodyQueryDesc;
        EntityQuery RigidBodyQuery;

        protected override void OnCreate()
        {
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            RigidBodyQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(RigidBody),typeof(Translation) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。

            RigidBodyQuery = GetEntityQuery(RigidBodyQueryDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<RigidBody> RigidBodyArray = RigidBodyQuery.ToComponentDataArray<RigidBody>(Allocator.TempJob);
            NativeArray<Translation> TranslationArray = RigidBodyQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            if (RigidBodyArray.Length <= 0 || TranslationArray.Length <= 0 )
            {
                TranslationArray.Dispose();
                RigidBodyArray.Dispose();
                return;
            }

            float DeltaTime = World.TinyEnvironment().fixedFrameDeltaTime;

            for (int EntityNum = 0; EntityNum<RigidBodyArray.Length; EntityNum++)
            {
                RigidBody NowRigidBody = RigidBodyArray[EntityNum];
                Translation NowTranslation = TranslationArray[EntityNum];

                if (!NowRigidBody.IsActive)
                {
                    continue;
                }

                if(NowRigidBody.UseGravity)
                {
                    NowRigidBody.Velocity.y -= Gravity * DeltaTime;
                    if(NowRigidBody.Velocity.y<-2f)
                    {
                        NowRigidBody.Velocity.y = -2f;
                    }
                }

                if(NowRigidBody.ActiveVec.x)
                {
                    NowTranslation.Value.x += NowRigidBody.Velocity.x;
                }

                if (NowRigidBody.ActiveVec.y)
                {
                    NowTranslation.Value.y += NowRigidBody.Velocity.y;
                }

                if (NowRigidBody.ActiveVec.z)
                {
                    NowTranslation.Value.z += NowRigidBody.Velocity.z;
                }

                RigidBodyArray[EntityNum] = NowRigidBody;
                TranslationArray[EntityNum] = NowTranslation;
            }

            //あくまでもToDataArrayで取得できるArrayはキャッシュの様なものなので、本Entityに書き込む必要があります。
            RigidBodyQuery.CopyFromComponentDataArray(RigidBodyArray);
            RigidBodyQuery.CopyFromComponentDataArray(TranslationArray);

            //NativeArrayはUnsafeな動的確保のため、自動で解放されません。
            //ちゃんと開放しましょう。
            TranslationArray.Dispose();
            RigidBodyArray.Dispose();
        }
    }
}