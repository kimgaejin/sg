﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionInfo : MonoBehaviour
{
    // references
    private Image img;
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

    // functions
    private void Awake()
    {
        img = transform.GetComponent<Image>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
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
            battleManager.AdjustLocationForDead();
        }

        if (hpBar) hpBar.Show(transform.position, hp/(float)maxHp);
    }

    public virtual void GetBuff(ChampionInfo target, BuffCommon.BUFFTYPE type, int restTurn, float value)
    {
        BuffCommon newBuff = new BuffCommon();
        newBuff.Init(target, type, restTurn, value);
        buff.Add(newBuff);
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
                break; 
            }
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
