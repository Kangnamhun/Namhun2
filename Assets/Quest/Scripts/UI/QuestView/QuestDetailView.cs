using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDetailView : MonoBehaviour
{
    [SerializeField]
    private GameObject displayGroup;
    [SerializeField]
    private Button cancelButton;
    
    [Header("Quest Description")]
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI description;

    [Header("Task Description")]
    [SerializeField]
    private RectTransform taskDescriptorGroup;
    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;
    [SerializeField]
    private int taskDescriptorPoolCount;

    [Header("Reward Descrption")]
    [SerializeField]
    private RectTransform rewardDescriptionGroup;
    [SerializeField]
    private TextMeshProUGUI rewardDescriptionPreafab;
    [SerializeField]
    private int rewardDescriptionPoolCount;

    private List<TaskDescriptor> taskDescriptorPool;
    private List<TextMeshProUGUI> rewardDescriptionPool;

    public Quest Target{get; private set;}

    private void Awake(){
        taskDescriptorPool = CreatePool(taskDescriptorPrefab, taskDescriptorPoolCount, taskDescriptorGroup);
        rewardDescriptionPool = CreatePool(rewardDescriptionPreafab, rewardDescriptionPoolCount, rewardDescriptionGroup);
        displayGroup.SetActive(false);
    }
    private void Start() {
        cancelButton.onClick.AddListener(CacnelQuest);
    }

    private List<T>CreatePool<T>(T prefab, int count, RectTransform parent) where T:MonoBehaviour{
        var pool = new List<T>(count);
        for(int i=0; i<count; i++){
            pool.Add(Instantiate(prefab, parent));
        }
        return pool;
    }

    private void CacnelQuest(){
        if(Target.IsCancelable){
            Target.Cancel();
        }
    }
    public void ShowTasks(Quest quest){
        int taskIndex = 0;
        foreach(var taskGroup in quest.TaskGroups){
            foreach(var task in taskGroup.Tasks){
                var poolObject = taskDescriptorPool[taskIndex++];
                poolObject.gameObject.SetActive(true);
                task.onStateChanged += OnTaskIsCompleted;
                if(task.IsComplete){
                    poolObject.UpdateTextUsingStrikeThrough(task);
                }
                else if(taskGroup == quest.CurrentTaskGroup){
                    poolObject.UpdateText(task);
                }
                else{
                    poolObject.UpdateText("● ? ? ? ? ? ? ? ? ?");
                }
            }
        }
        for(int i = taskIndex; i<taskDescriptorPool.Count; i++){//사용하지않는 poolObject는 끈다
            taskDescriptorPool[i].gameObject.SetActive(false);
        }
        var rewards = quest.Rewards;
        var rewardCount = rewards.Count;
        for(int i=0;i<rewardDescriptionPoolCount;i++){
            var poolObject = rewardDescriptionPool[i];
            if(i < rewardCount){
                var reward = rewards[i];
                poolObject.text = $"● {reward.Description} + {reward.Quantity}";
                poolObject.gameObject.SetActive(true);
            }
            else{
                poolObject.gameObject.SetActive(false);
            }
        }

        cancelButton.gameObject.SetActive(quest.IsCancelable && !quest.IsComplete);
    }
    private void OnTaskSuccessChanged(Quest quest, Task task, int currentSuccess, int prevSuccess)
        =>ShowTasks(quest);
    private void OnTaskIsCompleted(Task task, TaskState currentState, TaskState prevState) => ShowTasks(Target);

    public void Show(Quest quest){
        displayGroup.SetActive(true);
        if(Target != null){
            Target.onTaskSuccessChanged -= OnTaskSuccessChanged;
        }
        Target = quest;
        Target.onTaskSuccessChanged += OnTaskSuccessChanged;
        
        

        title.text = quest.DisplayName;
        description.text = quest.Description;
        ShowTasks(Target);
        // int taskIndex = 0;
        // foreach(var taskGroup in quest.TaskGroups){
        //     foreach(var task in taskGroup.Tasks){
        //         var poolObject = taskDescriptorPool[taskIndex++];
        //         poolObject.gameObject.SetActive(true);

        //         if(task.IsComplete){
        //             poolObject.UpdateTextUsingStrikeThrough(task);
        //         }
        //         else if(taskGroup == quest.CurrentTaskGroup){
        //             poolObject.UpdateText(task);
        //         }
        //         else{
        //             poolObject.UpdateText("● ? ? ? ? ? ? ? ? ?");
        //         }
        //     }
        // }

        
    }

    public void Hide(){
        Target = null;
        displayGroup.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }
}
