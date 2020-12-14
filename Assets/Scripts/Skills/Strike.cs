using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strike : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "공격";
        skillDesc = "배틀 존의 상대를 타격해 공격력의 100% 피해를 입힙니다.";
        skillIconName = "1";
        skillIndex = 200;
    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 0)
            {
                start.Attack(target, (int)(start.GetDamageValue() * 1.0f), 0);
                break;
            }
        }
    }
}
