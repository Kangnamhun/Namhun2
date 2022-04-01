using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{


    protected override void Init()
    {
        base.Init();
        SceneType = SceneState.Town;
        gameObject.GetOrAddComponent<CursorController>();
       
        Managers.Game.GetPlayer().transform.position =  Vector3.zero;


        Managers.Sound.Play("BGM/Town", Define.Sound.BGM);
    }

    protected override void SceneMove()
    {

    }
    public override void Clear()
    {

    }
}
