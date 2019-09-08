using Unity.Entities;
using Unity.Tiny.Text;
using Unity.Tiny.Core;

public class ScoreDrawSystem : ComponentSystem
{

    EntityQueryDesc ScoreDesc;
    EntityQuery ScoreQuery;

    protected override void OnCreate()
    {
        /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

        ScoreDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(ScoreTag), typeof(TextString) },
        };


        /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
        //�쐬�����N�G���̌��ʂ��擾���܂��B
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

