using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : SkillCommon
{
    // battle zone의 적에게 시전자의 공격력만큼의 데미지를 준다.

    public override void Do()
    {
        base.Do();

        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team != start.team && target.location == 1)
            {
                Debug.Log(start.name + "의" + "파이어볼! " + target.name + "에게 피해를 입힙니다!");
                target.Attacked(start.atk);
                break;
            }
        }
    }
}
