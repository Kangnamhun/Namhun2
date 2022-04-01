using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    Transform tr;
    Rigidbody rigid;
    PlayerAttack playerAttack;


    private bool isFire;
    public void SetFire(bool value) { isFire = value; }

    Vector3 offset;
    public float speed = 500f;

    GameObject target;
    public void SetTarget(GameObject value) {target = value ;}

    private void OnEnable()
    {

        isFire = false;
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();

        if (Managers.Game.GetPlayer() != null)
        {
            playerAttack = Managers.Game.GetPlayer().GetComponent<PlayerAttack>();
        }





    }
    void Update()
    {
     
        if ((Vector3.Distance(tr.position, offset) >= 20f))//사정거리 벗어나면
        {
            DisableFireBall();
        }

        if (playerAttack != null)
        {
            if (isFire)
            {
                if (target.layer == (int)Layer.Monster)
                {
                    
                    Vector3 vec = playerAttack.attackTarget.transform.position;
                    var cal = playerAttack.attackTarget.GetComponent<Collider>();
                    vec.y += cal.bounds.size.y / 2; // 몹의 중앙에 파이어볼 향하게
                    tr.position = Vector3.Lerp(tr.position, vec, 0.1f);
                }
                else if (target.layer == (int)Layer.Ground)
                {
                    rigid.AddForce(tr.forward * 30f);
                }
                

            }

        }
    }
    public void FireFireBall(Transform firepos)
    {
        offset = firepos.position;

    }
    public void DisableFireBall()
    {
        tr.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag =="Monster")
        {

            tr.gameObject.SetActive(false);
            Status playerstatus = Managers.Game.GetPlayer().GetComponent<Status>();
            Status status = other.GetComponent<Status>();

            status.TakeDamage(playerstatus, playerAttack.attackRatio);
        }
    }

    private void OnDisable()//오브젝트 비활성화
    {
        //값 초기화
        
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rigid.Sleep();
    }
}
