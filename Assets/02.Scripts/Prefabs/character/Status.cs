using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{

    #region hp = baseHP + 장비 + 레벨
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

    public float baseHP = 300; //기본최대체력
    protected float wearHP = 0; //장비를 껴서 얻는 최대체력
    protected float levelHP = 0; //레벨업시 얻는 최대체력
   
    public float MAX_HP
    {
        get { return baseHP + wearHP + levelHP; }
    }
    #endregion

    #region mp = baseMP + 장비 + 레벨
    public float mp { get; protected set; }
    public float baseMP = 300;
    protected float wearMP = 0;
    protected float levelMP = 0;
    public float MAX_MP
    {
        get { return baseMP + wearMP + levelMP; }
    }
    #endregion


    #region attack = baseAttack + 장비 + 레벨
    public float attack { get { return baseAttack + wearAttack + levelAttack; } }
    public float baseAttack = 30;
    protected float wearAttack = 0;
    protected float levelAttack = 0;

    #endregion


    #region defense = baseDefense + 장비 + 레벨
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

    public  virtual void TakeDamage(Status attacker,float ratio = 1f) //맞는타겟 호출함
    {
        if (BDeath) return; //만약 사망했다면


        int damage = Random.Range( (int)(attacker.attack / 10),  (int)((attacker.attack- (int)defense) * ratio));
        Hp -= damage;

        UI_Damage ui_Damage = Managers.UI.MakeWorldSpaceUI<UI_Damage>(transform);
        ui_Damage.target = transform;
        ui_Damage.Damage = damage;
  
    }


    protected virtual void Die()
    {
#if UNITY_EDITOR
        Debug.Log("사망");
#endif

        BDeath = true;
        
        questReporter.Report();
        animator.SetTrigger("Dead");


    }

}
