using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceKeepingForce : SkillCommon
{
    public GameObject hitEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "평화유지군";
        skillDesc = "아군을 진정시켜 모든 아군의 방어력을 2턴간 50% 상승시킵니다.";
        skillIconName = "8";
    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team)
            {
                target.GetBuff(target, BuffCommon.BUFFTYPE.INC_DEF, 2, 0.5f, 0);
                GameObject hitEffectInstance = Instantiate(hitEffect);
                hitEffectInstance.transform.position = target.transform.position + hitEffect.transform.localPosition;
                hitEffectInstance.SetActive(true);
            }
        }
    }
}
