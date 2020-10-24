using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordSwing : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "무모함";
        skillDesc = "적군 한복판으로 들어가 대검을 휘두릅니다. 적 모두에게 150%의 피해를 입힙니다.";
        skillIconName = "2";
    }

    public override IEnumerator Do()
    {
        base.Do();
        animator.Play("Attack");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        animator.Play("Idle");

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team)
            {
                target.Attacked(start.GetDamageValue());
            }
        }
    }
}
