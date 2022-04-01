using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using TMPro;
using UnityEngine;

public class QuestCompletionNotifier : MonoBehaviour
{
    [SerializeField]
    private string titleDescription;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI rewardText;
    [SerializeField]
    private float showTime = 3f;

    private ConcurrentQueue<Quest> reservedQuests = new ConcurrentQueue<Quest>();
    private StringBuilder stringBuilder = new StringBuilder();

    private void Start(){
        var questSystem = Managers.Quest;
        questSystem.onQuestCompleted += Notify;
        questSystem.onAchivementCompleted += Notify;

        gameObject.SetActive(false);
    }
    //private void OnDestory(){
    //    var questSystem = QuestSystem.Instance;
    //    if(questSystem != null){
    //        questSystem.onQuestCompleted -= Notify;
    //        questSystem.onAchivementCompleted -= Notify;
    //    }
    //}

    private void Notify(Quest quest){
        reservedQuests.Enqueue(quest);
        if(!gameObject.activeSelf){
            gameObject.SetActive(true);
            StartCoroutine("ShowNotice");
        }
    }

    private IEnumerator ShowNotice(){
        var waitSeconds = new WaitForSeconds(showTime);

        Quest quest;
        while(reservedQuests.TryDequeue(out quest)){
            titleText.text = titleDescription.Replace("%{dn}", quest.DisplayName);
            //dn이란?Mark
            //나중에 inspector창으로 titleDescription을 설정해 줄 때 이 마크를 입력해서 Quest의 Title이
            //문자열 어디에 출력될지 동적으로 결정,text로 어떤 정보를 보여줄 때 가장 일반적으로 쓰이는 방식
            foreach(var reward in quest.Rewards){
                stringBuilder.Append(reward.Description);
                stringBuilder.Append(" ");
                stringBuilder.Append(reward.Quantity);
                stringBuilder.Append(" ");
                //stringbuilder를 이용해서 문자열을 만드는 이유는 for문으로 문자열을 합쳐야 하는 경우
                //그냥 문자열 더하기 연산을 하면 성능에 굉장히 안좋다.
            }
            rewardText.text = stringBuilder.ToString();
            stringBuilder.Clear();

            yield return waitSeconds;
        }
        gameObject.SetActive(false);
    }
}
