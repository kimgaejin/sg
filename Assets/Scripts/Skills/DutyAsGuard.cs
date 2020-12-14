using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DutyAsGuard : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "경비병의 의무";
        skillDesc = "별로 싸우고싶은 마음은 없지만, 임무를 수행하는 척이라도 해야죠.\n배틀 존의 상대를 타격해 공격력의 200% 피해를 입힙니다.";
        skillIconName = "dutyGuard";
        skillCooltime = 3;
        skillIndex = 305;

    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        base.Activate();

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 0)
            {
                start.Attack(target, (int)(start.GetDamageValue() * 2.0f), 0);
            }
        }
    }
}
