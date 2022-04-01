using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_MonsterHpBar : UI_Scene
{
    #region SetUp
    enum GameObjects
    {
        Body,
        HpBarImage,
        NameText
    }

    Image hpBarImage;
    Text nameText;
    GameObject body;

    float time;
    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        body = Get<GameObject>((int)GameObjects.Body);
        hpBarImage = Get<GameObject>((int)GameObjects.HpBarImage).GetComponent<Image>();
        nameText = Get<GameObject>((int)GameObjects.NameText).GetComponent<Text>();

        body.SetActive(false);
    }
    #endregion

    public void ChangeMonsterHit(Status status)
    {
        if (status.BDeath ) return;
        time = 0;
        body.SetActive(true);
        nameText.text = status.name;
        hpBarImage.fillAmount = status.Hp / status.MAX_HP;

    }
    public void OffMonsterHpbar()
    {
        if (body.activeSelf == true)
        {
            body.SetActive(false);
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time >= 7)
        {
            body.SetActive(false);
        }
    }

}
