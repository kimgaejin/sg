using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeManner : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "예의 주입";
        skillDesc = "망치를 휘둘러 전방의 적에게 80%의 피해를 입힙니다. 2턴간 자신의 방어력이 20% 증가합니다.";
        skillIconName = "7";
    }

    public override IEnumerator Do()
    {
        base.Do();
        animator.Play("Attack");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 0)
            {
                int coefDamage = (int)(start.GetDamageValue() * 0.8f);
                target.Attacked(coefDamage, 0);
                start.GetBuff(start, BuffCommon.BUFFTYPE.INC_DEF, 2, 0.2f, 0);
                break;
            }
        }
    }
}
