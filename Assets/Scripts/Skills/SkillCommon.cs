using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCommon : MonoBehaviour
{
    public Animator animator;
    protected BattleManager battleManager;
    public ChampionInfo start;
    protected string skillName = "";
    protected string skillIconName = "";
    protected string skillDesc = "";

    private void Awake()
    {
        start = transform.GetComponent<ChampionInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        InitSelf();
    }

    private void Start()
    {
        transform.GetComponent<ChampionInfo>().AddSkill(this);
    }

    protected virtual void InitSelf()
    {
        
    }

    public virtual IEnumerator GoToBattleZone()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team && target.location == 1 && target != start)
            {
                int temp = target.location;
                target.location = start.location;
                start.location = temp;

                yield return StartCoroutine(battleManager.SwapPosition(start, target));
            }
        }
    }

    public virtual IEnumerator Do()
    {
        yield return null;
    }

    public virtual string GetSkillDescription() { return skillDesc; }
    public string GetSkillName() { return skillName; }
    public string GetSkillIconName() { return skillIconName; }
}
