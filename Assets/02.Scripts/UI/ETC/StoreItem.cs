using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class StoreItem : UI_Base  
{
    enum Images
    {
        ItemImage
    }
    enum Texts
    {
        ItemName,
        CoinPrice,
        ItemDescrip
    }

    #region sigletone
    public static StoreItem ins;
    private void Awake()
    {
        ins = this;
    }
    #endregion

    bool bInit;

    Image itemImage;
    Text itemName;
    [HideInInspector]
    public Text itemDescrip;
    public Text coinPrice;

    [HideInInspector]
    public ItemData itemData;

    UI_Message ui_Message;
    public System.Action<ItemData> on;

    public override void Init()
    {
        if (bInit) return;
        bInit = true;


        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        itemImage = GetImage((int)Images.ItemImage);

        itemName = GetText((int)Texts.ItemName);
        itemDescrip = GetText((int)Texts.ItemDescrip);
        coinPrice = GetText((int)Texts.CoinPrice);

    }

    public void SetItem(int _itemcode, int _itemCount, System.Action<ItemData> _on)
    {
        gameObject.SetActive(true);
        //itemcode -> 아이템을 세팅.
        itemData = new ItemData(_itemcode, _itemCount);

        itemImage.sprite = ItemInfo.ins.GetItemInfoSpriteIcon(itemData.itemcode);


        itemName.text = itemData.itemName;
        itemDescrip.text = itemData.iteminfoBase.description;
        coinPrice.text = itemData.gamecost.ToString();

        on = _on; //함수 담기
      
    }
    public void OnClickStoreItem(ItemData _itemData) //아이템 사기
    {
        PlayerStatus _playerStatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
#if UNITY_EDITOR
        Debug.Log("보유머니" + _playerStatus.gold + "템가격 : " + _itemData);
#endif
        //Debug.Log("@@@ 아이템 구매버튼"+_itemData.itemcode);

        if (_playerStatus.gold < _itemData.gamecost)//만약 플레이어가 가지고있는 골드가 아이템금액 적으면
        {
            Managers.UI.ui_ErrorText.SetErrorText(Define.Error.NoneGold);
        }
        else
        {
            bool _bGet = Managers.UI.ui_Inventory.AddItemData(_itemData);//인벤토리에 넣어주기
            if (_bGet)//아이템창이 꽉차서 구매를 못할경우
            {
                //Debug.Log("@@@ 보유머니 = 보유머니 - 아이템가격");
                //ui_Message.ShowMessage("아이템 구매", _itemData.itemName + "을" + _itemData.itemCount + "개 구매했습니다");
                
                _playerStatus.gold -= _itemData.gamecost * _itemData.itemCount; //상점에서 아이템 구매시 보유골드 빼주는거
            }
            else
            {
                Managers.UI.ui_ErrorText.SetErrorText(Define.Error.MaxInv);

            }
            Managers.UI.ClosePopupUI(ui_Message);
        }
      
    }

    public void ItemClick(PointerEventData eventData) // 아이템 더블클릭 했을때 소비템이면 개수 / 장비라면 그냥 구매하겠느냐 출력
    {
        int clickCount = eventData.clickCount;
        if (clickCount == 2)//두번 클릭시
        {
           if(on != null)
            {
                //여기서 showmessage 호출해줘야함
                ui_Message = Managers.UI.ShowPopupUI<UI_Message>();
                ui_Message.Init();
                ui_Message.okButton.gameObject.AddUIEvent(BuyOk);

                if (itemData.itemcode / 10000 == 1)
                {
                    ui_Message.countSlider.gameObject.SetActive(true);
                }
                else if (itemData.itemcode / 10000 == 2)
                {
                    ui_Message.ShowMessage("구매", itemData.itemName + "를 구매하겠습니까?");
                }
            }
        }
    }

    public void BuyOk(PointerEventData data)
    {
        if (on != null)
        {
            Managers.Sound.Play("EffectSound/Buy");
            itemData.itemCount = (int)ui_Message.countSlider.value; // 슬라이더 값이 변경될때 showmessage 호출해서 컨텐츠 텍스트 변경됨
            on(itemData); // OnClickStoreItem 호출
        }
    }
}
