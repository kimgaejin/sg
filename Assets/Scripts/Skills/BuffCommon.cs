using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCommon //: MonoBehaviour
{
    // 버프들의 부모클래스 지만 현재는 이 클래스 자체를 버프로 사용
    // ! 현재 버프의 소멸을 able==false로만 처리하기에 게임이 매우 길어져 버프의 개수가 많아지면 성능문제 가능

    public enum BUFFTYPE { DEFAULT, INC_ATK, DEC_ATK, INC_DEF, DEC_DEF, INC_SPD, DEC_SPD, DOT_DMG };

    public string buffName { get; private set; }  // 같은 버프인지 확인할 때 사용
    protected ChampionInfo champion;
    private int indexOnChampion;
    public bool able;
    public int restTurn { get; set; }     // 남은 턴동안 지속된다.
    protected float value = 0f;      // 적용되는 배율
    public bool isTimeless { get; private set; } = false; // 턴에 종속적인지 (영구지속 버프인지, 턴이 지나면 사라지는 버프인지)
    public bool isStack {get; private set;} = false;    // n중첩으로 중첩될 수 있는 버프인지
    public int stack { get; private set; } = 1;
    public int maxStack { get; private set; } = 1;
    protected BUFFTYPE type;

    public void Init(ChampionInfo champion, string buffName, int indexOnChampion, BUFFTYPE type, int turn, float value, bool isStack, int maxStack, bool isTimeless)
    {
        this.champion = champion;
        this.buffName = buffName;
        this.indexOnChampion = indexOnChampion;
        this.type = type;
        this.restTurn = turn;
        this.value = value;
        this.isStack = isStack;
        this.maxStack = maxStack;
        this.isTimeless = isTimeless;
        able = true;
    }

    public virtual void StartTurn()
    {
        // 턴이 시작할 때 호출

        if (!able) return;

    }

    public virtual void EndTurn()
    {
        // 턴이 끝났을 때 호출

        if (!able) return;

        restTurn--;
        if (restTurn <= 0)
        {
            champion.SetActiveBuffObject(indexOnChampion);
            able = false;
        }
    }

    public virtual void RemoveBuff()
    {
        // 버프를 제거할 때 호출

        if (!able) return;

        able = false;
    }

    public virtual void AddStack()
    {
        stack++;
        if (maxStack < stack) stack = maxStack;
    }

    public virtual void AttackEffect()
    {
        // 이 버프를 지닌 상태로 공격 할 때 추가효과

    }

    public virtual void BeAttackedEffect()
    {
        //이 버프를 지닌 상태로 공격에 맞을 때 추가효과

    }

    public BUFFTYPE GetBuffType()
    {
        return type;
    }

    public float GetValue()
    {
        return value;
    }
}