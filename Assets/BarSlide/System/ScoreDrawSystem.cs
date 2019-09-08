using Unity.Entities;
using Unity.Tiny.Text;
using Unity.Tiny.Core;

public class ScoreDrawSystem : ComponentSystem
{

    EntityQueryDesc ScoreDesc;
    EntityQuery ScoreQuery;

    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        ScoreDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(ScoreTag), typeof(TextString) },
        };


        /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
        //作成したクエリの結果を取得します。
        ScoreQuery = GetEntityQuery(ScoreDesc);
    }

    protected override void OnUpdate()
    {
    }

    public void RefrehScoreText()
    {
        Entities.With(ScoreQuery).ForEach((Entity TargetEntity) =>
        {
            EntityManager.SetBufferFromString<TextString>(TargetEntity, World.TinyEnvironment().GetConfigData<GameState>().Score.ToString());
        });
    }
}

