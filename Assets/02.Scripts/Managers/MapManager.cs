using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private BoxCollider box;

    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

   
    void Update()
    {
       
    }


    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.position = new Vector3(0, 1, 0);
    }
}
