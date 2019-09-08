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
        /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

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



        /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
        //�쐬�����N�G���̌��ʂ��擾���܂��B
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
            //�O���X�����Z�b�g
            Entities.With(GlassQuery).ForEach((ref GlassComponent GlassData, ref Translation Trans) =>
            {
                Trans.Value.x = 0;
                GlassData.Active = false;
                GlassData.EndTag = false;
            });

            //�Q�[���R���t�B�O�����Z�b�g
            Config.Score = 0;
            Config.Lv = 0;
            Config.End = false;
            GameStats.SetConfigData(Config);


            //���U���g�E�B���h�E�𖳌���
            Entities.With(ResultQuery).ForEach((ref RectTransform RecTrans) =>
            {
                RecTrans.anchoredPosition.y = 600;
            });

            //�����G���A�̍Đݒu
            var PutSystem = World.GetExistingSystem<OKCheckSystem>();
            PutSystem.RandomSet();

            //�X�R�A�e�L�X�g�̍X�V
            var ScoreDraw = World.GetExistingSystem<ScoreDrawSystem>();
            ScoreDraw.RefrehScoreText();
        }
    }
}
