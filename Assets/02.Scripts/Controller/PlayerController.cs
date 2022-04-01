using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{

    public Define.AttackType attackType { get; private set; }

    Rigidbody rigid;
    PlayerStatus playerStatus;
    QuestReporter questReporter;

    [HideInInspector]
    public PlayerAttack playerAttack;

    Vector3 moveVec = Vector3.zero;
    Vector3 dir = Vector3.zero;
    Vector3 rollVec = Vector3.zero;

    float moveAmount = 0f;
    float moveSpeed = 8f;
    public bool isJump { get; private set; }
    public bool isRoll { get; private set; }

    int _mask = (1 << (int)Layer.Item)
        | (1 << (int)Layer.Npc)
        | (1 << (int)Layer.Monster)
        | (1 << (int)Layer.Ground);

    protected override void Init()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerStatus = GetComponent<PlayerStatus>();
        questReporter = GetComponent<QuestReporter>();

        Managers.Mouse.MouseAction -= OnMouseEvent;
        Managers.Mouse.MouseAction += OnMouseEvent;

    }
    protected override void UpdateMoving()
    {
        
        if (playerAttack == null ? false : !playerAttack.canMove ) return; // 전직을 했고, canMove -> 공격, 스킬 사용중 이동 안 됨(특정스킬 제외)
        if (!Managers.UI.isAction) //팝업 있을때 행동 막기
        {
            Move();
            Run();
            Jump();
            Roll();
            SoundSet();
        }
        
    }
    protected override void UpdateAttack()
    {
        if (!Managers.UI.isAction)
        {
            OnAttack();
        }

    }

    #region moving
    private void Move()
    {
        moveVec = new Vector3(Managers.Input.hAxis, 0, Managers.Input.vAxis).normalized;
        if (isRoll) moveVec = rollVec; //구를때 방향전환 막기

        if (playerAttack != null)  // 공격중 이동 막기
        {
            if (!playerAttack.canMove) MoveStop();
        }

        float m = Mathf.Abs(Managers.Input.hAxis) + Mathf.Abs(Managers.Input.vAxis);
        moveAmount = Mathf.Clamp01(m);

        

        transform.LookAt(transform.position + moveVec);


        transform.position += moveVec * moveSpeed * Time.deltaTime;
        animator.SetFloat("Move", moveAmount, 0.2f, Time.deltaTime);

        //마우스로 이동 및 행동
        if (_lockTarget != null)
        {
            dir = DestPos(_lockTarget.transform.position); // 타겟과의 거리 값

            if (_lockTarget.layer == (int)Layer.Npc || _lockTarget.layer == (int)Layer.Item)
            {
                if (dir.magnitude < 0.5f) //케릭터가 타겟이랑 0.5미터 이내로 들어오면
                {
                    if(_lockTarget.layer == (int)Layer.Npc)
                    {
                        Managers.talk.Action(_lockTarget);
                    }
                    else if (_lockTarget.layer == (int)Layer.Item) //타겟의 레이어가 아이템이면
                    {
                       
                        TakeItem(_lockTarget);
                    }

                    _lockTarget = null;
                    return;
                }
                else
                {
                    MouseMove(); // 마우스로 이동할때의 함수
                }
            }
        }
    }

    private void TakeItem(GameObject _itemGO) //아이템 습득하는 스크립트
    {
        if (_itemGO != null) //      아이템이 있으면
        {
            //고유정보 : Itemcode, Itemname등등
            ItemPickUp _pick = _itemGO.GetComponent<ItemPickUp>(); //아이템 오브젝트안에 ItemPickUp 스크립트를찾아서
            if (_pick)
            {
                #if UNITY_EDITOR
                Debug.Log(_pick.itemData.itemName + " 획득했습니다");
                #endif
                

                //인벤토리에 넣어주기
                if(_pick.itemData.itemType == eItemType.Coin) //아이템타입이 코인이면
                {
                    //스텟에 직접넣어준다
                    Managers.Sound.Play("EffectSound/Coin");
                    playerStatus.gold += _pick.count;
                    _pick.ClearDestroy();
                }
                else
                {
                    bool _bGet = Managers.UI.ui_Inventory.AddItemData(_pick.itemData);
                    if (_bGet)
                    {
                        _pick.ClearDestroy();//오브젝트 주우면 필드에 주운아이템은 사라지게하기
                    }
                    else
                    {
                        Managers.UI.ui_ErrorText.SetErrorText(Define.Error.MaxInv);
#if UNITY_EDITOR
                        Debug.Log("아이템창이 가득찼습니다");
#endif
                    }
                }
            }
        }
    }


    void MouseMove()
    {
        moveAmount = Mathf.Clamp01(dir.magnitude);
        animator.SetFloat("Move", moveAmount, 0.2f, Time.deltaTime);

        transform.position += dir.normalized * moveSpeed * moveAmount * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
    }
    void Run()
    {
        if (playerAttack != null ? playerAttack.isAttack : false) return;// 이동스킬 중 달리기 막기

        if (isJump) 
        {
            moveSpeed = 8f*0.8f;
            return;
        }
        bool runnig = Managers.Input.run && moveVec.magnitude != 0f;
        
        moveSpeed = runnig ? 8f * 1.1f : 8f * 0.8f;
        animator.SetBool("IsRun", runnig);
    }


    public void MoveStop() // 마우스 이동중 다른 행동시 초기화
    {
        moveVec = Vector3.zero;
        _lockTarget = null;
    }


    #endregion
    #region jump
    private void Jump()
    {
       
        Jumptf(); // 떨어질때
        if (Managers.Input.jump && !isJump && !Managers.Input.roll && rigid.velocity.y == 0 && !isRoll)
        {
            isJump = true;
            animator.SetBool("IsJump", true);
            animator.SetTrigger("DoJump");
            animator.SetBool("IsRun", false);
            rigid.AddForce(Vector3.up * 17, ForceMode.Impulse);
        }
        
    }
    private void Jumptf()
    {
        if (rigid.velocity.y < -1f)
        {
            animator.SetBool("IsFall", true);
            animator.SetBool("IsJump", true);
            
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
            animator.SetBool("IsJump", false);
            animator.SetBool("IsFall", false);
        }

    }
    #endregion
    #region roll
    private void Roll()
    {
        if (playerAttack != null ? playerAttack.isAttack : false) return; //공격중에 행동금지

        if (!isJump && moveAmount != 0 && Managers.Input.roll)
        {

            rollVec = moveVec;
            isRoll = true;
            animator.SetTrigger("IsRoll");
            moveSpeed *= 1.5f;
            Invoke("RollOut", 0.5f); // 0.5초간 행동 제어
        }
    }
    private void RollOut()
    {
        isRoll = false;
        animator.ResetTrigger("IsRoll");
        moveSpeed /= 1.5f;


    }
    #endregion
    #region Attack
    void OnAttack()
    {
        if (Managers.Input.hAxis != 0 || Managers.Input.vAxis != 0 || Managers.Input.roll || Managers.Input.jump) //키보드 조작시 타겟 지우기
        {
            _lockTarget = null;
        }
        if (_lockTarget != null)
        {


            dir = DestPos(_lockTarget.transform.position);

            if (_lockTarget.layer == (int)Layer.Monster)
            {
                if (playerAttack == null)
                {
                    Managers.UI.ui_ErrorText.SetErrorText(Define.Error.NoneJob);
                    return;
                }

                if (DistanceAttackPos(dir)) //거리 비교 bool
                {
                    playerAttack.AttackTargetSet(_lockTarget);
                    _lockTarget = null;
                    playerAttack.OnAttack();
                    

                    return;
                }
                else
                {
                    MouseMove();
                }
            }
            else if (_lockTarget.layer == (int)Layer.Ground)
            {
                if (playerAttack == null)
                {
                    Managers.UI.ui_ErrorText.SetErrorText(Define.Error.NoneJob);
                    return;
                }
                playerAttack.AttackTargetSet(_lockTarget);
                _lockTarget = null;
                playerAttack.OnAttack();

            }
        }
    }

    //사정거리계산 메서드
    bool DistanceAttackPos(Vector3 destpos)
    {
        
        return destpos.magnitude <= playerAttack.range;
    }
    //클릭한곳 보는 함수
    void LookHitPoint(RaycastHit hit)
    {
        if (!isRoll && playerAttack != null ? playerAttack.canMove : true)
        {
            Vector3 turnVec = hit.point - transform.position;
            turnVec.y = 0;
            transform.LookAt(transform.position + turnVec);
        }
    }

    #endregion

  

    //마우스 클릭 이벤트 받는 메서드
    void OnMouseEvent(Define.MouseEvent evt)
    {
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        if (raycastHit)
        {
            switch (evt)
            {
                case Define.MouseEvent.PointerDown:

                    attackType = Define.AttackType.NormalAttack;  // 일반공격
                    LookHitPoint(hit);
                    if (_lockTarget != null) // 이벤트 발생시 비어있지않다면 비어주고 다시 부여
                    {
                        _lockTarget = null;
                    }

                    _lockTarget = hit.collider.gameObject;
                    questReporter.Report();

                    break;

                case Define.MouseEvent.PointerRightDown:

                    attackType = Define.AttackType.SkillAttack;
                    LookHitPoint(hit);
                    if (_lockTarget != null) // 이벤트 발생시 비어있지않다면 비어주고 다시 부여
                    {
                        _lockTarget = null;
                    }
                    _lockTarget = hit.collider.gameObject;

                    break;

            }


        }
    }

    void SoundSet()
    {
        if(isJump == true)
        {
            Managers.Sound.Play("Moving/Jump", Define.Sound.Moving);
        }

        else if (animator.GetFloat("Move") > 0.3f)
        {
            if(isRoll == true)
            {
                Managers.Sound.Play("Moving/Roll", Define.Sound.Moving);
            }
            else if (animator.GetBool("IsRun") == true)
            {
                Managers.Sound.Play("Moving/Run", Define.Sound.Moving);
            }
            else if (animator.GetBool("IsRun") == false)
            {
        
                Managers.Sound.Play("Moving/Walk", Define.Sound.Moving);
            }
        }
        
        else
        {
       
            Managers.Sound.StopSound(Define.Sound.Moving);
        }
   

    }
}
