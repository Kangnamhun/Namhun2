using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;//디버깅용 함수 Attribute
using UnityEngine;

using Debug = UnityEngine.Debug;

public enum QuestState{
    Inactive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion
}

[CreateAssetMenu(menuName ="Quest/Quest",fileName ="Quest_")]
public class Quest : ScriptableObject
{
    #region  Event
    public delegate void TaskSuccesChangedHandler(Quest quest, Task task, int curentSuccess, int prevSuccess);
    public delegate void CompelteHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);
    #endregion
    public event TaskSuccesChangedHandler onTaskSuccessChanged;
    public event CompelteHandler onCompleted;
    public event CanceledHandler onCanceled;
    public event NewTaskGroupHandler onNewTaskGroup;
   
    [SerializeField]
    private Category category;
    [SerializeField]
    private Sprite icon;

    [Header("Text")]
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;
    [SerializeField,TextArea]
    private string description;

    [Header("Task")]
    [SerializeField]
    private TaskGroup[] taskGroups;

    [Header("Reward")]
    [SerializeField]
    private Reward[] rewards;

    [Header("Option")]
    [SerializeField]
    private bool useAutoComplete;
    [SerializeField]
    private bool isCancelable;
    [SerializeField]
    private bool isSaveable;

    [Header("Condition")]
    [SerializeField]
    private Condition[] acceptionConditions;
    [SerializeField]
    private Condition[] cancelConditions;

    private int currentTaskGroupIndex;

    public Category Category => category;
    public Sprite Icon => icon;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public string Description => description;

    public QuestState State {get; private set;}
    public TaskGroup CurrentTaskGroup => taskGroups[currentTaskGroupIndex];
    public IReadOnlyList<TaskGroup> TaskGroups => taskGroups;
    public IReadOnlyList<Reward> Rewards => rewards;
    
    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsCompletable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelable => isCancelable && cancelConditions.All(x => x.IsPass(this));
    public bool IsAcceptable => acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSaveable => isSaveable;

    public QuestState state{get; private set;}
    //슬라임 10마리 잡아라
    //or
    //레드 슬라임 10마리, 블루 슬라임 10마리 잡아라 처럼 Task가 여러개인 경우
    //or
    //슬라임 10마리 잡아라 => 레드 슬라임 10마리 잡기 처럼 슬라임 10마리 잡고 다음 Task가 나타나는 경우
    public void OnRegister(){
        Debug.Assert(!IsRegistered, "This Quest is already registered.");//인자로 들어온 값이 false라면 뒤에 문장을 Error로 띄워준다.
        //절대 일어나면 안되는 일이 일어났을 때 검출하기 위한 코드, 예상 가능한 버그가 일어났을 때 빠르게 잡기 위함
        //Debugging 코드라 성능에 전혀 영향 X
        foreach(var taskGroup in taskGroups){
            taskGroup.SetUp(this);
            foreach(var task in taskGroup.Tasks){
                task.onSuccessChanged += onSuccessChanged;
            }
        }
        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }
    public void ReceiveReport(string category, object target, int successCount){
        Debug.Assert(IsRegistered, "This Quest is already registered.");
        Debug.Assert(!IsCancel, "This Quest has been canceled.");
        if(IsComplete){
            return;
        }
        CurrentTaskGroup.ReceiveReport(category, target, successCount);
        if(CurrentTaskGroup.IsAllTaskComplete){
            if(currentTaskGroupIndex + 1 == taskGroups.Length){
                State = QuestState.WaitingForCompletion;
                if(useAutoComplete){
                    Complete();
                }
            }
            else{
                var prevTaskGroup = TaskGroups[currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else{
            State =QuestState.Running;
        }
    }
    public void Complete(){
        CheckIsRunning();
        foreach(var taskGroup in taskGroups){
            taskGroup.Compelte();
        }
        State = QuestState.Complete;
        foreach(var reward in rewards){
            reward.Give(this);
        }
        Managers.Sound.Play("EffectSound/QuestClear");
        onCompleted?.Invoke(this);
        onTaskSuccessChanged = null;
        onNewTaskGroup = null;
        onCompleted = null;
        onCanceled = null;
    }
    public virtual void Cancel(){
        CheckIsRunning();
        Debug.Assert(IsCancelable, "This Quest can't be canceled.");
        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    public bool ContainsTarget(object target) => taskGroups.Any(x => x.ContainsTarget(target));
    public bool ContainsTarget(TaskTarget target) => ContainsTarget(target.Value);

    public Quest Clone(){
        var clone = Instantiate(this);
        clone.taskGroups = taskGroups.Select(x=>new TaskGroup(x)).ToArray();
        return clone;
    }

    public QuestSaveData ToSaveData(){
        return new QuestSaveData{
            codeName = codeName,
            state = State,
            taskGroupIndex = currentTaskGroupIndex,
            taskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray()
        };
    }
    public void LoadFrom(QuestSaveData saveData){
        State = saveData.state;
        currentTaskGroupIndex = saveData.taskGroupIndex;
        for(int i=0; i<currentTaskGroupIndex;i++){
            var taskGroup = taskGroups[i];
            taskGroup.Start();
            taskGroup.Compelte();
        }
        for(int i=0;i<saveData.taskSuccessCounts.Length;i++){
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
        }
    }
    private void onSuccessChanged(Task task, int curentSuccess, int prevSuccess) 
        => onTaskSuccessChanged?.Invoke(this, task, curentSuccess,prevSuccess);
    [Conditional("UNITY_EDITOR")]//유니티 에디터로 개발중일때만 동작하는 함수
    private void CheckIsRunning(){
        Debug.Assert(IsRegistered, "This Quest is already registered.");
        Debug.Assert(!IsCancel, "This Quest has been canceld.");
        Debug.Assert(!IsComplete, "This Quest has already completed.");
    }
}