using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 500f;
    private Rigidbody rigid;
    private Transform tr;
    Vector3 offset;

    public GameObject chargeParticle;
    public GameObject fireParticle;
    PlayerAttack playerAttack;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }
    private void OnEnable()
    {
        if (Managers.Game.GetPlayer() != null)
        {
            playerAttack = Managers.Game.GetPlayer().GetComponent<PlayerAttack>();
        }
    }

    private void Update()
    {
        if ((Vector3.Distance(tr.position, offset) >= 20f))//�����Ÿ� �����
        {
            DisableArrow();
        }

    }

    public void FireArrow(Transform firepos)
    {
        offset = firepos.position;
        rigid.AddForce(tr.right * speed);

    }
  
    public void DisableArrow() 
    {
        chargeParticle.SetActive(false);
        fireParticle.SetActive(false);
        tr.gameObject.SetActive(false);
            
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Monster"))
        {
            rigid.Sleep();
            rigid.useGravity = false;
            tr.position = other.ClosestPointOnBounds(tr.position);
            //�ǰ�ó��
            Status playerstatus = playerAttack.GetComponent<Status>();
            Status status = other.GetComponent<Status>();

            status.TakeDamage(playerstatus,playerAttack.attackRatio);

            DisableArrow();
        }
        else if (other.CompareTag("Ground"))
        {
            rigid.Sleep();
            rigid.useGravity = false;
            tr.position = other.ClosestPointOnBounds(tr.position) + new Vector3(0, 0.5f, 0); // ClosestPointOnBounds -> �浹�������� ���� ����� �� ����
            DisableArrow();
        }
        
    }


    private void OnDisable()//������Ʈ ��Ȱ��ȭ
    {
        //�� �ʱ�ȭ

        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rigid.Sleep();


    }

    
}
