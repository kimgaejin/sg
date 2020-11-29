using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleBoost : SkillCommon
{
    public GameObject hitEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "정복욕";
        skillDesc = "고함을 지르고 아군을 고무시킵니다. 모든 아군의 공격력을 3턴간 25% 증가시킵니다.";
        skillIconName = "3";
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
            if (target.team == start.team && target.isDead == false)
            {
                target.GetBuff(target, skillIconName, BuffCommon.BUFFTYPE.INC_ATK, 3, 0.25f, false, 1, false, 0);
                GameObject hitEffectInstance = Instantiate(hitEffect);
                hitEffectInstance.transform.position = target.transform.position + hitEffect.transform.localPosition;
                hitEffectInstance.SetActive(true);
            }
        }
    }
}
