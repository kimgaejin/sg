using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleBoost : SkillCommon
{
    // 사기진작
    // 아군 전체에게 3턴간 공격력 25% 증가 버프를 건다

    protected override void InitSelf()
    {
        base.InitSelf();
        skillDesc = "[사기진작]\n아군 전체의 공격력을 3턴간 증가시킨다";
    }

    public override void Do()
    {
        base.Do();

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team)
            {
                Debug.Log(start.name + "의" + "사기진작 " + target.name + "의 공격력이 증가합니다.");
                target.GetBuff(target, BuffCommon.BUFFTYPE.INC_ATK, 3, 0.25f);
            }
        }
    }
}
