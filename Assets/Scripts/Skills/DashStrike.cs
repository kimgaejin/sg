using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashStrike : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "도약 공격";
        skillDesc = "휴식 존의 상대를 하나 타격해 공격력의 80% 피해를 입힙니다.";
        skillIconName = "임시공격";
        skillIndex = 201;

    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        List<ChampionInfo> targets = new List<ChampionInfo>();
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location != 0 && target.isDead == false)
            {
                targets.Add(target);
            }
        }

        int rand = Random.Range(0, targets.Count);
        start.Attack(targets[rand], (int)(start.GetDamageValue() * 0.8f), 0);
    }
}
