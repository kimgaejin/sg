using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordRush : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillDesc = "전방의 적을 가릅니다!";
    }

    public override void Do()
    {
        base.Do();

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 1)
            {
                Debug.Log(start.name + "의 대검 파쇄! " + target.name + "에게 피해를 입힙니다!");
                target.Attacked(start.atk);
                break;
            }
        }
    }
}
