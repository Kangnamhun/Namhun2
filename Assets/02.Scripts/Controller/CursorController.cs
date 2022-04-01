using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Layer
{
	Item = 6,
	Monster = 7,
	Npc = 8,
	Ground = 9,
	Player = 12


}
public class CursorController : MonoBehaviour
{
	
	int _mask = (1 << (int)Layer.Npc) | (1 << (int)Layer.Monster) | (1 << (int)Layer.Ground) | (1 << (int)Layer.Item);

	Texture2D _attackIcon;
	Texture2D _handIcon;
	Texture2D _talkIcon;
    Texture2D _takeItem;

    enum CursorType
	{
		None,
		Attack,
		Hand,
		Talk,
        TakeItem
    }

	CursorType _cursorType = CursorType.None;

	void Start()
	{
		_attackIcon = Managers.Resource.Load<Texture2D>("Cursor/Attack");
		_handIcon = Managers.Resource.Load<Texture2D>("Cursor/Hand");
		_talkIcon = Managers.Resource.Load<Texture2D>("Cursor/TalkNPC");
        _takeItem = Managers.Resource.Load<Texture2D>("Cursor/TakeItem");
    }

	void Update()
	{
		
		if (Input.GetMouseButton(0))
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

	

		if (Physics.Raycast(ray, out hit, 100.0f, _mask))
		{
			if (IsPointerOverUIObject())
			{
				if (_cursorType != CursorType.Hand)
				{
					ChangeCursor(_handIcon, CursorType.Hand);
				}
				return;
			}
			if (hit.collider.gameObject.layer == (int)Layer.Monster)
			{
				if (_cursorType != CursorType.Attack)
				{
					ChangeCursor(_attackIcon, CursorType.Attack);

				}
			}

			else if(hit.collider.gameObject.layer ==(int)Layer.Npc)
			{
				if (_cursorType != CursorType.Talk)
				{

					ChangeCursor(_talkIcon, CursorType.Talk);
				}
			}
            else if (hit.collider.gameObject.layer == (int)Layer.Item)
            {
                if (_cursorType != CursorType.TakeItem)
                {
					ChangeCursor(_takeItem, CursorType.TakeItem);
                }
            }
            else
            {
				if (_cursorType != CursorType.Hand)
				{
					ChangeCursor(_handIcon, CursorType.Hand);
				}
			}
		}
	}

	private void ChangeCursor(Texture2D cursor, CursorType cursorType)
    {
		Cursor.SetCursor(cursor, new Vector2(cursor.width / 3, 0), CursorMode.Auto);
		_cursorType = cursorType;
	}

	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
