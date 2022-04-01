using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskState{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(fileName = "Task_", menuName = "Quest/Task/Task")]
public class Task : ScriptableObject {
    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    //State가 변할 때 마다 알려주는 이벤트,UI Update Code를 Event에 연결하면 Task의 상태를 Update에서 추적할 필요 없이
    //상태가 바뀌면 알아서 UI가 Update되는 방식
    public delegate void SuccessChangedHandler(Task task, int curentSuccess, int prevSuccess);
    #endregion
    [SerializeField]
    private Category category;
    [Header("Text")]
    [SerializeField]
    private string codeName;//외부에 보이는 것이 아닌 프로그래머가 검색하기 위한 내부적으로 사용하는 이름,데이터를 비교할 때 많이 사용
    [SerializeField]
    private string description;
    [Header("Action")]
    [SerializeField]
    private TaskAction action;
    [Header("Target")]
    [SerializeField]
    private TaskTarget[] targets;
    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue initialSuccessValue;

    private TaskState state;
    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    [SerializeField]
    private int needSuccessToComplete;
    [SerializeField]
    private bool canReceiveReportDuringCompletion;
    //퀘스트가 완료되어도 계속 성공횟수를 보고 받을 것인가?
    //아이템 100개 모으기 퀘스트 경우 100개 모으고 유저가 50개를 버렸다면 다시 완료할 수 없는 상태로 돌아가야 하기 때문에
    private int curentSuccess;
    public int CurrentSuccess{
        get => curentSuccess;
        set{
            int prevSuccess = curentSuccess;
            curentSuccess = Mathf.Clamp(value, 0, needSuccessToComplete);
            if(curentSuccess != prevSuccess){
                State = curentSuccess == needSuccessToComplete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, curentSuccess, prevSuccess);
            }
        }
    }
    public Category Category => category;
    public string CodeName => codeName;
    public string Description => description;
    public int NeedSuccessToComplete => needSuccessToComplete;
    public Quest Owner{get; private set;}
    public void SetUp(Quest owner){
        Owner = owner;
    }
    public void Start() {
        State = TaskState.Running;
        if(initialSuccessValue){
            curentSuccess = initialSuccessValue.GetValue(this);
        }
    }
    public void End(){
        onSuccessChanged = null;
        onStateChanged = null;
    }
    public TaskState State{
        get => state;
        set{
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
            //=> ?.은 onStateChanged가 null이면 null을 반환하고 아니면 뒤에 함수를 실행해라.
            //=> 이 변수에 연결된 이벤트가 없어서 null이면 아무일이 안일어나고 있으면 Invoke 함수가 실행되서 이벤트가 실행
        }
    }
    public bool IsComplete => State == TaskState.Complete;//Task가 Complete인지 확인
    public void ReceiveReport(int successCount){
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }
    public void Complete(){
        curentSuccess = needSuccessToComplete;
    }
    public bool IsTarget(string category, object target) => Category == category && 
                                                            targets.Any(x=>x.IsEqual(target)) &&
                                                            (!IsComplete || (IsComplete && canReceiveReportDuringCompletion));
    //TaskTarget을 통해 Task가 성공 횟수를 보고 받을 대상인지 확인하는 함수
    //시스템에 보고된 Category와 Target이 내가 가진 것들과 같아야 True를 리턴

    public bool ContainsTarget(object target) => targets.Any(x => x.IsEqual(target));
} 