using DG.Tweening;
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
        // 이미 생성된 캐릭터들의 ChampionInfo를 읽어, 현재 존재하는 Location 등에 매핑
        int tlA = GetNextLocationIndex(1);
        int tlB = GetNextLocationIndex(2);

        foreach (Transform tf in team.transform)
        {
            ChampionInfo targetCI = tf.GetComponent<ChampionInfo>();

            championList.Add(targetCI);
            if (teamIndex == 1)
            {
                goSkillSelectPanel.transform.GetChild(tlA).GetComponent<SkillSelectUI>().SetChampionSkill(targetCI);
                targetCI.InitCharacter(teamIndex, tlA);
                tlA++;
            }

            else if (teamIndex == 2)
            {
                targetCI.InitCharacter(teamIndex, tlB);
                tlB++;
            }
        }

    }

    private int GetNextLocationIndex(int teamNumber)
    {
        // championList 에 teamNumber 이 같은 캐릭터들 중, 가장 늦은 Location + 1 을 반환한다.
        int cnt = 0;
        foreach (ChampionInfo ci in championList)
        {
            if (ci.team == teamNumber)
                cnt++;
        }
        return cnt;
    }

    public void SetTeamLocation()
    {
        // 캐릭터들의 Transform.position 를 Location대로 배치

        // team location A, B
        Transform tlA = tfLocations.Find("TeamA");
        Transform tlB = tfLocations.Find("TeamB");
        foreach (ChampionInfo ci in championList)
        {
            if (ci.team == 1)
                ci.transform.position = tlA.GetChild(ci.location).transform.position;
            else if (ci.team == 2)
                ci.transform.position = tlB.GetChild(ci.location).transform.position;
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
            // 쿨타임에 따라 가시성 여부. 추후 최적화 필요
            foreach (Transform tf in goSkillSelectPanel.transform)
            {
                if (tf.GetComponent<SkillSelectUI>())
                {
                    tf.GetComponent<SkillSelectUI>().SetVisualEnableByCooltime();
                }
            }

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
                    Debug.Log(championList[i].name + " skill");
                    yield return StartCoroutine(curSkill.Do());
                    Debug.Log(championList[i].name + " skill done");

                    // 매번 실행하는게 퍼포먼스상 맞지 않지만,
                    // champion.Attcked() 에서 호출하기엔 내부에 코루틴요소가 들어있어서 애매해서 일단 넣음
                    yield return AdjustLocationForDead();

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

    public IEnumerator AdjustLocationForDead()
    {
        // 죽은 챔피언의 location이 0번이라면 다음 차례의 같은팀에게 넘긴다.
        for (int i = 0; i < championList.Count; i++)
        {
            if (championList[i].isDead == true && championList[i].location == 0)
            {
                for (int j = i; j < i + championList.Count; j++)
                {
                    int k = j % championList.Count;
                    if (championList[k].isDead == false && championList[i].team == championList[k].team)
                    {
                        championList[k].location = 0;
                        championList[i].location = 1;
                        yield return StartCoroutine(SwapPosition(championList[i], championList[k]));

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

        if (goBackChar.animator)
            goBackChar.animator.Play("Run");
        
        goBackChar.transform.DOMove(jumpInPosition, 0.8f).SetEase(Ease.Linear);
        

        yield return new WaitForSeconds(0.4f);
        if (jumpInChar.animator) 
            jumpInChar.animator.Play("Run");
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
        if (goBackChar.animator)
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
