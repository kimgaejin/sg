using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCommon : MonoBehaviour
{
    protected BattleManager battleManager;
    public ChampionInfo start;

    private void Awake()
    {
        start = transform.GetComponent<ChampionInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    public virtual void GoToBattleZone()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team && target.location == 1)
            {
                int temp = target.location;
                target.location = start.location;
                start.location = temp;

                battleManager.SwapAnchoredPosition(start, target);
            }
        }
    }

    public virtual void Do()
    {

    }
}
