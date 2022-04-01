using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Scene : BaseScene
{

    protected override void Init()
    {
        base.Init();
        SceneType = SceneState.Map1;

        gameObject.GetOrAddComponent<CursorController>();
        Managers.Game.GetPlayer().transform.position = new Vector3(-11.06f, 3.08f, -12.63f); // Æ÷Å» À§Ä¡
        Managers.Sound.Play("BGM/Map1", Define.Sound.BGM);


    }


    public override void Clear()
    {

    }
}

