using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
	public Animator animator{get;set;}
	
	[SerializeField]
	protected GameObject _lockTarget;
	

	public bool isAttackReady { get; protected set; } // ���� ����
	public float attackDelay { get; protected set; } //  ������ ���
	public float attackRate { get; protected set; }  // ��Ÿ�� & ����
	public bool isSkillReady { get; protected set; } // ���� ����
	public float skillDelay { get; protected set; } //  ������ ���
	public float skillRate { get; protected set; }  // ��Ÿ�� & ����


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
	//Ÿ�ٰ� �Ÿ� ���
	protected Vector3 DestPos(Vector3 targetpoint)
	{
		Vector3 dest = targetpoint - transform.position;

		dest.y = 0;

		return dest;
	}
}
