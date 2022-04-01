using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState {
    Idle = 0,
    Walk = 1,
    Attack = 2,
    Trace = 3,
    Die = 4
}

public class MonsterController : BaseController
{
    #region SetUp
    float moveSpeed = 8f, rotateSpeed = 3f;

    public Vector3 limitRange_Min, limitRange_Max;
    
    MonsterState monsterState;
    GameObject player;
    Vector3 movePos;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    private float _attackRange = 4f;

    private float distance = 0f;

    public float attackRange { get; protected set; }
    public float scanRange { get; protected set; }

    protected override void Init()
    {
        attackRate = 1.2f;
        player = Managers.Game.GetPlayer();
        InvokeRepeating("RandomPos", 2f, 3f);
        monsterState = MonsterState.Idle;
        animator = GetComponent<Animator>();
    }
    #endregion
    protected virtual void PlayerScan()
    {
        if (player == null)
         return;

        distance = (player.transform.position - transform.position).magnitude;
        if(distance <= _attackRange && isAttackReady){
            monsterState = MonsterState.Attack;
            return;
        }
        else if (distance <= _scanRange)
        {
            _lockTarget = player;
            monsterState = MonsterState.Trace;
            return;
        }
        else{
            monsterState = MonsterState.Walk;
        }
    }

    protected override void UpdateMoving()
    {
        PlayerScan();
        switch (monsterState) {
            // case MonsterState.Idle:
            //     if (Random.Range(1, 20) == 2)
            //         monsterState = MonsterState.Walk;
            //     break;
            case MonsterState.Walk:
                if(movePos == Vector3.zero)return;
                LookDirection(transform, movePos);
                RigidMovePos(transform, movePos, moveSpeed);
                LimitMoveRange(transform, limitRange_Min, limitRange_Max);
                if (Random.Range(1, 20) == 2)
                    monsterState = MonsterState.Idle;
                animator.SetInteger("state", (int)monsterState);
                break;

            case MonsterState.Trace:
                LookTarget(transform, player.transform, rotateSpeed);
                if(distance >= _attackRange){
                    RigidMovePos(transform, player.transform.position - transform.position, moveSpeed);
                }
                LimitMoveRange(transform, limitRange_Min, limitRange_Max);
                animator.SetInteger("state", (int)monsterState);
          
            break;

            case MonsterState.Attack:
                MonsterAttack();
                if(isAttackReady)
                attackDelay = 0;
                break;
        }
        
    }
    
    void MonsterAttack()
    {
        LookTarget(transform, player.transform, rotateSpeed);
        animator.SetInteger("state", (int)monsterState);

    }
    private void RandomPos(){
        movePos = new Vector3(Random.Range(-1f, 2f), transform.position.y, Random.Range(-1f, 2f));
    }
    private void LookDirection(Transform objTransform, Vector3 moveDir) {
        objTransform.rotation = Quaternion.LookRotation(-moveDir.x * Vector3.right + -moveDir.z * Vector3.forward);
    }

    private void LookTarget(Transform objTransform, Transform targetTransform, float speed) 
    {
        if (animator.GetBool("Attack")) return;
        Vector3 dir = new Vector3(objTransform.position.x - targetTransform.transform.position.x, 0, objTransform.position.z - targetTransform.transform.position.z);
        objTransform.rotation = Quaternion.Lerp(objTransform.rotation, Quaternion.LookRotation(dir), speed * Time.fixedDeltaTime);
    }

    private void RigidMovePos(Transform objTransform, Vector3 dir, float speed) 
    {
        if (animator.GetBool("Attack")) return;
        objTransform.gameObject.GetComponent<Rigidbody>().MovePosition(objTransform.position + new Vector3(dir.x, 0, dir.z).normalized * speed * Time.fixedDeltaTime);
    }

    private void LimitMoveRange(Transform objTransform, Vector3 minRange, Vector3 maxRange) {
        objTransform.position = new Vector3(Mathf.Clamp(objTransform.position.x, minRange.x, maxRange.x), objTransform.position.y, 
                                Mathf.Clamp(objTransform.position.z, minRange.z, maxRange.z));
    }


    protected virtual void OnAttack()
    {
        Managers.Sound.Play("EffectSound/Attack/SlimeAttack");
        Vector3 vec = transform.localPosition + (-transform.forward * 5);
        Collider[] hit = Physics.OverlapSphere(vec, 4f,1<<(int)Layer.Player);
        for (int i = 0; i < hit.Length; i++)
        {
            Status status = hit[i].GetComponent<Status>();
            status.TakeDamage(GetComponent<Status>());
        }
    }
}
