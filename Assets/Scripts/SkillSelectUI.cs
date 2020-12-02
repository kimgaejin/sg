using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectUI : MonoBehaviour
{
    // 각 챔피언이 갖고있는 스킬 설명을 출력하고 스킬 클릭(활성화)시 챔피언의 스킬로 끼워두는 역할
    private ChampionInfo champion;
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
                    tf.Find("characterName").GetComponent<Text>().text = champion.name;
                    tf.Find("characterDesc").GetComponent<Text>().text = champion.desc;
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
                Image imgIcon = tf.Find("SkillIcon").GetComponent<Image>();
                Text txtName = tf.Find("SkillName").GetComponent<Text>();
                if (champion.skills[i].GetCooltimeRemain() < 1)
                {
                    tf.Find("SkillIcon").GetComponent<Image>().color = Color.white;
                    txtName.color = Color.white;
                }
                else
                {
                    txtName.text = champion.skills[i].GetSkillName() + " (" + champion.skills[i].GetCooltimeRemain().ToString() + ")";
                    txtName.color = Color.grey;

                    tf.Find("SkillIcon").GetComponent<Image>().color = Color.gray;
                }
                i++;
            }

        }
    }


    private bool SelectButton(int ind)
    {
        // 스킬을 클릭했을 때, 인덱스를 바꿔주는 함수
        bool isWrongAccess = false;

        // 범위를 초과한 접근
        if (champion.skills.Count <= ind)
        {
            Debug.LogError("스킬 인덱스 범위를 초과한 접근");
            isWrongAccess = true;
        }

        // 쿨타임이 남아있어 선택할 수 없음
        if (0 < champion.skills[ind].GetCooltimeRemain())
        {
            Debug.Log("쿨타임");
            isWrongAccess = true;
            // ! 오디오.선택불가음
        }

        // 패시브 선택
        if (champion.skills[ind].IsPassive())
        {
            isWrongAccess = true;
        }

        // 제대로 선택되지 않은 경우, 선택가능한 스킬을 사용하려고 시도한다.
        if (isWrongAccess == true)
        {
            for (int i = ind + 1; i < ind + skillsTransList.Count; i++)
            {
                if (SelectButton(i % skillsTransList.Count) == true)
                {
                    return true;
                }
            }
            return false;
        }

        // 배경색 변환
        skillsTransList[curSkillIndex].Find("shadow").GetComponent<Image>().color = Color.black;
        skillsTransList[ind].Find("shadow").GetComponent<Image>().color = Color.yellow;

        // 인덱스 및 위치 변환
        curSkillIndex = ind;
        champion.curSkillIndex = curSkillIndex;

        return true;
    }

    public void SelectCurButton()
    {
        SelectButton(curSkillIndex);
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

    public void Skill4Btn()
    {
        SelectButton(3);
    }

    public void Skill5Btn()
    {
        SelectButton(4);
    }

    public void Skill6Btn()
    {
        SelectButton(5);
    }
}
