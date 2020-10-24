using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleBoost : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "정복욕";
        skillDesc = "고함을 지르고 아군을 고무시킵니다. 모든 아군의 공격력을 3턴간 25% 증가시킵니다.";
        skillIconName = "3";
    }

    public override IEnumerator Do()
    {
        base.Do();
        animator.Play("Skill2");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        animator.Play("Idle");

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team)
            {
                target.GetBuff(target, BuffCommon.BUFFTYPE.INC_ATK, 3, 0.25f);
            }
        }
    }
}
