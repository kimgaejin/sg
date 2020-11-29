using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTalent : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "전투의 재능";
        skillDesc = "[패시브]매 타격마다 공격력이 5%씩 증가합니다.\n(최대 중첩 10회)";
        skillIconName = "battleTalent";
        isPassive = true;

    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        base.Activate();

    }

    public override void Passive(BattleManager.BATTLETIME tag, ChampionInfo target)
    {
        if (tag == BattleManager.BATTLETIME.ATTACKING)
        {
            if (target == start)
            {
                start.GetBuff(start, skillIconName, BuffCommon.BUFFTYPE.INC_ATK, 1, 0.05f, true, 1, true, 0);
            }
        }
    }
}
