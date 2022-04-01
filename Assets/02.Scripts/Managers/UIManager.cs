using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    //sortorder
    int _order = 10; // �˾��� �ƴѰŶ� ���Ƽ� 10����
    //���� ���߿� ����� �˾� ���� ����� -> stack

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); //gameobject��� popup�� �ִ� ���� -> ������Ʈ�� ���۳�Ʈ ���������̶� �ƹ��� ������ ������ �����ʱ� ����
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

      public void SetCanvas(GameObject go, bool sort = true)// �ܺο��� �˾��� ������ ��ĵ���� ��û -> order�� �켱���� ä���޶�
      { 
        Canvas canvas= Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // ��øĵ�������� �θ� � ���� ������ ������ ���ÿ����� �޴´�

        if(sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else // �˾��� �ƴϸ�
        {
            canvas.sortingOrder = 0;
        }
    }
    public void AddLinkedList(GameObject go) //esc �������� ������� ���� �ϱ����� linkedlist ���
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
    public bool StateLinkedList() // ���� ���� �˾��� �ִٸ�
    {
        if (_sceneLinkedList.Count != 0)
        {
            return true;
        }
        else
            return false;
    }
    public T MakeWorldSpaceUI<T>(Transform parent = null,string name =null) where T : UI_Base// �̸��� T�� ���� �޴� ���� ->name -> prefabs ������ ����  // T��  Ÿ��
    {
        if (string.IsNullOrEmpty(name)) //�̸��� ����ִٸ� ������Ÿ���� name �޾ƿͼ� �־��ֱ�
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}"); // ������ ��ȯ
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode= RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);

    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene// �̸��� T�� ���� �޴� ���� ->name -> prefabs ������ ����  // T��  Ÿ��
    {
        if (string.IsNullOrEmpty(name)) //�̸��� ����ִٸ� ������Ÿ���� name �޾ƿͼ� �־��ֱ�
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}"); // ������ ��ȯ
        T sceneUI = Util.GetOrAddComponent<T>(go); // ��ũ��Ʈ �־��ֱ�
        _sceneUI = sceneUI; 

        go.transform.SetParent(Root.transform); //�θ� �����ؼ� �ѹ��� ����

        //showpopupui���� ���� ���� �� ���ִ� ���� -> ���� �����Ǿ��ִ� ui�� ��Ʈ���Ҷ� ī���Ͱ� �� �ȴ�.
        return sceneUI;
    }
    public T ShowPopupUI<T>(bool movestop = true,string name = null) where T :UI_Popup// �̸��� T�� ���� �޴� ���� ->name -> prefabs ������ ����  // T��  Ÿ��
    {
        if (string.IsNullOrEmpty(name)) //�̸��� ����ִٸ� ������Ÿ���� name �޾ƿͼ� �־��ֱ�
            name = typeof(T).Name;

        GameObject go= Managers.Resource.Instantiate($"UI/PopUp/{name}"); // ������ ��ȯ
        T popup =  Util.GetOrAddComponent<T>(go); // ��ũ��Ʈ �־��ֱ�
        _popupStack.Push(popup); // ���ÿ� �־��ֱ�
       // go.GetComponent<Canvas>().sortingOrder = _popupStack.Count;
        go.transform.SetParent(Root.transform); //�θ� �����ؼ� �ѹ��� ����


        isAction = movestop; // �̵� �Ұ���


        //showpopupui���� ���� ���� �� ���ִ� ���� -> ���� �����Ǿ��ִ� ui�� ��Ʈ���Ҷ� ī���Ͱ� �� �ȴ�.
        return popup;
    }
    public bool StatePopupUI() // ���� ���� �˾��� �ִٸ�
    {
        if (_popupStack.Count != 0)
        {
            return true;
        }
        else
            return false;
    }
    public void ClosePopupUI(UI_Popup popup) // �� �� �����ϴ�
    {
        if (_popupStack.Count == 0) //���� �ǵ帱�� ī��Ʈ üũ�ϱ�
            return;

        if(_popupStack.Peek() !=popup) // ���������� ������ popup�� �ƴ϶��
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

        //���� �����ؼ� �ݱ�
        if (_popupStack.Count == 0) //���� �ǵ帱�� ī��Ʈ üũ�ϱ�
            return;
       UI_Popup popup = _popupStack.Pop(); // �̾ƿ���
        Managers.Resource.Destroy(popup.gameObject); // ���� �� �����
        popup = null; // Ȥ�� �𸣴� null �� �����ұ��
        _order--;

        if (!StatePopupUI())
        {
            isAction = false; // �̵�����
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
