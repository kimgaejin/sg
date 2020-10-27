using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceKeepingForce : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "평화유지군";
        skillDesc = "아군을 진정시켜 모든 아군의 방어력을 2턴간 50% 상승시킵니다.";
        skillIconName = "8";
    }

    public override IEnumerator Do()
    {
        base.Do();
        animator.Play("Attack");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team)
            {
                target.GetBuff(target, BuffCommon.BUFFTYPE.INC_DEF, 2, 0.5f);
            }
        }
    }
}
