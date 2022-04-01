using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonsterController
{
    [SerializeField]
    private Transform[] bombSpawn;
    private GameObject BombSlime;
    //private float moveSpeed = 8f, rotateSpeed = 3f;
    private string bombSlime;
    public ParticleSystem RangeParticle;
    public ParticleSystem DamageParticle;
    Status status;
    protected override void Init()
    {
        base.Init();

    }
    private void Awake() {


        bombSpawn = Util.FindChild(gameObject,"BombSpawn", true).GetComponentsInChildren<Transform>();
        bombSlime = "Boom_Slime_A";
        status = GetComponent<Status>();
         attackRate = 4f;
        skillRate = 15f;

    }
    protected override void UpdateMoving(){
        base.UpdateMoving();
    }
    protected override void UpdateAttack()
    {
        base.UpdateAttack();

        if(isSkillReady){
            switch(Random.Range(5,7)){
                case 5:
                    animator.SetInteger("state",5);
                    skillDelay = 0;
                break;
                case 6:
                    animator.SetInteger("state",6);
                    skillDelay = 0;
                break;
                case 7:
                break;
            }
        }
    }
    
    protected override void OnAttack()
    {
        Vector3 vec = transform.localPosition + (-transform.forward * 5);
        Collider[] hit = Physics.OverlapSphere(vec, 4f,1<<(int)Layer.Player);

        if (hit != null)
        {
           
            Status tagetstatus = hit[0].GetComponent<Status>();
            tagetstatus.TakeDamage(status);
        }
    }
    void SkillStart()
    {
        RangeParticle.gameObject.SetActive(true);
    }
    void SkillAttack()
    {
        DamageParticle.gameObject.SetActive(true);
        Collider[] hit = Physics.OverlapSphere(DamageParticle.transform.position, 4.5f, 1 << (int)Layer.Player);

        
        RangeParticle.gameObject.SetActive(false);
        DamageParticle.gameObject.SetActive(false);
        if(hit != null)
        {
            Status status = hit[0].GetComponent<Status>();
            status.TakeDamage(GetComponentInParent<Status>());
        }
    }
    void BossSkill(){
        for(int i=1; i<bombSpawn.Length; i++){
            BombSlime = Managers.Pool.MakeObj(bombSlime);
            BombSlime.transform.position = bombSpawn[i].transform.position;
            BombSlime.transform.rotation = bombSpawn[i].transform.rotation;
            BombSlime.SetActive(true);
        }
    }
    void BossDie()
    {
        status.questReporter.Report();
        Managers.Resource.Destroy(gameObject);
    }
}
