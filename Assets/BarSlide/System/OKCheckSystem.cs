using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.HitBox2D;
using RigidBodySystems;
using Unity.Collections;

[UpdateAfter(typeof(AddForceTestSystem))]
public class OKCheckSystem : ComponentSystem
{
    EntityQueryDesc GlassDesc;
    EntityQuery GlassQuery;

    EntityQueryDesc CheckLineDesc;
    EntityQuery CheckLineQuery;

    EntityQueryDesc OkAreaDesc;
    EntityQuery OkAreaQuery;


    EntityQueryDesc ResultDesc;
    EntityQuery ResultQuery;

    Unity.Mathematics.Random RandomData;

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


        ResultDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(ResultTag) , typeof(Disabled)},
        };

        GlassQuery = GetEntityQuery(GlassDesc);
        CheckLineQuery = GetEntityQuery(CheckLineDesc);
        OkAreaQuery = GetEntityQuery(OkAreaDesc);
        ResultQuery = GetEntityQuery(ResultDesc);

        RandomData = new Unity.Mathematics.Random(3158456);
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
                ScoreUp(Trans.Value.x);
                RandomSet();
            }
            else
            {
                //失敗
                GlassData.EndTag = true;
                //Resultを有効化
                Entities.With(ResultQuery).ForEach((Entity ResultEntity) =>
                {
                    EntityManager.SetEnabled(ResultEntity,true);
                });
                var GameStats = World.TinyEnvironment();
                var Config = GameStats.GetConfigData<GameState>();
                Config.End = true;
                GameStats.SetConfigData(Config);
            }
        });

        OkArea.Dispose();
        CheckLine.Dispose();
    }

    private void ScoreUp(float Pos)
    {
        var GameStats = World.TinyEnvironment();
        var Config = GameStats.GetConfigData<GameState>();

        RandomData.InitState((uint)(Config.Score * RandomData.NextInt(0, 1000) * Pos));

        Config.Score += 1;
        if (Config.Score%3 == 0)
        {
            if(Config.Lv<0.4f)
            {
                Config.Lv += 0.1f;
            }
        }
        GameStats.SetConfigData(Config);
    }

    public void RandomSet()
    {
        var GameStats = World.TinyEnvironment();
        var Config = GameStats.GetConfigData<GameState>();
        float Base = 1 - Config.Lv;
        Entities.With(OkAreaQuery).ForEach((Entity ThisEntity,ref Translation Trans ,ref NonUniformScale Scale ,ref OKAreaTag AreaData) =>
        {
            float BaseScale = RandomData.NextFloat(AreaData.MinSize * Base, AreaData.MaxSize * Base);
            float BasePos = BaseScale / 2;
            Scale.Value.x = BaseScale;
            Trans.Value.x = RandomData.NextFloat(AreaData.MinPos+BasePos,AreaData.MaxPos-BasePos);
        });
    }
}
