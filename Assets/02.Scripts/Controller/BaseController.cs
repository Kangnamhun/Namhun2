using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
	public Animator animator{get;set;}
	
	[SerializeField]
	protected GameObject _lockTarget;
	

	public bool isAttackReady { get; protected set; } // 공격 가능
	public float attackDelay { get; protected set; } //  딜레이 계산
	public float attackRate { get; protected set; }  // 쿨타임 & 공속
	public bool isSkillReady { get; protected set; } // 공격 가능
	public float skillDelay { get; protected set; } //  딜레이 계산
	public float skillRate { get; protected set; }  // 쿨타임 & 공속


	private void Start()
	{
		Init();
	}


    private void Update()
    {
		
		UpdateMoving();
		UpdateAttack();
		UpdateTime();
	}



	protected abstract void Init();

	protected virtual void UpdateMoving() { }

	protected virtual void UpdateAttack() { }

	void UpdateTime() 
	{
		attackDelay += Time.deltaTime;
		isAttackReady = attackRate < attackDelay;

		skillDelay += Time.deltaTime;
		isSkillReady = skillRate < skillDelay;
	}
	//타겟과 거리 계산
	protected Vector3 DestPos(Vector3 targetpoint)
	{
		Vector3 dest = targetpoint - transform.position;

		dest.y = 0;

		return dest;
	}
}
