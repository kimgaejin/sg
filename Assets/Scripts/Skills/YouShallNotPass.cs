using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouShallNotPass : SkillCommon
{
    public GameObject buffEffect;

    protected override void InitSelf()
    {
        base.InitSelf();
        skillName = "넌 못 지나간다";
        skillDesc = "훼방을 놓을 작정입니다. 큰 데미지로 공격해야 제거할 수 있습니다.\n신성한 힘으로 자신의 체력을 모두 회복합니다.";
        skillIconName = "9";
        skillCooltime= 0;
        skillIndex = 312;
    }

    public override IEnumerator Do()
    {
        yield return StartCoroutine(base.Do());
    }

    public override void Activate()
    {
        foreach (ChampionInfo target in battleManager.championList)
        {
            if (target == start && target.isDead == false)
            {
                target.GetHeal((int)(target.maxHp), 0);
                GameObject buffEffectInstance = Instantiate(buffEffect);
                buffEffectInstance.transform.position = target.transform.position + buffEffect.transform.localPosition;
                buffEffectInstance.SetActive(true);
                break;
            }
        }
    }
}
