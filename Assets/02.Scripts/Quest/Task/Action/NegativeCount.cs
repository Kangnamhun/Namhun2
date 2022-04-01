using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/Task/Action/NegativeCount",fileName ="Negative Count")]
public class NegativeCount : TaskAction
{
    public override int Run(Task task, int curentSuccess, int successCount)
    {
        return successCount < 0 ? curentSuccess - successCount : curentSuccess;
    }
}
