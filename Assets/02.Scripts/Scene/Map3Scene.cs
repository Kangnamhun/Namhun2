using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class Map3Scene : BaseScene
{
    PlayableDirector playableDirector;
    protected override void Init()
    {
        base.Init();
        SceneType = SceneState.Map3;
        gameObject.GetOrAddComponent<CursorController>();

        Managers.Game.GetPlayer().transform.position = new Vector3(-4f, 1.06f, -30.24f);
        Managers.Sound.Play("BGM/Map3", Define.Sound.BGM);

        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.Play();
        
    }

    public override void Clear()
    {
        Managers.Pool.ActiveFalse("Boom_Slime_A");
    }
    public void PlayerStop()
    {
        Managers.UI.isTalk(true);
    }
    public void PlayerGo()
    { 
        Managers.UI.isTalk(false);
       
    }
}