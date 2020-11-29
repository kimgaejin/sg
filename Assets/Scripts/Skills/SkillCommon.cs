using System.Collections;
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
    protected bool isPassive = false;

    private void Awake()
    {
        start = transform.GetComponent<ChampionInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        InitSelf();
    }

    private void Start()
    {
        transform.GetComponent<ChampionInfo>().AddSkill(this);
        playableDirector.GetComponent<SignalReceiver>().GetReactionAtIndex(0).AddListener(() => Activate());
    }

    protected virtual void InitSelf()
    {
        cameraLocation = 0;

        // find playableDirector
        Transform tfGraphics =  start.transform.Find("Graphics");
        if (!playableDirector)
        {
            foreach (Transform target in tfGraphics)
            {
                PlayableDirector pd = target.GetComponent<PlayableDirector>();
                if (pd)
                {
                    playableDirector = pd;
                    break;
                }
            }
        }
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
            // 애니메이션 스킬중첩때문에 임시 주석
            /*
            playableDirector.playableAsset = playableAsset;
            playableDirector.Play();
            yield return new WaitUntil(() => playableDirector.state != UnityEngine.Playables.PlayState.Playing);
            */
            Activate();
        }
        else
        {
            Activate();
        }
        skillCooltimeRemain = skillCooltime;
        yield break;
    }

    public virtual void Activate() { }

    public virtual void Passive(BattleManager.BATTLETIME tag, ChampionInfo target) { }        
    // [턴 시작, 차례가 되었을 때, 아군/적군 (스킬 사용 전, 스킬 사용 중의 값, 스킬 사용 후), 턴 종료]


    public int GetCameraLocationIndex() { return cameraLocation; }
    public virtual string GetSkillDescription() { return skillDesc; }
    public string GetSkillName() { return skillName; }
    public string GetSkillIconName() { return skillIconName; }
    public int GetCooltimeRemain() { return skillCooltimeRemain;  }

    public bool IsPassive() { return isPassive; }
}
