using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordRush : SkillCommon
{
    public GameObject hitEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "광기";
        skillDesc = "강력한 한방 공격으로 전방의 적에게 200%의 피해를 입힙니다.";
        skillIconName = "beheading";
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
                start.Attack(target, (int)( start.GetDamageValue() * 2.5f ), 0);

				GameObject hitEffectInstance = Instantiate(hitEffect);
                hitEffectInstance.transform.position = target.transform.position + hitEffect.transform.localPosition;
                hitEffectInstance.SetActive(true);
                break;
            }
        }
    }
}
