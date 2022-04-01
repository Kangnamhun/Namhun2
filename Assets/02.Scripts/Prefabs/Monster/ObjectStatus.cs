using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStatus : Status
{
    BombSlime bombSlime;
    protected override void Init()
    {
        bombSlime = GetComponent<BombSlime>();
        base.Init();
    }
    protected override void Die()
    {
        Managers.Resource.Instantiate("Explosion").transform.position = transform.position;
        bombSlime.ExplosionTime = 0f;
        bombSlime.BombColor.material.color = bombSlime.baseColor;
        gameObject.SetActive(false);
    }
    public override void TakeDamage(Status attacker, float ratio = 1)
    {
        if (BDeath) return; //만약 사망했다면


        int damage = Random.Range( (int)(attacker.attack / 10),  (int)((attacker.attack- (int)defense) * ratio));
        Hp -= damage;
    }
}
