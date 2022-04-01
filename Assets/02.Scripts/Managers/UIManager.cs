using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    //sortorder
    int _order = 10; // 팝업이 아닌거랑 같아서 10으로
    //가장 나중에 실행된 팝업 먼저 지우기 -> stack

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); //gameobject대신 popup을 넣는 이유 -> 오브젝트는 컴퍼넌트 패턴형식이라 아무런 정보를 가지고 있지않기 때문
    UI_Scene _sceneUI = null;
    LinkedList<GameObject> _sceneLinkedList = new LinkedList<GameObject>();



    public bool isAction { get; private set; }
    public bool isTalk(bool isaction) => (isAction = isaction);

    #region SceneUISet
    public UI_Menu ui_Menu { get; private set; }
    public UI_Inventory ui_Inventory { get; private set; }
    public UI_Equipment ui_Equipment { get; private set; }
    public UI_Quest ui_Quest { get; private set; }
    public UI_MonsterHpBar ui_MonsterHpbar { get; private set; }
    public UI_ErrorText ui_ErrorText { get; private set; }
    public UI_PlayerData ui_PlayerData { get; private set; }
    public UI_Money ui_Money { get; private set; }
    #endregion

    public GameObject Root
    { get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            Object.DontDestroyOnLoad(root);
            return root;
        }
    }

      public void SetCanvas(GameObject go, bool sort = true)// 외부에서 팝업이 켜질때 셋캔버스 요청 -> order로 우선순위 채워달라
      { 
        Canvas canvas= Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // 중첩캔버스에서 부모가 어떤 값을 따르던 본인의 소팅오더를 받는다

        if(sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else // 팝업이 아니면
        {
            canvas.sortingOrder = 0;
        }
    }
    public void AddLinkedList(GameObject go) //esc 눌렀을때 순서대로 제어 하기위해 linkedlist 사용
    {
        _sceneLinkedList.AddLast(go);
        go.GetComponentInParent<Canvas>().sortingOrder = _sceneLinkedList.Count;
    }
    public void RemoveLinkedList(GameObject go)
    {
        if (_sceneLinkedList.Count == 0) return;
        _sceneLinkedList.Remove(go);
    }
    public void CloseScene()
    {
        if (_sceneLinkedList.Count == 0) return;
        _sceneLinkedList.Last.Value.gameObject.SetActive(false);
        _sceneLinkedList.RemoveLast();
     

    }
    public bool StateLinkedList() // 현재 켜진 팝업이 있다면
    {
        if (_sceneLinkedList.Count != 0)
        {
            return true;
        }
        else
            return false;
    }
    public T MakeWorldSpaceUI<T>(Transform parent = null,string name =null) where T : UI_Base// 이름과 T를 따로 받는 이유 ->name -> prefabs 연동을 위해  // T는  타입
    {
        if (string.IsNullOrEmpty(name)) //이름이 비어있다면 프리팹타입의 name 받아와서 넣어주기
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}"); // 프리팹 소환
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode= RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);

    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene// 이름과 T를 따로 받는 이유 ->name -> prefabs 연동을 위해  // T는  타입
    {
        if (string.IsNullOrEmpty(name)) //이름이 비어있다면 프리팹타입의 name 받아와서 넣어주기
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}"); // 프리팹 소환
        T sceneUI = Util.GetOrAddComponent<T>(go); // 스크립트 넣어주기
        _sceneUI = sceneUI; 

        go.transform.SetParent(Root.transform); //부모 지정해서 한번에 관리

        //showpopupui에서 오더 관리 안 해주는 이유 -> 원래 생성되어있던 ui를 컨트롤할때 카운터가 안 된다.
        return sceneUI;
    }
    public T ShowPopupUI<T>(bool movestop = true,string name = null) where T :UI_Popup// 이름과 T를 따로 받는 이유 ->name -> prefabs 연동을 위해  // T는  타입
    {
        if (string.IsNullOrEmpty(name)) //이름이 비어있다면 프리팹타입의 name 받아와서 넣어주기
            name = typeof(T).Name;

        GameObject go= Managers.Resource.Instantiate($"UI/PopUp/{name}"); // 프리팹 소환
        T popup =  Util.GetOrAddComponent<T>(go); // 스크립트 넣어주기
        _popupStack.Push(popup); // 스택에 넣어주기
       // go.GetComponent<Canvas>().sortingOrder = _popupStack.Count;
        go.transform.SetParent(Root.transform); //부모 지정해서 한번에 관리


        isAction = movestop; // 이동 불가능


        //showpopupui에서 오더 관리 안 해주는 이유 -> 원래 생성되어있던 ui를 컨트롤할때 카운터가 안 된다.
        return popup;
    }
    public bool StatePopupUI() // 현재 켜진 팝업이 있다면
    {
        if (_popupStack.Count != 0)
        {
            return true;
        }
        else
            return false;
    }
    public void ClosePopupUI(UI_Popup popup) // 좀 더 안전하다
    {
        if (_popupStack.Count == 0) //스택 건드릴때 카운트 체크하기
            return;

        if(_popupStack.Peek() !=popup) // 마지막으로 담은게 popup이 아니라면
        {
#if UNITY_EDITOR
            Debug.Log("Close popup failed!");
#endif
            return;
        }
        ClosePopupUI();
    }
    
    public void ClosePopupUI()
    {

        //스택 추출해서 닫기
        if (_popupStack.Count == 0) //스택 건드릴때 카운트 체크하기
            return;
       UI_Popup popup = _popupStack.Pop(); // 뽑아오기
        Managers.Resource.Destroy(popup.gameObject); // 뽑은 후 지우기
        popup = null; // 혹시 모르니 null 또 접근할까봐
        _order--;

        if (!StatePopupUI())
        {
            isAction = false; // 이동가능
        }
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void SetSceneUI()
    {
        ui_Menu = Managers.UI.ShowSceneUI<UI_Menu>();
        ui_ErrorText = Managers.UI.ShowSceneUI<UI_ErrorText>();
        ui_Inventory = Managers.UI.ShowSceneUI<UI_Inventory>();
        ui_MonsterHpbar = Managers.UI.ShowSceneUI<UI_MonsterHpBar>();
        ui_Equipment = Managers.UI.ShowSceneUI<UI_Equipment>();
        ui_PlayerData = Managers.UI.ShowSceneUI<UI_PlayerData>();
        ui_Money = Managers.UI.ShowSceneUI<UI_Money>();

        Managers.UI.ShowSceneUI<UI_QuickSlot>();
      
        ui_Quest = Managers.UI.ShowSceneUI<UI_Quest>();
        //ui_Quest.Init();

    }





}
