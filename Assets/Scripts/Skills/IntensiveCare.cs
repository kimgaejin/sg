using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensiveCare : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "집중 치료";
        skillDesc = "체력 비율이 가장 낮은 팀원을 공격력의 150%만큼 회복시킵니다.\n쿨타임 2턴";
        skillIconName = "임시지원";
        skillCooltime = 2;
    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        ChampionInfo target = null;
        foreach (ChampionInfo candidate in battleManager.championList)
        {
            if (candidate.team == start.team && candidate.location != 0 && candidate.isDead == false)
            {
                if (target == null) target = candidate;
                else
                {
                    if (candidate.hp / candidate.maxHp < target.hp / target.maxHp)
                    {
                        target = candidate;
                    }
                }
            }
        }
        target.GetHeal((int)(start.GetAtk()*1.5f), 0);

    }
}
