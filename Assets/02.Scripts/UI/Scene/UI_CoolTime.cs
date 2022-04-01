using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_CoolTime : UI_Scene
{
    bool bInit;
    enum Images
    {
        BackGroundSkillImage,
        BackGroundNormalImage,
        NormalImage,
        SkillImage
    }
   
    Image backGroundSkillImage, backGroundNormalImage;
    Image normalImage, skillImage;

    public override void Init()
    {
        base.Init();
        if (bInit) return;
        bInit = true;

        Bind<Image>(typeof(Images));

        backGroundNormalImage = GetImage((int)Images.BackGroundNormalImage);
        backGroundSkillImage = GetImage((int)Images.BackGroundSkillImage);
        normalImage = GetImage((int)Images.NormalImage);
        skillImage = GetImage((int)Images.SkillImage);

    }
    public void SetSkiilImage(string jobstring)
    {
        backGroundNormalImage.sprite = ItemInfo.ins.GetSprite(jobstring + "_01");
        normalImage.sprite = ItemInfo.ins.GetSprite(jobstring + "_01");

        backGroundSkillImage.sprite = ItemInfo.ins.GetSprite(jobstring + "_02");
        skillImage.sprite = ItemInfo.ins.GetSprite(jobstring + "_02");
    }

    public void SetCoolTimeImage(float normalrate, float skillrate, float normalDel, float SkillDel)
    {
        backGroundNormalImage.fillAmount = 1 - (normalDel / normalrate);
        backGroundSkillImage.fillAmount = 1- (SkillDel / skillrate);
    }

}
