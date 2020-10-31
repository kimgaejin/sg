﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : SkillCommon
{
    // battle zone의 적에게 시전자의 공격력만큼의 데미지를 준다.
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "화염구";
        skillDesc = "마법으로 소환한 잿불 응집체를 날려 전방의 적에게 150% 피해량을 줍니다.";
        skillIconName = "5";
    }

    public override IEnumerator Do()
    {
        base.Do();
        animator.Play("Attack");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 1)
            {
                int coefDamage = (int)(start.GetDamageValue() * 1.5f);
                target.Attacked(coefDamage, 0);
                break;
            }
        }
    }
}
