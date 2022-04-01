using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> quests;

    public IReadOnlyList<Quest> Quests => quests;
    public Quest FindQuestBy(string codeName) => quests.FirstOrDefault(x => x.CodeName == codeName);

#if UNITY_EDITOR
    [ContextMenu("FindQuests")]
    private void FindQuests(){
        FindQuestsBy<Quest>();
    }
    [ContextMenu("FindAchivements")]
    private void FindAchivements(){
        FindQuestsBy<Achivement>();
    }
    private void FindQuestsBy<T>() where T: Quest{
        quests = new List<Quest>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");//FindAsset = Asset폴더에서 필터에 맞는 Asset의 GUID를 불러오는 함수
        //GUID란? 유니티가 Asset을 관리하기 위해서 내부적으로 사용하는 ID
        foreach(var guid in guids){
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if(quest.GetType() == typeof(T)){
                //if문으로 type을 체크하는 이유 : 만약에 T가 QuestClass라면 Achivement가 Quest를 상속받고 있기 때문에 
                //위의 FindAssets에서 Quest객체와 Achivement객체를 모두 가져오기 때문에 한 번 더 확인하는 것.
                quests.Add(quest);
            }
            EditorUtility.SetDirty(this);//SetDirty ? QuestDataBase객체가 가진 Serialize 변수가 변화가 생겼으니 Asset을 저장할 때 반영하라른 의미.
            AssetDatabase.SaveAssets();
        }
    }
#endif
}