using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum ePotionType { HpPotion = 10001, MpPotion = 10002 };


public class Potion : UI_Base
{
    public ePotionType potionType;
    bool bPotion;

    enum Images
    {
        PotionImage
    }
    Image potionImage;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        potionImage = GetImage((int)Images.PotionImage);

        potionImage.gameObject.AddUIEvent(Invork_Potion);

    }
    void Invork_Potion(PointerEventData data)
    {
        if (bPotion) return;
        {
            //�κ��丮�� ������ �ִ��� �˻��ϰ�
            //������ true ������ ���ұ�Ŵ
            //������ false ������ ���ҽ�Ű������
            int _itemcode = (int)potionType;
            ItemData _itemData =  Managers.UI.ui_Inventory.CheckAndEatHP(_itemcode);
           
            if (_itemData != null)
            {
                Managers.Sound.Play("EffectSound/Potion");
                StartCoroutine(PotionDelay(1f, potionImage)); //������ 1��(��Ÿ��)
            }
            else
            {
                Managers.UI.ui_ErrorText.SetErrorText(Define.Error.NonePotion);
            }
        }
    }

    IEnumerator PotionDelay(float _duration, Image _skillBoard) //���� ��Ÿ��
    {
        bPotion = true;
        float _speed = 1f / _duration;
        float _percent = 0;
        while (_percent < 1f)
        {
            _percent += _speed * Time.deltaTime;
            _skillBoard.fillAmount = 1f - _percent;
            yield return null;
        }
        _skillBoard.fillAmount = 0f;
        bPotion = false;
    }
}
