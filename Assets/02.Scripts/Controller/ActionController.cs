using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    public bool nearNPC = false;
    [SerializeField]
    public GameObject scanNPC { get; private set; }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Npc"))
        {
            nearNPC = true;
            scanNPC = other.gameObject;
        }
        else return;
    }

    public void OnTriggerExit(Collider other)
    {

        nearNPC = false;
        scanNPC = null;
    }
    void Update()
    {
        if (Managers.Input.talking && scanNPC != null && !Managers.UI.isAction)
        {
            Managers.talk.Action(scanNPC);
        }
    }


}
