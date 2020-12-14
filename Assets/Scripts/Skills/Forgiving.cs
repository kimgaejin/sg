﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forgiving : SkillCommon
{
    public GameObject buffEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "면죄부";
        skillDesc = "신의 축복을 빌어 모든 아군의 체력을 해당 아군의 최대체력 25%만큼 회복합니다.";
        skillIconName = "9";
        skillIndex = 303;

    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team && target.isDead == false)
            {
                target.GetHeal((int)(target.maxHp/4.0f), 0);
                GameObject buffEffectInstance = Instantiate(buffEffect);
                buffEffectInstance.transform.position = target.transform.position + buffEffect.transform.localPosition;
                buffEffectInstance.SetActive(true);
            }
        }
    }
}
