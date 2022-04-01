using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{

    #region hp = baseHP + ��� + ����
    float hp;
    public float Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            if(hp <=0)
            {
                Die();
            }

        }
    }

    public float baseHP = 300; //�⺻�ִ�ü��
    protected float wearHP = 0; //��� ���� ��� �ִ�ü��
    protected float levelHP = 0; //�������� ��� �ִ�ü��
   
    public float MAX_HP
    {
        get { return baseHP + wearHP + levelHP; }
    }
    #endregion

    #region mp = baseMP + ��� + ����
    public float mp { get; protected set; }
    public float baseMP = 300;
    protected float wearMP = 0;
    protected float levelMP = 0;
    public float MAX_MP
    {
        get { return baseMP + wearMP + levelMP; }
    }
    #endregion


    #region attack = baseAttack + ��� + ����
    public float attack { get { return baseAttack + wearAttack + levelAttack; } }
    public float baseAttack = 30;
    protected float wearAttack = 0;
    protected float levelAttack = 0;

    #endregion


    #region defense = baseDefense + ��� + ����
    public float defense { get { return baseDefense + wearDefense + levelDefense; } }
    protected float baseDefense = 3;
    protected float wearDefense = 0;
    protected float levelDefense = 0;
    #endregion

    protected float levelExp = 0;
    
    protected Animator animator;
    public QuestReporter questReporter { get; private set; }
    bool bDeath;
    public bool BDeath 
    {
        get { return bDeath; }
        set
        {
            bDeath = value;
        }

    }
    void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        animator = GetComponent<Animator>();
        questReporter = gameObject.GetOrAddComponent<QuestReporter>();
    }

    public  virtual void TakeDamage(Status attacker,float ratio = 1f) //�´�Ÿ�� ȣ����
    {
        if (BDeath) return; //���� ����ߴٸ�


        int damage = Random.Range( (int)(attacker.attack / 10),  (int)((attacker.attack- (int)defense) * ratio));
        Hp -= damage;

        UI_Damage ui_Damage = Managers.UI.MakeWorldSpaceUI<UI_Damage>(transform);
        ui_Damage.target = transform;
        ui_Damage.Damage = damage;
  
    }


    protected virtual void Die()
    {
#if UNITY_EDITOR
        Debug.Log("���");
#endif

        BDeath = true;
        
        questReporter.Report();
        animator.SetTrigger("Dead");


    }

}
