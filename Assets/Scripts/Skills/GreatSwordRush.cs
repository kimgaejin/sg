using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordRush : SkillCommon
{
    public GameObject hitEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "빈틈!";
        skillDesc = "상대방의 헛점을 노려 공격합니다! 배틀 존의 상대에게 150% 피해를 입힙니다.";
        skillIconName = "beheading";
        skillCooltime = 1;
        skillIndex = 301;
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
                start.Attack(target, (int)( start.GetDamageValue() * 1.5f ), 0);

				GameObject hitEffectInstance = Instantiate(hitEffect);
                hitEffectInstance.transform.position = target.transform.position + hitEffect.transform.localPosition;
                hitEffectInstance.SetActive(true);
                break;
            }
        }
    }
}
