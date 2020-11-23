﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeManner : SkillCommon
{
    public GameObject buffEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "예의 주입";
        skillDesc = "망치를 휘둘러 전방의 적에게 80%의 피해를 입힙니다. 2턴간 자신의 방어력이 20% 증가합니다.";
        skillIconName = "7";
    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 0)
            {
                int coefDamage = (int)(start.GetDamageValue() * 0.8f);
                target.Attacked(coefDamage, 0);
                start.GetBuff(start, BuffCommon.BUFFTYPE.INC_DEF, 2, 0.2f, 0);
                GameObject buffEffectInstance = Instantiate(buffEffect);
                buffEffectInstance.transform.position = target.transform.position + buffEffect.transform.localPosition;
                buffEffectInstance.SetActive(true);
                break;
            }
        }
    }
}
