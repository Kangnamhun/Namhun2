using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2Scene : BaseScene
{
   
    protected override void Init()
    {
        base.Init();
        SceneType = SceneState.Map2;



        gameObject.GetOrAddComponent<CursorController>();
        Managers.Game.GetPlayer().transform.position = new Vector3(7.08f, 0.06f, -4.98f);
        Managers.Sound.Play("BGM/Map2", Define.Sound.BGM);
    }

    protected override void SceneMove()
    {
        base.SceneMove();

    }


    public override void Clear()
    {

    }
}
