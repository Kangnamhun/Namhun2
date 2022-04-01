using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementView : MonoBehaviour
{
    [SerializeField]
    private RectTransform achivementGroup;
    [SerializeField]
    private AchivementDetailView achivementDetailViewPrefab;

    private void Start(){
        var questSystem = Managers.Quest;
        CreateDetailView(questSystem.ActiveAchivements);
        CreateDetailView(questSystem.CompleteAchivements);

        gameObject.SetActive(false);
    }
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            gameObject.SetActive(false);
        }
    }
    private void CreateDetailView(IReadOnlyList<Quest> achivements){
        foreach(var achivement in achivements){
            Instantiate(achivementDetailViewPrefab, achivementGroup).Setup(achivement);
        }
    }
}
