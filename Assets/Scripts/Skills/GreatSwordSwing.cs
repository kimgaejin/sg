using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordSwing : SkillCommon
{
    public GameObject hitEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "무모함";
        skillDesc = "적군 한복판으로 들어가 대검을 휘두릅니다. 적 모두에게 150%의 피해를 입힙니다.\n쿨타임 2턴";
        skillIconName = "임시공격";
        skillCooltime = 2;
        skillIndex = 302;

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
            if (target.team != start.team)
            {
                start.Attack(target, start.GetDamageValue(), 0);
                GameObject hitEffectInstance = Instantiate(hitEffect);
                hitEffectInstance.transform.position = target.transform.position + hitEffect.transform.localPosition;
                hitEffectInstance.SetActive(true);
            }
        }
    }
}
