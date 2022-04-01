using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public int talkIndex { get; protected set; }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
        talkIndex = 0;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }    
}
