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
    public enum BATTLETIME { DEFUALT, STAGE_START, ROUND_START, ROUND_END, TURN_START, TURN_END, ATTACK_BEFORE, ATTACKING, ATTACKED, ATTACK_AFTER};

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
    private bool isRoundClear = false;

    public List<ChampionInfo> championList;
    public List<SkillCommon> passiveList;
    public List<SkillSelectUI> selectableSkillPopup;
    public bool processButton = false;
    public GameObject goGoButton;
    private Image imgGoButton;

    public Coroutine StartRound(Transform round)
    {
        return StartCoroutine(Routine(round));
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
        passiveList = new List<SkillCommon>();
        selectableSkillPopup = new List<SkillSelectUI>();
    }

    public void DoPassive(BATTLETIME time, ChampionInfo target)
    {
        // BATTLETIME 에 따라 턴 시작, 공격 중, 공격 받을 때 등 호출하는 함수
        // 패시브리스트에 있는 패시브들을 모두 실행한다.
        // 공격 할 때 받을 때 모두 passiveList를 한바퀴씩 도니까 성능이 너무 떨어지지 않을까 걱정은 되는데 일단 그대로

        foreach (SkillCommon sc in passiveList)
        {
            sc.Passive(time, target);
        }
    }

    public void AddPassiveList(ChampionInfo target)
    {
        // target의 skills를 확인하고 패시브리스트에 해당하는 스킬들을 넣는다
        foreach (SkillCommon sc in target.skills)
        {
            if (sc.IsPassive())
            {
                passiveList.Add(sc);
            }
        }
    }

    public void DeletePassiveTarget(ChampionInfo target)
    {
        // target에 해당하는 사람의 passive를 목록에서 제외한다.
        List<SkillCommon> toRemove = new List<SkillCommon>();
        foreach (SkillCommon sc in passiveList)
        {
            if (target == sc.start)
            {
                toRemove.Add(sc);
            }
        }

        foreach (SkillCommon sc in toRemove)
        {
            passiveList.Remove(sc);
        }
    }

    public void AddTeam(GameObject team, int teamIndex)
    {
        // 이미 생성된 캐릭터들의 ChampionInfo를 읽어, 현재 존재하는 Location 등에 매핑
        int tlA = GetNextLocationIndex(1);
        int tlB = GetNextLocationIndex(2);

        int teamACnt = 0;
        foreach (Transform tf in team.transform)
        {
            ChampionInfo targetCI = tf.GetComponent<ChampionInfo>();

            championList.Add(targetCI);
            if (teamIndex == 1)
            {
                goSkillSelectPanel.transform.GetChild(teamACnt).GetComponent<SkillSelectUI>().SetChampionSkill(targetCI);
                selectableSkillPopup.Add(goSkillSelectPanel.transform.GetChild(tlA).GetComponent<SkillSelectUI>());
                targetCI.InitCharacter(teamIndex, tlA);
                AddPassiveList(targetCI);
                tlA++;
                teamACnt++;
            }

            else if (teamIndex == 2)
            {
                targetCI.InitCharacter(teamIndex, tlB);
                AddPassiveList(targetCI);
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
        Debug.Log(tfLocations.name);
        Transform tlA = tfLocations.Find("TeamA");
        Transform tlB = tfLocations.Find("TeamB");

        foreach (ChampionInfo ci in championList)
        {
            if (ci.team == 1)
                if (ci.location < tlA.childCount)
                {
                    ci.transform.position = tlA.GetChild(ci.location).transform.position;
                    ci.transform.rotation = Quaternion.Euler(0, 195, 0);
                }
                else
                    Debug.LogError("SetTeamLocation(); 캐릭터의 수가 배정된 Location보다 많음");
            else if (ci.team == 2)
                if (ci.location < tlB.childCount)
                {
                    ci.transform.position = tlB.GetChild(ci.location).transform.position; 
                    ci.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                    Debug.LogError("SetTeamLocation(); 캐릭터의 수가 배정된 Location보다 많음");
        }
    }

    IEnumerator Routine(Transform round)
    {
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);
        WaitForSeconds wait02 = new WaitForSeconds(0.2f);
        WaitForSeconds wait03 = new WaitForSeconds(0.3f);
        WaitForSeconds wait05 = new WaitForSeconds(0.5f);
        int teamDead = 0;

        // StageManager에서 매 라운드를 시작할 때 호출
        isRoundFinished = false;
        isRoundClear = false;

        championList.Clear();
        Debug.Log("초기화된 챔피언리스트 " + championList.Count);

        yield return wait01;

        SetRound(round);
        AddTeam(goTeamA, 1);

        goSkillSelectPanel.SetActive(false);
        AddTeam(goTeamB, 2);
        Debug.Log("할당된 챔피언리스트 " + championList.Count);

        SetTeamLocation();
        SortChampionWithSpeed();

        camera.SetCamera(cameraBase);
        noticeManager.ShowNotice("전투를 시작합니다. ", 1);

        // 전투 시작시 모든 캐릭터 BattleIdle 애니메이션
        //championList.ForEach((character) => character.animator.Play("BattleIdle"));
        // 애니메이션이 없는 경우 예외처리
        foreach (ChampionInfo ci in championList)
        {
            if (ci.animator) ci.animator.Play("BattleIdle");
        }


        while (true)
        {
            Debug.Log("Passive List: " + passiveList.Count.ToString());
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

            DoPassive(BATTLETIME.TURN_START, null);
            // 스킬 쿨타임 줄이기
            // 스킬을 시전한 이후부터 쿨타임이 생기므로 위치를 함부로 바꾸면 안됨
            DecreaseSkillCooltimeRemain();

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

                    Debug.Log("curSkill " + curSkill.GetSkillName());
                    yield return StartCoroutine(curSkill.Do());
                    if (championList[i].animator)
                        championList[i].animator.CrossFade("BattleIdle", 0.1f);
                    yield return wait05;

                    teamDead = CheckTeamDead();
                    if (teamDead != 0) break;

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
                    foreach (BuffCommon b in championList[i].buffList)
                    {
                        b.EndTurn();
                    }
                }
            }

            DoPassive(BATTLETIME.TURN_END, null);

            RecursingSkillSelectPanel();

            teamDead = CheckTeamDead();
            if (teamDead != 0) break;
            
            yield return wait01;
        }

        // 한 팀이 모두 죽어 종료
        if (teamDead == 1)
        {
            noticeManager.ShowNotice("모든 아군이 사망했습니다.", 20);
        }

        // 패시브를 모두 삭제하고 다음 라운드에 다시 넣는다.
        // 버프들은 지속되기에 큰 상관은 없다. 지속되어야하는 패시브가 있을 시 변경
        foreach (ChampionInfo ci in championList)
        {
            DeletePassiveTarget(ci);
        }

        // 전투 종료 후 모든 캐릭터 Idle 애니메이션
        foreach (ChampionInfo ci in championList)
        {
            if (ci.animator)
                ci.animator.CrossFade("Idle", 0.1f);
        }

        yield return wait05;

        isRoundFinished = true;
        if (teamDead == 1)
            isRoundClear = false;
        else
            isRoundClear = true;
        yield break;
    }

    private int CheckTeamDead()
    {
        bool teamADead = true;
        bool teamBDead = true;
        foreach (ChampionInfo ci in championList)
        {
            if (ci.team == 1 && ci.isDead == false) teamADead = false;
            if (ci.team == 2 && ci.isDead == false) teamBDead = false;
        }
        if (teamADead) return 1;
        if (teamBDead) return 2;
        return 0;
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

    public bool IsRoundClear()
    {
        return isRoundClear;
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
            goBackChar.animator.CrossFade("Run", 0.1f);
        
        goBackChar.transform.DOMove(jumpInPosition, 0.8f).SetEase(Ease.Linear);
        

        yield return new WaitForSeconds(0.4f);
        if (jumpInChar.animator) 
            jumpInChar.animator.CrossFade("Run", 0.1f);
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
        if (jumpInChar.animator)
            jumpInChar.animator.CrossFade("BattleIdle", 0.1f);
        if (goBackChar.animator)
            goBackChar.animator.CrossFade("BattleIdle", 0.1f);
        jumpInChar.ShowHpBar();
        goBackChar.ShowHpBar();

    }

    private void RecursingSkillSelectPanel()
    {
        // 스킬 사용 후 쿨타임이 생겼으면 다른 스킬로 커서를 옮기는 함수
        foreach (SkillSelectUI ssui in selectableSkillPopup)
        {
            ssui.SelectCurButton();
        }
    }

    private void DecreaseSkillCooltimeRemain()
    {
        // 스킬 쿨타임 줄이기
        for (int i = 0; i < championList.Count; i++)
        {
            if (championList[i].isDead == false)
            {
                foreach (SkillCommon skill in championList[i].skills)
                {
                    skill.DecreaseCooltimeRemain();
                }
            }
        }

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
