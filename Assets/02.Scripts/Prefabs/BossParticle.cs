using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticle : MonoBehaviour
{
    public Status attackerStatus;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
         Collider[] hit = Physics.OverlapSphere(transform.position, 4.5f, 1 << (int)Layer.Player);

        attackerStatus = GetComponentInParent<Status>();
        for (int i = 0; i < hit.Length; i++)
        {
            Status status = hit[i].GetComponent<Status>();
            
            status.TakeDamage(attackerStatus);
        }
    }
}
