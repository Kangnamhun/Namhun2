using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class GameManager 
{

    GameObject _player;
    public string _name { get; private set; }
    public GameObject GetPlayer() { return _player; }


    public void SetName(string name) {  _name = name;  }
    
    public GameObject Spawn(string path, Transform parent = null)
    {
       
        GameObject go = Managers.Resource.Instantiate(path);
        
        _player = go;

        return go;
    }

    public void OnUpdate()
    {

        if (Managers.Input.escape)
        {


            if (Managers.UI.StatePopupUI())
            {
                Managers.UI.ClosePopupUI();


            }
            else if (Managers.UI.StateLinkedList())
            {
                Managers.UI.CloseScene();
            }
            else
            {
                Managers.UI.ui_Menu.OpenMenu();
            }
        }


        if (Managers.UI.isAction) return;


        if (Managers.Input.equip) //u를 누를시
        {
            Debug.Log("장비창 열기");
            Managers.UI.ui_Equipment.OnOffEquipment();

   
        }
        if (Managers.Input.inven)
        {
            Managers.UI.ui_Inventory.OnOffInventory();

        }
        if(Managers.Input.quest)
        {
            Debug.Log("Quest 오픈");
           
            Managers.UI.ui_Quest.OpenQuest();

        }
        
    }

}
