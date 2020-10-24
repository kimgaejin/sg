using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forgiving : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "면죄부";
        skillDesc = "신의 축복을 빌어 모든 아군의 체력을 -";
        skillIconName = "9";
    }

    public override void Do()
    {
        base.Do();

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team)
            {
                target.GetBuff(target, BuffCommon.BUFFTYPE.INC_ATK, 3, 0.21f);
            }
        }
    }
}
