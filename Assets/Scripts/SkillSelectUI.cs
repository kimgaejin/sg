using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectUI : MonoBehaviour
{
    // 각 챔피언이 갖고있는 스킬 설명을 출력하고 스킬 클릭(활성화)시 챔피언의 스킬로 끼워두는 역할
    private ChampionInfo champion;
    private RectTransform rtfSignSelect;
    public int curSkillIndex = 0;
    private List<Transform> skillsTransList;
    

    public void SetChampionSkill(ChampionInfo champion)
    {
        // 초기 1회 호출하여 챔피언과 연관된 스킬들을 나열
        this.champion = champion;

        skillsTransList = new List<Transform>();
        int i = 0;
        foreach (Transform tf in transform)
        {

            if (i < champion.skills.Count && tf.name.Contains("Skill"))
            {
                // 스킬들을 칸에 할당
                tf.Find("SkillName").GetComponent<Text>().text = champion.skills[i].GetSkillName();
                tf.Find("SkillDesc").GetComponent<Text>().text = champion.skills[i].GetSkillDescription();
                tf.Find("SkillIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" +champion.skills[i].GetSkillIconName()) as Sprite;
                skillsTransList.Add(tf);
                i++;
            }
            else
            {
                if (tf.name.Contains("Sign"))
                {
                    // 선택 effect나 패시브 effect 등 effect들 관리
                    rtfSignSelect = tf.Find("Select").GetComponent<RectTransform>();
                }
                else
                {
                    // 예외처리. 스킬 개수가 칸보다 많음.
                    tf.gameObject.SetActive(false);
                }
            }
        }

        SetVisualEnableByCooltime();
        SelectButton(0);
    }

    public void SetVisualEnableByCooltime()
    {
        int i = 0;
        // 쿨타임에 따라 아이콘을 정리
        foreach (Transform tf in transform)
        {

            if (i < champion.skills.Count && tf.name.Contains("Skill"))
            {
                // 스킬들을 칸에 할당
                Image img = tf.Find("SkillIcon").GetComponent<Image>();
                if (0 < champion.skills[i].GetCooltimeRemain())
                    tf.Find("SkillIcon").GetComponent<Image>().color  = Color.white;
                else
                    tf.Find("SkillIcon").GetComponent<Image>().color = Color.gray;
                i++;
            }

        }
    }


    private void SelectButton(int ind)
    {
        if (champion.skills.Count <= ind)

        // 쿨타임이 남아있어 선택할 수 없음
            if (0 < champion.skills[ind].GetCooltimeRemain())
        {

            // ! 오디오.선택불가음
            return;
        }

        curSkillIndex = ind;
        champion.curSkillIndex = curSkillIndex;
        rtfSignSelect.anchoredPosition = skillsTransList[ind].GetComponent<RectTransform>().anchoredPosition;
    }

    public void Skill1Btn()
    {
        SelectButton(0);
    }

    public void Skill2Btn()
    {
        SelectButton(1);
    }

    public void Skill3Btn()
    {
        SelectButton(2);
    }
}
