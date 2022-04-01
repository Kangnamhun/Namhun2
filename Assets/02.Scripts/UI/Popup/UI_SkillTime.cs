using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_SkillTime : UI_Popup
{
    bool bInit;
    enum Images
    {
        StateImage
    }

    Image stateImage;
    public override void Init()
    {
        base.Init();
        if (bInit) return;
        bInit = true;

        base.Init();
        Bind<Image>(typeof(Images));

        stateImage = GetImage((int)Images.StateImage);


    }

    public void SetImage(float charge)
    {
        stateImage.fillAmount = charge /5;
    }

}
