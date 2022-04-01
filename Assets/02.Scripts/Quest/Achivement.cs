using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Achivement", fileName = "Achivement_")]
public class Achivement : Quest
{
    public override bool IsCancelable => false;
    public override bool IsSaveable => true;
    public override void Cancel()
    {
        Debug.LogAssertion("Achivement can't be canceled.");
    }
}