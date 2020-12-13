﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 한 스테이지 안에 n개의 라운드가 들어갈 수 있음
    // 각 라운드의 배경종류와 캐릭터들을 초기화하고 끝나면 넘어가는 역할
    private BattleManager battleManager;
    private ScenarioManager scenario;
    private Transform teamA;
    private Transform teamB;
    private List<string []> enemyList;
    private string [] clearEventMessage;

    private bool scenarioProcess = false;   // true면 시나리오 계속 진행, false면 시나리오 종료 후 전투 개시
    private int curRoundIndex;

    private void Awake()
    {
        teamA = GameObject.Find("TeamA").transform;
        teamB = GameObject.Find("TeamB").transform;
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        scenario = GameObject.Find("ScenarioManager").GetComponent<ScenarioManager>();

        // "goStage;stageName;stageDesc;playerChara(/);roundEnemy(/+);scenarioName;mapType;openCondition;clearEvent"
        enemyList = new List< string []>();
#if UNITY_EDITOR
        if (EventManager.Instance.events.Count == 0)
        {
            EventManager.Instance.events.Enqueue("goStage;TestStage;TestStageDesc;주인공/헤돈/;경비병 1+경비병 1/경비병 1+경비병 1+경비병 1;tutorial000;Round 001;;");
        }
#endif
        while (true)
        {
            if (EventManager.Instance.events.Count <= 0) break;

            string eventCommandLine = EventManager.Instance.events.Dequeue();
            Debug.Log(eventCommandLine);

            string[] commandLine = eventCommandLine.Split(';');

            string commandType = commandLine[0];

            if (commandLine[0] == "goStage")
            {
                string stageName = commandLine[1];
                string stageDesc = commandLine[2];
                string[] playerChara = commandLine[3].Split('/');
                string[] roundEnemy = commandLine[4].Split('/');
                string scenarioName = commandLine[5];
                string mapType = commandLine[6];
                clearEventMessage = commandLine[8].Split('/');

                // 플레이어 캐릭터 생성
                foreach (string chara in playerChara)
                {
                    if (chara == "")
                        continue;
                    GameObject charaPrefab = Resources.Load<GameObject>("Prefabs/Character/" + chara) as GameObject;
                    if (charaPrefab != null)
                        Instantiate(charaPrefab, teamA.transform);
                }
                // 몬스터&맵 생성
                List<string> line = new List<string>();
                int i = 0;
                foreach (string enemys in roundEnemy)
                {
                    string[] enemy = enemys.Split('+');
                    line.Clear();

                    foreach (string prefabName in enemy)
                    {
                        if (prefabName != "")
                        {
                            line.Add(prefabName);
                        }
                    }
                    if (0 < line.Count)
                    {
                        enemyList.Add( line.ToArray() );
                        CreateRound(mapType, i);
                        i++;
                    }
                }
                // 시나리오 생성
                scenario.GetCsvTable("Scenario/" + scenarioName);

                break;
            }
        }
    }

    private void Start()
    {
        battleManager.Init();

        StartCoroutine(StartStage());
    }

    private IEnumerator StartStage()
    {
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);

        while (true)
        {
            // 진행하는 라운드는 하나로 고정되어있으므로 polling 방식으로 라운드가 끝났는지 확인
            curRoundIndex = 0;
            while (curRoundIndex < this.transform.childCount)
            {
                yield return wait01;
                if (battleManager.IsRoundFinished())
                {
                    Debug.Log("아ㅣ니 분명");
                    CreateEnemy(curRoundIndex);
                    yield return wait01;
                    battleManager.StartRound(this.transform.GetChild(curRoundIndex));

                    StartScenario();
                    while (scenarioProcess) { yield return wait01; }
                    curRoundIndex++;
                    yield return wait01;
                }
            }

            break;
        }
        curRoundIndex = -1;
        StartScenario();
        while (scenarioProcess) { yield return wait01; } 
    }

    private GameObject CreateRound(string name, int locationSequence)
    {
        // 이름이 name인 프리팹을 가져와 생성한다
        // locationSequence 번째로 배치시킨다: 1당 1000 x 증가
        string path = "Prefabs/Stage/";
        GameObject prefab = Resources.Load<GameObject>(path + name) as GameObject;
        GameObject round = Instantiate(prefab, this.transform);
        round.transform.position = new Vector3((1 + locationSequence) * 1000, 0, 0);
        return round;
    }

    private void SetEnemyTest()
    {
        // ! 임시로 적들 정보 배치. 추후 파일로 바꾸건 뭐로 바꾸건 여하튼 바꿔야됨.
        /*
        List<string> line1 = new List<string>();
        line1.Add("경비병 1");
        line1.Add("경비병 1");
        line1.Add("경비병 1");
        enemyList.Add(line1);

        List<string> line2 = new List<string>();
        line2.Add("경비병 1");
        line2.Add("경비병 1");
        line2.Add("경비병 1");
        enemyList.Add(line2);

        List<string> line3 = new List<string>();
        line3.Add("경비병 1");
        line3.Add("경비병 1");
        line3.Add("경비병 1");
        enemyList.Add(line3);
        */
    }

    private void CreateEnemy(int line)
    {
        foreach (Transform target in teamB)
        {
            Debug.Log("삭제하고");
            Destroy(target.gameObject);
        }

        string path = "Prefabs/Character/";
        foreach (string s in enemyList[line])
        {
            Debug.Log( s+ " s 만들고");

            GameObject prefab = Resources.Load<GameObject>(path + s) as GameObject;
            GameObject target = Instantiate(prefab, teamB);
            target.transform.localPosition = Vector3.zero;
        }
       
    }

    private void SaveClearMessage()
    {
        foreach (string message in clearEventMessage)
        {
            EventManager.Instance.events.Enqueue(message);
        }
    }

    public void StartScenario()
    {
        scenario.ReadCurrentData(curRoundIndex.ToString());
        scenarioProcess = true;
    }

    public void ReadScenario()
    {
        scenarioProcess = scenario.ReadCurrentData(curRoundIndex.ToString());
    }
}
