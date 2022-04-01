using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurorialScene : BaseScene
{

    protected override void Init()
    {
        base.Init();
        SceneType = SceneState.Tutorial;

        gameObject.GetOrAddComponent<CursorController>();


        GameObject  player = Managers.Game.Spawn("Player");

        player.name = Managers.Game._name;

        Camera.main.gameObject.GetOrAddComponent<CameraFollow>().SetPlayer(player);


        Managers.UI.SetSceneUI();

        Managers.Sound.Play("BGM/Tutorial", Define.Sound.BGM);
    }

    protected override void SceneMove()
    {
        base.SceneMove();
    }
    public override void Clear()
    {
        
    }
}
