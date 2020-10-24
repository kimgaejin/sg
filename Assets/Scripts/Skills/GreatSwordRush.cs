using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordRush : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "광기";
        skillDesc = "공중으로 도약한 다음 대검으로 강하게 내리쳐 전방의 적에게 250%의 피해를 입힙니다.";
        skillIconName = "1";
    }

    public override IEnumerator Do()
    {
        base.Do();
        animator.Play("Skill1");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        animator.Play("Idle");

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 1)
            {
                int coefDamage = (int)(start.GetDamageValue() * 2.5f);
                target.Attacked(coefDamage);
                break;
            }
        }
    }
}
