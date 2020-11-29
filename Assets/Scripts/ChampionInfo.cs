using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionInfo : MonoBehaviour
{
    // references
    protected ChampionInfo championInfo;
    protected BattleManager battleManager;
    protected HealthBar healthBar;
    public Animator animator;
    public GameObject modelObject;

    // values
    public bool isDead;
    public int team;
    public int location;
    private string name;
    public List<SkillCommon> skills;
    public int curSkillIndex;

    public int maxHp;
    public int hp;
    public int atk;
    public int def;
    public int spd;
    public List<BuffCommon> buffList { get; set; }
    private Transform buffLocation;

    private Transform tfCameraLocaiton;

    // functions
    private void Awake()
    {
        championInfo = transform.GetComponent<ChampionInfo>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        buffLocation = transform.Find("CharacterUI").Find("BuffLocation");
        healthBar= transform.Find("CharacterUI").Find("Health").GetComponent<HealthBar>();
        tfCameraLocaiton = transform.Find("CharacterUI").Find("CameraLocation");
        Transform tfGraphics = transform.Find("Graphics");


        name = transform.name;
        hp = maxHp;
        isDead = false;

        buffList = new List<BuffCommon>();
        skills = new List<SkillCommon>();

        foreach (Transform tf in tfGraphics)
        {
            if (tf.GetComponent<Animator>())
            {
                modelObject = tf.gameObject;
                break;
            }
        }

    }

    public virtual void InitCharacter(int team, int location)
    {
        //hp = maxHp;
        //isDead = false;
        this.team = team;
        this.location = location;
        ShowHpBar();
    }

    public virtual void Attack(ChampionInfo target, int damage, int sequence)
    {
        // 공격할 때 호출
        battleManager.DoPassive(BattleManager.BATTLETIME.ATTACKING, championInfo);

        target.Attacked(damage, sequence);
    }

    public virtual void Attacked(int damage, int sequence)
    {
        // 공격당했을 때 호출
        battleManager.DoPassive(BattleManager.BATTLETIME.ATTACKED, championInfo);

        int totalDmg = damage -= GetDef();
        if (totalDmg < 0) totalDmg = 0; // 공격력보다 방어력이 높을 시 데미지 0
        battleManager.ShowDamage(transform, totalDmg, sequence);

        this.hp -= totalDmg;
        if (hp <= 0)    // 사망
        {
            Debug.Log(name + "은 죽었습니다");
            isDead = true;
            DeleteAllBuff();
            // 코루틴 문제로 BattleManager 쪽으로 옮김
            //battleManager.AdjustLocationForDead();
        }
        if (animator)
            animator.Play("Damage");

        ShowHpBar();
    }

    public virtual void GetBuff(ChampionInfo target, string buffIconName, BuffCommon.BUFFTYPE type, int restTurn, float value, bool isStack, int maxStack, bool isTimeless, int sequence)
    {
        BuffCommon newBuff = new BuffCommon();
        newBuff.Init(target, buffIconName, buffList.Count, type, restTurn, value, isStack, maxStack, isTimeless);

        // 이미 존재하는 버프라면 갱신
        BuffCommon seinorBuff = null;
        foreach (BuffCommon buff in buffList)
        {
            if (buff.buffName == newBuff.buffName)
            {
                seinorBuff = buff;
                break;
            }
        }

        if (seinorBuff == null)
        {   // 버프를 새로 할당받은 경우
            buffList.Add(newBuff);
            CreateBuffIconGameObject(buffIconName, type);
        } 
        else
        {   // 이미 존재하던 버프
            if (seinorBuff.isTimeless == false)
            {   // 지속시간이 있는 경우에만 비교가 필요함
                if (seinorBuff.restTurn < newBuff.restTurn)
                {
                    seinorBuff.restTurn = newBuff.restTurn;
                }
            }

            if (newBuff.isStack)
            {
                newBuff.AddStack();
            }
        }

        // 버프텍스트 생성
        string buffShowText = "";   // 버프생성시 표시되는 내용
        if (type == BuffCommon.BUFFTYPE.INC_ATK) buffShowText = "공격력 증가";
        else if (type == BuffCommon.BUFFTYPE.INC_DEF) buffShowText = "방어력 증가";
        else if (type == BuffCommon.BUFFTYPE.DEC_DEF) buffShowText = "방어력 감소";
        else if (type == BuffCommon.BUFFTYPE.DOT_DMG) buffShowText = "지속 피해";
        else buffShowText = "알수없음";
        battleManager.ShowBuffText(transform, buffShowText, sequence);

    }

    public virtual void GetHeal(int healPoint, int sequence)
    {
        // 회복시 호출
        if (this.hp < this.maxHp + healPoint)
            healPoint = this.maxHp - this.hp;

        battleManager.ShowHeal(transform, healPoint, sequence);

        this.hp += healPoint;
        ShowHpBar();
    }

    public void AddSkill(SkillCommon skill)
    {
        skills.Add(skill);
    }

    public int GetDamageValue()
    {
        return GetAtk();
    }

    public float GetBuffValueSum(BuffCommon.BUFFTYPE type)
    {
        float value = 0;
        foreach (BuffCommon b in buffList)
        {
            if (b.able && b.GetBuffType() == type)
            {
                value += (b.GetValue() * b.stack);
            }
        }
        return value;
    }

    public void SubBuff(BuffCommon.BUFFTYPE type, int restTurn, float value)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BuffCommon b = buffList[i];
            if (b.GetBuffType() == type &&  b.restTurn == restTurn && b.GetValue() == value)
            {
                buffList.RemoveAt(i);
                SetActiveBuffObject(i);
                break; 
            }
        }
    }

    public void SetActiveBuffObject(int index)
    {
        buffLocation.GetChild(index).gameObject.SetActive(false);

        GrabBuffIcons(index);
    }

    private void GrabBuffIcons(int index)
    {
        // index부터의 버프 아이콘들을 왼쪽으로 이동시킨다.
        for (int i = index; i < buffList.Count; i++)
        {
            if (buffList[i].able == true)
            {
                buffLocation.GetChild(i).position += new Vector3(0, 0, 0.25f);
            }
        }
    }

    public void DeleteAllBuff()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BuffCommon b = buffList[i];
            buffList.RemoveAt(i);
            buffLocation.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void CreateBuffIconGameObject(string iconName, BuffCommon.BUFFTYPE type)
    {
        // 캐릭터에 종속되는 버프 아이콘을 만든다

        if (battleManager.goBuffIconPrefab != null)
        {
            GameObject buffIcon = Instantiate(battleManager.goBuffIconPrefab, buffLocation);

            int curBuffSize = 0;
            foreach (BuffCommon b in buffList)
            {
                if (b.able)
                {
                    curBuffSize++;
                }
            }

            // 버프 아이콘 호출
            /*
            // 버프 기능에 따라 아이콘을 불러오던 것
            // Resouces에서 불러오는거라 추후 Manager에서 저장해두고 꺼내다쓰는거로 최적화 필요함
            string buffIconName = "";
            if (type == BuffCommon.BUFFTYPE.INC_ATK) buffIconName = "buff_incAtk";
            else if (type == BuffCommon.BUFFTYPE.INC_DEF) buffIconName = "buff_incDef";
            else if (type == BuffCommon.BUFFTYPE.DEC_DEF) buffIconName = "buff_decDef";
            else if (type == BuffCommon.BUFFTYPE.DOT_DMG) buffIconName = "buff_dotDmg";
            */
            // 버프스킬 이름에 따라 아이콘을 불러오기
            string buffIconName = iconName;

            Sprite buffIconSpr = Resources.Load<Sprite>("SkillIcons/" + buffIconName) as Sprite;
            if (buffIconSpr) buffIcon.GetComponent<SpriteRenderer>().sprite = buffIconSpr;

            buffIcon.transform.localPosition = Vector3.zero;
            // !! 왜 좌표값이 x추가가 아니라 z추가인지 이해가 잘 안되네;
            buffIcon.transform.position += new Vector3(0, 0, -0.25f) * (curBuffSize - 1);

        }
    }

    public int GetHp()
    {
        return hp;
    }

    public void ShowHpBar()
    {
        if (healthBar) healthBar.SetGage(hp / (float)maxHp);
    }

    public Transform GetCameraPoint(int ind)
    {
        return tfCameraLocaiton.GetChild(ind);
    }

    public int GetAtk()
    {
        // 공격력 버프가 적용된 공격력 수치
        float value = GetBuffValueSum(BuffCommon.BUFFTYPE.INC_ATK);
        return atk  + (int)(atk * value);
    }

    public int GetDef()
    {
        // 방어력 버프가 적용된 방어력 수치
        float defValue = GetBuffValueSum(BuffCommon.BUFFTYPE.INC_DEF);
        return def + (int)(def * defValue);
    }
}
