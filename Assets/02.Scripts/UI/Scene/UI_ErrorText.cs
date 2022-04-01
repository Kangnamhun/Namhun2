using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ErrorText : UI_Scene
{
    enum Texts
    {
        ErrorText
    }

    Text errorText;
    Color alpha;
    float alphaSpeed;
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));

        errorText = GetText((int)Texts.ErrorText);

        alpha.a = 0f;
        alphaSpeed = 12f;
        
    }

    private void Update()
    {

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); //�ؽ�Ʈ ���İ� ����
        errorText.color = alpha;
    }

    public void SetErrorText(Define.Error type)
    {
        alpha.a = 255f;
        
        switch (type)
        {
            case Define.Error.NoneJob:
                errorText.text = "������ �����ϴ�.";
                break;
            case Define.Error.NoneWeapon:
                errorText.text = "������ ���Ⱑ �����ϴ�.";          
                break;
            case Define.Error.OtherWeapon:
                errorText.text = "������ �ٸ� ���� �Դϴ�.";
                break;
            case Define.Error.CoolTime:
                errorText.text = "��Ÿ�� �Դϴ�.";
                break;
            case Define.Error.NoneMp:
                errorText.text = "������ �����մϴ�.";
                break;
            case Define.Error.NonePotion:
                errorText.text = "������ �����ϴ�.";
                break;
            case Define.Error.NoneGold:
                errorText.text = "���� ��尡 �����մϴ�";
                break;
            case Define.Error.NoneTarget:
                errorText.text = "���͸� �������ּ���";
                break;
            case Define.Error.MaxInv:
                errorText.text = "�κ��丮�� ���� á���ϴ�.";
                break;
        }

    }

}
