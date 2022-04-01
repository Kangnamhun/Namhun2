using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questTitleText;
    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;

    private Dictionary<Task,TaskDescriptor>taskDescritorsByTask = new Dictionary<Task, TaskDescriptor>();
    private Quest targetQuest;

    private void OnDestroy() {
        if(targetQuest != null){
            targetQuest.onNewTaskGroup -= UpdateTaskDescriptos;
            targetQuest.onCompleted -= DestorySelf;
        }
        foreach(var tuple in taskDescritorsByTask){
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }

    public void SetUp(Quest targetQuest, Color titleColor){
        this.targetQuest = targetQuest;
        questTitleText.text = targetQuest.Category == null ? 
                    targetQuest.DisplayName : $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";
        questTitleText.color = titleColor;
        targetQuest.onNewTaskGroup += UpdateTaskDescriptos;
        targetQuest.onCompleted += DestorySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptos(targetQuest,taskGroups[0],null);

        if(taskGroups[0] != targetQuest.CurrentTaskGroup){
            for(int i=1; i<taskGroups.Count; i++){
                var taskGroup = taskGroups[i];
                UpdateTaskDescriptos(targetQuest,taskGroup,taskGroups[i-1]);
                if(taskGroup == targetQuest.CurrentTaskGroup){
                    break;
                }
            }
        }
    }

    private void UpdateTaskDescriptos(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup){
        foreach(var task in currentTaskGroup.Tasks){
            var taskDescriptor = Instantiate(taskDescriptorPrefab, transform);
            taskDescriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;
            taskDescritorsByTask.Add(task, taskDescriptor);
        }
        if(prevTaskGroup != null){
                foreach(var task in prevTaskGroup.Tasks){
                    var taskDescriptor = taskDescritorsByTask[task];
                    taskDescriptor.UpdateTextUsingStrikeThrough(task);
                }
            }
    }

    private void UpdateText(Task task, int currentSuccess, int prevSuccess){
        taskDescritorsByTask[task].UpdateText(task);
    }
    private void DestorySelf(Quest quest){
        Destroy(gameObject);
    }
}
