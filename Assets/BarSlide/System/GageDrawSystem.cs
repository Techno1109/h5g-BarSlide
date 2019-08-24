using Unity.Entities;
using Unity.Tiny.UILayout;

public class GageDrawSystem : ComponentSystem
{
    EntityQueryDesc GageQueryDesc;
    EntityQuery GageQuery;



    protected override void OnCreate()
    {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

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
            //まず1パーセントの比率を出す
            float Value = GageData.FirstLength / GageData.MaxValue;
            //現在の値から割合を算出
            float Length = Value * GageData.NowValue;

            //最終的な場所を算出
            //まず、0の状態の場所を算出
            float PosX = GageData.FistPos - (Value * (GageData.MaxValue / 2));

            //足された分のずらす場所を算出
            PosX += (Value * GageData.NowValue) / 2;

            //RectTransFormに値を挿入

            //長さをまず設定
            RectTransData.sizeDelta.x = (Value * GageData.NowValue);

            //X座標を設定
            RectTransData.anchoredPosition.x = PosX;
        });
    }
}
