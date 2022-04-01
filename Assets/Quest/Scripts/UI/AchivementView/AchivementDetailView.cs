using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchivementDetailView : MonoBehaviour
{
    [SerializeField]
    private Image achivementIcon;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private Image rewardIcon;
    [SerializeField]
    private TextMeshProUGUI rewardText;
    [SerializeField]
    private GameObject completionScreen;

    private Quest target;

        private void OnDestory(){
        if(target != null){
            target.onTaskSuccessChanged -= UpdateDescription;
            target.onCompleted -= ShowCompletionScreen;
        }
    }

    public void Setup(Quest achivement){
        target = achivement;
        achivementIcon.sprite = achivement.Icon;
        titleText.text = achivement.DisplayName;
        var task = achivement.CurrentTaskGroup.Tasks[0];
        description.text = BuildTaskDescription(task);

        var reward = achivement.Rewards[0];
        rewardIcon.sprite = reward.Icon;
        rewardText.text = $"{reward.Description} + {reward.Quantity}";

        if(achivement.IsComplete){
            completionScreen.SetActive(true);
        }
        else{
            completionScreen.SetActive(false);
            achivement.onTaskSuccessChanged += UpdateDescription;
            achivement.onCompleted += ShowCompletionScreen;
        }
    }

    private void UpdateDescription(Quest achivement, Task task, int currentSuccess, int prevSuccess)
        =>description.text = BuildTaskDescription(task);

    private void ShowCompletionScreen(Quest achivement) => completionScreen.SetActive(true);
    private string BuildTaskDescription(Task task) => $"{task.Description} {task.CurrentSuccess} / {task.NeedSuccessToComplete}";
}
