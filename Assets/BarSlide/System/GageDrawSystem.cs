using Unity.Entities;
using Unity.Tiny.UILayout;

public class GageDrawSystem : ComponentSystem
{
    EntityQueryDesc GageQueryDesc;
    EntityQuery GageQuery;



    protected override void OnCreate()
    {
        /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

        GageQueryDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GageComponent),typeof(RectTransform)},
        };

        GageQuery = GetEntityQuery(GageQueryDesc);
    }

    protected override void OnUpdate()
    {
        Entities.With(GageQuery).ForEach((ref GageComponent GageData,ref RectTransform RectTransData) =>
        {
            //�܂�1�p�[�Z���g�̔䗦���o��
            float Value = GageData.FirstLength / GageData.MaxValue;
            //���݂̒l���犄�����Z�o
            float Length = Value * GageData.NowValue;

            //�ŏI�I�ȏꏊ���Z�o
            //�܂��A0�̏�Ԃ̏ꏊ���Z�o
            float PosX = GageData.FistPos - (Value * (GageData.MaxValue / 2));

            //�����ꂽ���̂��炷�ꏊ���Z�o
            PosX += (Value * GageData.NowValue) / 2;

            //RectTransForm�ɒl��}��

            //�������܂��ݒ�
            RectTransData.sizeDelta.x = (Value * GageData.NowValue);

            //X���W��ݒ�
            RectTransData.anchoredPosition.x = PosX;
        });
    }
}
