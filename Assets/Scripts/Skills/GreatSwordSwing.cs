using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordSwing : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillDesc = "상대 진영으로 들어가 전원을 대검으로 가릅니다!";
    }

    public override void Do()
    {
        base.Do();

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team)
            {
                Debug.Log(start.name + "의" + "대검 휘두르기! " + target.name + "에게 피해를 입힙니다!");
                target.Attacked(start.GetDamageValue());
            }
        }
    }
}
