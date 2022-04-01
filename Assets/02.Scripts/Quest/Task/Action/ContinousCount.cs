using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/ContinousCount", fileName = "Continous Count")]
public class ContinousCount : TaskAction
{
    public override int Run(Task task, int curentSuccess, int successCount)
    {
        return successCount > 0 ? curentSuccess + successCount : 0;
    }
}
