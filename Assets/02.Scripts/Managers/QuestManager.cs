using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestManager
{
    #region Save Path
    private const string kSaveRootPath = "questSystem";
    private const string kActiveQuestsSavePath = "activeQuests";
    private const string kCompletedQuestsSavePath = "completedQuests";
    private const string kActiveAchivementsSavePath = "activeAchivements";
    private const string kCompletedAchivementsSavePath = "completedAchivements";
    #endregion
    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion
    private static bool isApplicationQuitting;


    private List<Quest> activeQuests = new List<Quest>();
    private List<Quest> completedQuests = new List<Quest>();

    private List<Quest> activeAchivements = new List<Quest>();
    private List<Quest> completeAchivements = new List<Quest>();

    private QuestDatabase questDatabase;
    private QuestDatabase achivementDatabase;

    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    public event QuestRegisteredHandler onAchivementRegistered;
    public event QuestCompletedHandler onAchivementCompleted;
    //public event QuestCanceledHandler onAchivementCanceled;

    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;

    public IReadOnlyList<Quest> ActiveAchivements => activeAchivements;
    public IReadOnlyList<Quest> CompleteAchivements => completeAchivements;

    private void Awake() 
    {


        questDatabase = Resources.Load<QuestDatabase>("Quest Database");
        achivementDatabase = Resources.Load<QuestDatabase>("Achivement Database");
        if(!Load()){
            foreach(var achivement in achivementDatabase.Quests){
                Register(achivement);
            }
        }
    }


    public Quest Register(Quest quest){
        var newQuest = quest.Clone();
        if(newQuest is Achivement){
            newQuest.onCompleted += OnAchivementCompleted;
            activeAchivements.Add(newQuest);

            newQuest.OnRegister();
            onAchivementRegistered?.Invoke(newQuest);
        }
        else{
            newQuest.onCompleted += OnQuestCompleted;
            newQuest.onCanceled += OnQuestCanceled;

            activeQuests.Add(newQuest);

            newQuest.OnRegister();
            onQuestRegistered?.Invoke(newQuest);
        }
        return newQuest;
    }
    public void ReceiveReport(string category, object target, int successCount){
        ReceiveReport(activeQuests, category, target, successCount);
        ReceiveReport(activeAchivements, category, target, successCount);
    }
    public void ReceiveReport(Category category, TaskTarget target, int successCount)
        => ReceiveReport(category.CodeName, target.Value, successCount);
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount){
        foreach(var quest in quests.ToArray()){
        //ToArray로 List의 사본을 만들어서 for문을 돌리는 이유는 for문이 돌아가는 와중에
        //퀘스트가 완료되면 목록에서 빠질 수도 있기 때문에 사본으로 돌림
            quest.ReceiveReport(category, target, successCount);
        }
    }

    public void CompleteWaitingQuests(){
        foreach(var quest in activeQuests.ToList()){
            if(quest.IsCompletable){
                quest.Complete();
            }
        }
    }
    public bool ContainsInActiveQuests(Quest quest) => activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompleteQuests(Quest quest) => completedQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchivements(Quest quest) => activeAchivements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedAchivements(Quest quest) => completeAchivements.Any(x => x.CodeName == quest.CodeName);

    public void Save(){
        var root = new JObject();
        root.Add(kActiveQuestsSavePath, CreateSaveDatas(activeQuests));
        root.Add(kCompletedQuestsSavePath, CreateSaveDatas(completedQuests));
        root.Add(kActiveAchivementsSavePath, CreateSaveDatas(activeAchivements));
        root.Add(kCompletedAchivementsSavePath, CreateSaveDatas(completeAchivements));

        PlayerPrefs.SetString(kSaveRootPath, root.ToString());
        PlayerPrefs.Save();
    }

    public bool Load(){
        if(PlayerPrefs.HasKey(kSaveRootPath)){
            var root = JObject.Parse(PlayerPrefs.GetString(kSaveRootPath));
            LoadSaveDatas(root[kActiveQuestsSavePath], questDatabase, LoadActiveQuest);
            LoadSaveDatas(root[kCompletedQuestsSavePath], questDatabase, LoadCompletedQuest);
            LoadSaveDatas(root[kActiveAchivementsSavePath], achivementDatabase, LoadActiveQuest);
            LoadSaveDatas(root[kCompletedAchivementsSavePath], achivementDatabase, LoadCompletedQuest);
            return true;
        }
        else{
            return false;
        }        
    }

    private JArray CreateSaveDatas(IReadOnlyList<Quest>quests){
        var saveDatas = new JArray();
        foreach(var quest in quests){
            if(quest.IsSaveable){
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));//SaveData를 Json형태로 변환한 후 Json Array에 담는것.
            }
        }
        return saveDatas;
    }
    private void LoadSaveDatas(JToken datasToken, QuestDatabase database, System.Action<QuestSaveData, Quest>onSuccess){
        var datas = datasToken as JArray;//datasToken이란 CreateSaveDatas에서 만들어진 SaveData가 저장되었다가 Load할 때 이 함수로 들어온다.
        foreach(var data in datas){
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveData.codeName);
            onSuccess.Invoke(saveData, quest);
        }
    }
    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }
    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest){
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);
        if(newQuest is Achivement){
            completeAchivements.Add(newQuest);
        }
        else{
            completedQuests.Add(newQuest);
        }
    }

    #region Callback
    private void OnQuestCompleted(Quest quest){
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }
    private void OnQuestCanceled(Quest quest){
        activeQuests.Remove(quest);
        onQuestCanceled?.Invoke(quest);

      //  Managers.Resource.Destroy(quest, Time.deltaTime);
    }
    private void OnAchivementCompleted(Quest achivement){
        activeAchivements.Remove(achivement);
        completeAchivements.Add(achivement);

        onAchivementCompleted?.Invoke(achivement);
    }
    #endregion
}