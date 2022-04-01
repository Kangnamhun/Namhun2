using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SoundMenu : UI_Base
{
    enum Buttons
    {
        BackButton
    }
    enum GameObjects
    {
        AllSlider,
        BGMSlider,
        EffectSlider
    }
    Slider allSlider;
    Slider bgmSlider;
    Slider effectSlider;
    
    Button backButton;

    RectTransform tr;
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        allSlider = Get<GameObject>((int)GameObjects.AllSlider).GetComponent<Slider>();
        bgmSlider = Get<GameObject>((int)GameObjects.BGMSlider).GetComponent<Slider>();
        effectSlider = Get<GameObject>((int)GameObjects.EffectSlider).GetComponent<Slider>();

        backButton = GetButton((int)Buttons.BackButton);


        allSlider.onValueChanged.AddListener(Function_AllSlider);
        bgmSlider.onValueChanged.AddListener(Function_BGMSlider);
        effectSlider.onValueChanged.AddListener(Function_EffectSlider);

        backButton.gameObject.AddUIEvent(BackClick);
        
        tr = GetComponent<RectTransform>();
    }

    private void Function_AllSlider(float _value)
    {
        Managers.Sound.AllSoundCtrl(_value);

    }
    private void Function_BGMSlider(float _value)
    {
        Managers.Sound.BGMSoundCtrl(_value);

    }
    private void Function_EffectSlider(float _value)
    {
        Managers.Sound.EffectSoundCtrl(_value);

    }

    private void BackClick(PointerEventData data)
    {
        tr.gameObject.SetActive(false);
    }

}
