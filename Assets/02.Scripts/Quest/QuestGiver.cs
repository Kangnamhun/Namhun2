using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private Quest[] quests;

    public void QuestSet()
    {
        foreach (var quest in quests)
        {
            if (quest.IsAcceptable && !Managers.Quest.ContainsInCompleteQuests(quest) && !Managers.Quest.ContainsInActiveQuests(quest))
            {
                Managers.Quest.Register(quest);
            }
        }
    }
}