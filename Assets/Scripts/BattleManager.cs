﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    // 공격속도에 따라 턴 배급 
    // 턴을 잡은 몬스터를 가진 플레이어가 조작
    // 턴을 잡은 몬스터의 스킬을 선택
    // 해당 스킬의 대상을 선택
    // 스킬 수행 후 다음 턴을 가진 몬스터에게 턴을 넘김

    // 한 측의 몬스터들이 모두 행동불능이 되면 (전체체력이 0이되면) 게임 종료
    public GameObject goBuffIconPrefab;

    private Transform tfMainCanvas;
    public GameObject goTeamA;
    public GameObject goTeamB;
    private Transform tfLocations;
    private DamagePlotter dmgPlt;
    private GameObject goSkillSelectPanel;
    private NoticeManager noticeManager;
    private Transform curRound;
    private CameraMoving camera;
    private Transform cameraBase;

    private bool isRoundFinished = true;

    public List<ChampionInfo> championList;
    public bool processButton = false;
    public GameObject goGoButton;
    private Image imgGoButton;

    public void StartRound(Transform round)
    {
        StartCoroutine(Routine(round));
    }

    public void Init()
    {
        // 라운드에따라 변경되지않는 것들
        tfMainCanvas = GameObject.Find("Canvas").transform;
        GameObject goDmgPlt = GameObject.Find("DamagePlotter");
        dmgPlt = goDmgPlt.GetComponent<DamagePlotter>();
        goSkillSelectPanel = tfMainCanvas.Find("SkillSelectPanel").gameObject;
        imgGoButton = goGoButton.GetComponent<Image>();
        noticeManager = GameObject.Find("NoticeManager").GetComponent<NoticeManager>();
        camera = GameObject.Find("CameraManager").GetComponent<CameraMoving>();

        championList = new List<ChampionInfo>();
    }

    public void AddTeam(GameObject team, int teamIndex)
    {
        int ind = championList.Count;

        foreach (Transform tf in team.transform)
        {
            ChampionInfo targetCI = tf.GetComponent<ChampionInfo>();
            targetCI.InitCharacter(teamIndex, ind);
            Debug.Log("targetCI " + targetCI.transform.name + " " + ind.ToString());
            championList.Add(targetCI);
            if (teamIndex == 1)
                goSkillSelectPanel.transform.GetChild(ind).GetComponent<SkillSelectUI>().SetChampionSkill(targetCI);

            ind++;
        }

    }

    public void SetTeamLocation()
    {
        foreach (ChampionInfo ci in championList)
        {
            Debug.Log("ci location " + ci.location.ToString() + " tfLocations.Name " + ci.name);
            ci.transform.position = tfLocations.GetChild(ci.location).transform.position;
        }
    }

    IEnumerator Routine(Transform round)
    {
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);
        WaitForSeconds wait02 = new WaitForSeconds(0.2f);
        WaitForSeconds wait03 = new WaitForSeconds(0.3f);
        WaitForSeconds wait05 = new WaitForSeconds(0.5f);

        // StageManager에서 매 라운드를 시작할 때 호출
        isRoundFinished = false;

        championList.Clear();
        Debug.Log("초기화된 챔피언리스트 " + championList.Count);

        SetRound(round);
        AddTeam(goTeamA, 1);

        goSkillSelectPanel.SetActive(false);
        AddTeam(goTeamB, 2);
        Debug.Log("할당된 챔피언리스트 " + championList.Count);

        SetTeamLocation();

        SortChampionWithSpeed();


        yield return wait01;


        camera.SetCamera(cameraBase);
        noticeManager.ShowNotice("전투를 시작합니다. ", 1);

        while (true)
        {
            // 각 플레이어의 스킬을 설정해두고
            // if (양 쪽 모두 go를 눌렀거나 타임아웃이 됨)

            processButton = false;
            imgGoButton.color = Color.white;

            while (processButton == false)
            {

                yield return wait01;
            }
            processButton = false;
            imgGoButton.color = Color.gray;

            goSkillSelectPanel.SetActive(false);

            // 공격순서에 따라 스킬을 실행한다
            for (int i = 0; i < championList.Count; i++)
            {
                if (championList[i].isDead == false)
                {
                    SkillCommon curSkill = championList[i].skills[championList[i].curSkillIndex];

                    yield return wait02;
                    yield return StartCoroutine(curSkill.GoToBattleZone());
                    yield return wait03;

                    // 스킬에서 제안하는 카메라포인트로 이동하는 함수
                    // 다만, 아직까지 카메라 뷰를 바꿔서 이득을 보는 경우를 만들지 않아 사용하지않음.
                    //Transform tfCameraPointBySkill = championList[i].GetCameraPoint(curSkill.GetCameraLocationIndex());
                    //camera.SetCamera(tfCameraPointBySkill);

                    yield return StartCoroutine(curSkill.Do());
                }
            }
            // 스킬에서 카메라 포인트를 바꾼 후, 원래 위치로 되돌리는 함수
            //camera.SetCamera(cameraBase);

            // 턴이 끝날때의 버프효과
            for (int i = 0; i < championList.Count; i++)
            {
                if (championList[i].isDead == false)
                {
                    foreach (BuffCommon b in championList[i].buff)
                    {
                        b.EndTurn();
                    }
                }
            }

            // 모두 죽은 팀이 있는지 확인
            bool teamADead = true;
            bool teamBDead = true;
            foreach (ChampionInfo ci in championList)
            {
                if (ci.team == 1 && ci.isDead == false) teamADead = false;
                if (ci.team == 2 && ci.isDead == false) teamBDead = false;
            }
            if (teamADead || teamBDead)
            {
                isRoundFinished = true;
                if (teamADead) noticeManager.ShowNotice("모든 아군이 사망했습니다.", 20);
                break;
            }
            yield return wait01;
        }
    }

    public void SetRound(Transform round)
    {
        curRound = round;
        cameraBase = round.Find("StageCameraLocation").Find("Base");
        tfLocations = round.Find("Locations");

    }

    public bool IsRoundFinished()
    {
        return isRoundFinished;
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
                        championList[k].location = 1;
                        championList[i].location = 0;
                        StartCoroutine(SwapPosition(championList[i], championList[k]));
                        i = -1;
                        break;
                    }
                }
            }
           
        }
    }

    public IEnumerator SwapPosition(ChampionInfo jumpInChar, ChampionInfo goBackChar)
    {
        Vector3 rightLookCharacterRotation = new Vector3(0f, 195f, 0f);
        Vector3 leftLookCharacterRotation = new Vector3(0f, -15f, 0f);
        switch(goBackChar.team)
        {
            case 1:
                goBackChar.modelObject.transform.eulerAngles = leftLookCharacterRotation;
                break;
            case 2:
                goBackChar.modelObject.transform.eulerAngles = rightLookCharacterRotation;
                break;
        }
        Vector3 jumpInPosition = jumpInChar.transform.position;
        Vector3 goBackPosition = goBackChar.transform.position;
        goBackChar.animator.Play("Run");
        goBackChar.transform.DOMove(jumpInPosition, 0.8f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.4f);
        jumpInChar.animator.Play("JumpIn");
        yield return jumpInChar.transform.DOMove(goBackPosition, 0.4f).SetEase(Ease.Linear).WaitForCompletion();
        switch (goBackChar.team)
        {
            case 1:
                goBackChar.modelObject.transform.eulerAngles = rightLookCharacterRotation;
                break;
            case 2:
                goBackChar.modelObject.transform.eulerAngles = leftLookCharacterRotation;
                break;
        }
        goBackChar.animator.Play("Idle");
        jumpInChar.ShowHpBar();
        goBackChar.ShowHpBar();

    }

    public void ShowHeal(Transform target, int value, int sequence)
    {
        dmgPlt.ShowDamage(target.position, value, Color.green, sequence);
    }

    public void ShowDamage(Transform target, int value, int sequence)
    {
        dmgPlt.ShowDamage(target.position, value, Color.red, sequence);
    }

    public void ShowBuffText(Transform target, string value, int sequence)
    {
        dmgPlt.ShowDamage(target.position, value, sequence);
    }
}
