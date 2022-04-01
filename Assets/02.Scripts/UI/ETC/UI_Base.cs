using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class UI_Base : MonoBehaviour
{

    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>(); // Ű��/ ���


    public abstract void Init(); // ��ӹ��� �ֵ��� ���Ŷ�

    private void Start()
    {
        Init();
        AddUIEvent(this.gameObject, (PointerEventData data) => Managers.Sound.Play("EffectSound/Button"));
    }
    protected void Bind<T>(Type type) where T : UnityEngine.Object // ������ �Լ�
    {
        string[] names = Enum.GetNames(type);            // Ÿ���� name���� �����ؼ� ���� 
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; //names�� ���̸�ŭ ������Ʈ �迭 ũ�� �Ҵ�
        _objects.Add(typeof(T), objects); // ��ųʸ� _objects�� Add(Ÿ��(ex Button) , ������Ʈ �迭)

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true); // ��ũ��Ʈ�� �޸� gameobject, �̸� , �ڽı��� ã���ǰ�
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true); // ��ũ��Ʈ�� �޸� gameobject, �̸� , �ڽı��� ã���ǰ�

            if (objects[i] == null)
            {
#if UNITY_EDITOR
                Debug.Log($"Failed to bind({names[i]})");
#endif

            }
        }
    }
    protected T Get<T>(int idx) where T : UnityEngine.Object //���̴� �Լ�
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false) // ��ųʸ��� ���� �����ö� TryGetValue
            return null; // �� �������°� ���������� return null;

        return objects[idx] as T; // TŸ������ ĳ����

    }

    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }


    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt= Util.GetOrAddComponent<UI_EventHandler>(go);
        
        switch(type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;

            case Define.UIEvent.OnPointer:
                evt.OnPointerHandler -= action;
                evt.OnPointerHandler += action;
                break;
            case Define.UIEvent.OnPointerExit:
                evt.OnPointerHandler -= action;
                evt.OnPointerHandler += action;
                break;
        }
    }

    
}
