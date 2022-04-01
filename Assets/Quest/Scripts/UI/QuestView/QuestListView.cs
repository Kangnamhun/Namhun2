using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class QuestListView : MonoBehaviour
{
    public static QuestListView ins;
    [SerializeField]
    private TextMeshProUGUI elementTextPrefab;
    [SerializeField]
    private Dictionary<Quest,GameObject> elementByQuest = new Dictionary<Quest, GameObject>();
    [SerializeField]
    private ToggleGroup toggleGroup;

    private void Awake() {
        ins = this;
        toggleGroup = GetComponent<ToggleGroup>();
    }
    //public void Init()
    //{
    //    toggleGroup = GetComponent<ToggleGroup>();
    //}
    public void AddElement(Quest quest, UnityAction<bool> onClicked){
        var element = Instantiate(elementTextPrefab, transform);
        element.text = quest.DisplayName;

        var toggle = element.GetComponent<Toggle>();
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(onClicked);

        elementByQuest.Add(quest, element.gameObject);
    }

    public void RemoveElement(Quest quest){
        Destroy(elementByQuest[quest]);
        elementByQuest.Remove(quest);
    }
}
