using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTargetMarker : MonoBehaviour
{
    [SerializeField]
    private TaskTarget target;
    [SerializeField]
    private MarkerMaterialData[] markerMaterialDatas;

    private Dictionary<Quest,Task> targetTasksByQuest = new Dictionary<Quest, Task>();
    private Transform cameraTransform;
    private Renderer _renderer;
    private int currentRunningTargetTaskCount;

    private void Awake() {
        cameraTransform = Camera.main.transform;
        _renderer = GetComponent<Renderer>();
    }

    private void Start() {
        gameObject.SetActive(false);
        Managers.Quest.onQuestRegistered += TryAddTargetQuest;
        foreach(var quest in Managers.Quest.ActiveQuests){
            TryAddTargetQuest(quest);
        }        
    }

    private void Update() {
        var rotation = Quaternion.LookRotation((cameraTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 180f, 0f);
    }

    private void OnDestroy() {
        Managers.Quest.onQuestRegistered -= TryAddTargetQuest;
        foreach(KeyValuePair<Quest,Task>item in targetTasksByQuest){
            item.Key.onNewTaskGroup -= UpdateTargetTask;
            item.Key.onCompleted -= RemoveTargetQuest;
            item.Value.onStateChanged -= UpdateRunningTargetTaskCount;
        }
        // foreach ((Quest quest, Task task) in targetTasksByQuest){
        //     quest.onNewTaskGroup -= UpdateTargetTask;
        //     quest.onCompleted -= RemoveTargetQuest;
        //     task.onStateChanged -= UpdateRunningTargetTaskCount;
        // }
    }

    private void TryAddTargetQuest(Quest quest){
        if (target !=null && quest.ContainsTarget(target)){
            quest.onNewTaskGroup += UpdateTargetTask;
            quest.onCompleted += RemoveTargetQuest;

            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }

    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null){
        targetTasksByQuest.Remove(quest);//딕셔너리에 데이터가 있다면
        
        var task = currentTaskGroup.FindTaskByTarget(target);
        if(task != null){
            targetTasksByQuest[quest] = task;
            task.onStateChanged += UpdateRunningTargetTaskCount;
            UpdateRunningTargetTaskCount(task, task.State);
        }
    }

    private void RemoveTargetQuest(Quest quest) => targetTasksByQuest.Remove(quest);

    private void UpdateRunningTargetTaskCount(Task task, TaskState currentState, TaskState prevState = TaskState.Inactive){
        if(currentState == TaskState.Running){
            _renderer.material = markerMaterialDatas.First(x => x.category == task.Category).markerMaterial;
            currentRunningTargetTaskCount++;
        }
        else{
            currentRunningTargetTaskCount--;
        }
        gameObject.SetActive(currentRunningTargetTaskCount != 0);
    }

    [System.Serializable]
    private struct MarkerMaterialData{
        public Category category;
        public Material markerMaterial;
    }
}