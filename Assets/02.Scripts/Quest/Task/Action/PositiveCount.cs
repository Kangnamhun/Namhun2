using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/Task/Action/PositiveCount",fileName ="Positive Count")]
public class PositiveCount : TaskAction
{
    public override int Run(Task task, int curentSuccess, int successCount)
    {
        return successCount > 0 ? curentSuccess + successCount : curentSuccess;
    }
}
