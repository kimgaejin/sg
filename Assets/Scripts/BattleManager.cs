using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // 공격속도에 따라 턴 배급 
    // 턴을 잡은 몬스터를 가진 플레이어가 조작
    // 턴을 잡은 몬스터의 스킬을 선택
    // 해당 스킬의 대상을 선택
    // 스킬 수행 후 다음 턴을 가진 몬스터에게 턴을 넘김

    // 한 측의 몬스터들이 모두 행동불능이 되면 (전체체력이 0이되면) 게임 종료

    public GameObject goTeamA;
    public GameObject goTeamB;

    public List<ChampionInfo> championList;
    public bool processButton = false;

    private void Start()
    {
        championList = new List<ChampionInfo>();
        int ind = 1;
        foreach (Transform tf in goTeamA.transform)
        {
            ChampionInfo targetCI = tf.GetComponent<ChampionInfo>();
            targetCI.StartBattle(1, ind);
            championList.Add(targetCI);
            ind++;
        }

        ind = 1;
        foreach (Transform tf in goTeamB.transform)
        {
            ChampionInfo targetCI = tf.GetComponent<ChampionInfo>();
            targetCI.StartBattle(2, ind);
            championList.Add(targetCI);
            ind++;
        }

        SortChampionWithSpeed();
        StartCoroutine(Routine());
    }

    IEnumerator Routine()
    {
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);

        while (true)
        {
            // 각 플레이어의 스킬을 설정해두고
            // if (양 쪽 모두 go를 눌렀거나 타임아웃이 됨)
            // 현재는 모든 캐릭터가 fireball만 쏘기로

            while (processButton == false)
            {
                yield return wait01;
            }
            processButton = false;

            // 스킬을 실행한다
            for (int i = 0; i < championList.Count; i++)
            {
                if (championList[i].isDead == false)
                {
                    championList[i].skill.Do();
                }
            }

            yield return wait01;
        }
    }

    public void OnProcessButton()
    {
        processButton = true;
    }

    private void SortChampionWithSpeed()
    {
        // 챔피언리스트를 공격속도로 정렬한다.
        championList.Sort(delegate (ChampionInfo A, ChampionInfo B)
        {
            if (A.spd > B.spd) return -1;
            else if (A.spd < B.spd) return 1;
            return 0;
        });
    }

    public void AdjustLocationForDead()
    {
        // 죽은 챔피언의 location이 1번이라면 다음 차례의 같은팀에게 넘긴다.
        for (int i = 0; i < championList.Count; i++)
        {
            if (championList[i].isDead == true && championList[i].location == 1)
            {
                for (int j = i; j < i + championList.Count; j++)
                {
                    int k = j % championList.Count;
                    if (championList[k].isDead == false && championList[i].team == championList[k].team)
                    {
                        Debug.Log(championList[i].name + "의 1번 순서를 " + championList[k].name + "으로 계승했습니다.");
                        championList[k].location = 1;
                        championList[i].location = 0;
                        i = -1;
                        break;
                    }
                }
            }
           
        }
    }


}
