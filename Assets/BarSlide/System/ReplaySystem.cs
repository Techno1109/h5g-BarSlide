using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.UILayout;
using Unity.Tiny.Core2D;
using Unity.Collections;
using Unity.Tiny.UIControls;

[UpdateAfter(typeof(OKCheckSystem))]
public class ReplaySystem : ComponentSystem
{
    EntityQueryDesc ReplayButtonDesc;
    EntityQuery ReplayButtonQuery;

    EntityQueryDesc ResultDesc;
    EntityQuery ResultQuery;

    EntityQueryDesc GlassDesc;
    EntityQuery GlassQuery;

    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        ReplayButtonDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(ReplayButton), typeof(PointerInteraction) },
        };


        ResultDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(ResultTag) ,typeof(RectTransform)},
        };

        GlassDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GlassTag), typeof(GlassComponent), typeof(Translation) },
        };



        /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
        //作成したクエリの結果を取得します。
        ReplayButtonQuery = GetEntityQuery(ReplayButtonDesc);
        GlassQuery = GetEntityQuery(GlassDesc);
        ResultQuery = GetEntityQuery(ResultDesc);
    }

    protected override void OnUpdate()
    {
        var GameStats = World.TinyEnvironment();
        var Config = GameStats.GetConfigData<GameState>();

        if (!Config.End)
        {
            return;
        }

        NativeArray<PointerInteraction> ReplayButtons = ReplayButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);

        if (ReplayButtons.Length <= 0)
        {
            ReplayButtons.Dispose();
            return;
        }

        bool InputButton = false;

        for (int i = 0; i < ReplayButtons.Length; i++)
        {
            InputButton = InputButton || ReplayButtons[i].clicked;
        }

        if (InputButton)
        {
            //グラスをリセット
            Entities.With(GlassQuery).ForEach((ref GlassComponent GlassData, ref Translation Trans) =>
            {
                Trans.Value.x = 0;
                GlassData.Active = false;
                GlassData.EndTag = false;
            });

            //ゲームコンフィグをリセット
            Config.Score = 0;
            Config.Lv = 0;
            Config.End = false;
            GameStats.SetConfigData(Config);


            //リザルトウィンドウを無効化
            Entities.With(ResultQuery).ForEach((ref RectTransform RecTrans) =>
            {
                RecTrans.anchoredPosition.y = 600;
            });

            //成功エリアの再設置
            var PutSystem = World.GetExistingSystem<OKCheckSystem>();
            PutSystem.RandomSet();

            //スコアテキストの更新
            var ScoreDraw = World.GetExistingSystem<ScoreDrawSystem>();
            ScoreDraw.RefrehScoreText();
        }
    }
}
