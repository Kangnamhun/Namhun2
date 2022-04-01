using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorFireBall : MonoBehaviour
{

    PlayerAttack playerAttack;
    Transform tr;
    GameObject target;
    ParticleSystem particleObject; //파티클시스템
    AudioSource audioSource;
    AudioClip audioClip;
    bool explosion;
    void Awake()
    {
        particleObject = GetComponent<ParticleSystem>();
        tr = GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Managers.Resource.Load<AudioClip>("Sounds/EffectSound/Attack/MagicSkill");
        audioClip = Managers.Resource.Load<AudioClip>("Sounds/EffectSound/Attack/Explosion");

    }
    private void OnEnable()
    {
        if (Managers.Game.GetPlayer() != null)
        {
            playerAttack = Managers.Game.GetPlayer().GetComponent<PlayerAttack>();
            target = playerAttack.attackTarget;
            audioSource.Play();
            
        }   
        explosion = false;

    }
    private void Update()
    {
        //if (target == null) return;
        if (target.layer != (int)Layer.Monster || explosion) return;


        tr.position = target.transform.position;
        if(particleObject.time >= 6.3f)
        {
            explosion = true;
            audioSource.Stop();
            audioSource.PlayOneShot(audioClip);
            Status status= playerAttack.attackTarget.GetComponent<Status>(); // 몬스터 status
            Status playerStatus = playerAttack.GetComponent<Status>();
            status.TakeDamage(playerStatus, playerAttack.skillRatio);
            status.TakeDamage(playerStatus, playerAttack.skillRatio);
            status.TakeDamage(playerStatus, playerAttack.skillRatio);

        }
    }
    private void OnDisable()
    {
    }
}
