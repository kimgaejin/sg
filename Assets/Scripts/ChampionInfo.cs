using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionInfo : MonoBehaviour
{
    // references
    protected BattleManager battleManager;
    protected HpBar hpBar;
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
    private int hp;
    public int atk;
    public int def;
    public int spd;
    public List<BuffCommon> buff;
    private Transform buffLocation;


    // functions
    private void Awake()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        buffLocation = transform.Find("CharacterUI").Find("BuffLocation");
        name = transform.name;
        hp = maxHp;

        buff = new List<BuffCommon>();
        skills = new List<SkillCommon>();
    }

    public virtual void StartBattle(int team, int location)
    {
        //img.color = Color.white;
        hp = maxHp;
        isDead = false;
        this.team = team;
        this.location = location;
    }

    public virtual void Attacked(int damage)
    {
        // 공격당했을 때 호출

        int appDef = this.def;
        float defValue = GetBuffValueSum(BuffCommon.BUFFTYPE.INC_DEF);
        appDef = appDef + (int)(appDef * defValue);

        int totalDmg = damage -= appDef;
        if (totalDmg < 0) totalDmg = 0; // 공격력보다 방어력이 높을 시 데미지 0
        battleManager.ShowDamage(transform, totalDmg);

        this.hp -= totalDmg;
        if (hp <= 0)    // 사망
        {
            Debug.Log(name + "은 죽었습니다");
            isDead = true;
            DeleteAllBuff();
            battleManager.AdjustLocationForDead();
        }

        if (hpBar) hpBar.Show(transform.position, hp/(float)maxHp);
    }

    public virtual void GetBuff(ChampionInfo target, BuffCommon.BUFFTYPE type, int restTurn, float value)
    {
        BuffCommon newBuff = new BuffCommon();
        newBuff.Init(target, buff.Count, type, restTurn, value);
        buff.Add(newBuff);

        // 버프 아이콘 추가
        if (battleManager.goBuffIconPrefab != null)
        {
            GameObject buffIcon = Instantiate(battleManager.goBuffIconPrefab, buffLocation);

            int curBuffSize = 0;
            foreach (BuffCommon b in buff)
            {
                if (b.able)
                {
                    curBuffSize++;
                }
            }
           
            // Resouces에서 불러오는거라 추후 Manager에서 저장해두고 꺼내다쓰는거로 최적화 필요함
            string buffIconName = "";
            if (type == BuffCommon.BUFFTYPE.INC_ATK) buffIconName = "buff_incAtk";
            else if (type == BuffCommon.BUFFTYPE.INC_DEF) buffIconName = "buff_incDef";
            else if (type == BuffCommon.BUFFTYPE.DEC_DEF) buffIconName = "buff_decDef";
            else if (type == BuffCommon.BUFFTYPE.DOT_DMG) buffIconName = "buff_dotDmg";

            Sprite buffIconSpr = Resources.Load<Sprite>("SkillIcons/" + buffIconName) as Sprite;
            if (buffIconSpr) buffIcon.GetComponent<SpriteRenderer>().sprite = buffIconSpr;

            buffIcon.transform.localScale = Vector3.one;
            buffIcon.transform.localPosition = Vector3.zero;
            // !! 왜 좌표값이 x추가가 아니라 z추가인지 이해가 잘 안되네;
            buffIcon.transform.position += new Vector3(0, 0, -0.25f) * (curBuffSize-1);
        }
    }

    public void AddSkill(SkillCommon skill)
    {
        skills.Add(skill);
    }

    public int GetDamageValue()
    {
        int damage = atk;
        float value = GetBuffValueSum(BuffCommon.BUFFTYPE.INC_ATK);
        damage = damage + (int)(atk * value);
        return damage;
    }

    public float GetBuffValueSum(BuffCommon.BUFFTYPE type)
    {
        float value = 0;
        foreach (BuffCommon b in buff)
        {
            if (b.able && b.GetBuffType() == type)
            {
                value += b.GetValue();
            }
        }
        return value;
    }

    public void SubBuff(BuffCommon.BUFFTYPE type, int restTurn, float value)
    {
        for (int i = 0; i < buff.Count; i++)
        {
            BuffCommon b = buff[i];
            if (b.GetBuffType() == type &&  b.GetRestTurn() == restTurn && b.GetValue() == value)
            {
                buff.RemoveAt(i);
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
        for (int i = index; i < buff.Count; i++)
        {
            if (buff[i].able == true)
            {
                buffLocation.GetChild(i).position += new Vector3(0, 0, 0.25f);
            }
        }
    }

    public void DeleteAllBuff()
    {
        for (int i = 0; i < buff.Count; i++)
        {
            BuffCommon b = buff[i];
            buff.RemoveAt(i);
            buffLocation.GetChild(i).gameObject.SetActive(false);
        }
    }

    public int GetHp()
    {
        return hp;
    }

    public void LinkHpBar(HpBar hb)
    {
        hpBar = hb;
        ShowHpBar();
    }

    public void ShowHpBar()
    {
        if (hpBar) hpBar.Show(transform.position, hp / (float)maxHp);
    }
}
