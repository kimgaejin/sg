using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTalent : SkillCommon
{
    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "전투의 재능";
        skillDesc = "1회 타격시마다 공격력이 5%씩 증가합니다.\n(최대 중첩 10회)";
        skillIconName = "1";
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
            Debug.Log("패시브 외부");

            if (target == start)
            {
                Debug.Log("패시브 발동");
                start.GetBuff(start, BuffCommon.BUFFTYPE.INC_ATK, 99, 0.05f, 0);
            }
        }
    }
}
