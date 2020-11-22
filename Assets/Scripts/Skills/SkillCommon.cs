﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SkillCommon : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public PlayableAsset playableAsset;
    protected BattleManager battleManager;
    public ChampionInfo start;
    protected string skillName = "";
    protected string skillIconName = "";
    protected string skillDesc = "";
    protected int skillCooltimeRemain = 0;
    protected int skillCooltime = 0;
    protected int cameraLocation;

    private void Awake()
    {
        start = transform.GetComponent<ChampionInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        InitSelf();
    }

    private void Start()
    {
        transform.GetComponent<ChampionInfo>().AddSkill(this);
        GetComponent<SignalReceiver>().GetReactionAtIndex(0).AddListener(() => Activate());
    }

    protected virtual void InitSelf()
    {
        cameraLocation = 0;
    }

    public virtual IEnumerator GoToBattleZone()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target.team == start.team && target != start)
            {
                if (target.location == 0)
                {
                    int temp = target.location;
                    target.location = start.location;
                    start.location = temp;

                    yield return StartCoroutine(battleManager.SwapPosition(start, target));
                }
            }
        }
    }

    public virtual IEnumerator Do()
    {
        if (playableDirector)
        {
            playableDirector.playableAsset = playableAsset;
            playableDirector.Play();
            yield return new WaitUntil(() => playableDirector.state != UnityEngine.Playables.PlayState.Playing);
        }
        skillCooltimeRemain = skillCooltime;
    }

    public virtual void Activate() { }

    public int GetCameraLocationIndex() { return cameraLocation; }
    public virtual string GetSkillDescription() { return skillDesc; }
    public string GetSkillName() { return skillName; }
    public string GetSkillIconName() { return skillIconName; }
    public int GetCooltimeRemain() { return skillCooltimeRemain;  }
}
