using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : PlayerAttack
{
    GameObject[] fireBallPos;
    FireBall[] fireBalls;

    int hasFireBall;
    string fireballobj;
    string rotatorfall;
    float charge;

    private void Awake()
    {


        fireBallPos = GameObject.FindGameObjectsWithTag("FirePos");
        fireBalls = new FireBall[fireBallPos.Length];
        fireballobj = "FireBall";
        rotatorfall = "RotatorFireBall";
        hasFireBall = 0;

        range = 10.0f;
        attackRate = 0.8f;
        skillRate = 10f;
        attackRatio = 1f;
        skillRatio = 1.5f;

    }

    protected override IEnumerator Use()
    {
        animator.SetBool("Fire", false);

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);

        UI_SkillTime ui_SkillTime = Managers.UI.ShowPopupUI<UI_SkillTime>();
        ui_SkillTime.Init();
        while (true)
        {

            charge += Time.deltaTime;

            if (hasFireBall < 5 ? charge >= hasFireBall : false) // 차징만큼 담기
            {
                var fireballObj = Managers.Pool.MakeObj(fireballobj);
               
                if (fireballObj != null)
                {
                    fireBalls[hasFireBall] = fireballObj.GetComponent<FireBall>();
                    fireballObj.transform.position = fireBallPos[hasFireBall].transform.position;
                    fireballObj.transform.rotation = fireBallPos[hasFireBall].transform.rotation;
                    fireballObj.gameObject.SetActive(true);
                    fireBalls[hasFireBall].FireFireBall(fireBalls[hasFireBall].transform);

                    hasFireBall++;
                    ui_SkillTime.SetImage(hasFireBall);
                }

            }

            if (!Managers.Input.fire) // 발사
            {
                
                animator.SetBool("Fire", true);
                Managers.UI.ClosePopupUI(ui_SkillTime);
                for (int i = 0; i< hasFireBall; i++)
                {
                    fireBalls[i].SetTarget(attackTarget);
                    fireBalls[i].SetFire(true);
                    Managers.Sound.Play("EffectSound/Attack/FireBall");
                    yield return new WaitForSeconds(0.15f);

                }
                
                isAttack = false;
                attackDelay = 0;
                hasFireBall = 0;
                charge = 0;

                break;
            }

            yield return null;
        }
        yield return null;
    }

    protected override IEnumerator Skill()
    {
        if (attackTarget.layer != (int)Layer.Monster)// 몬스터 타겟이 아니면 취소 
        {
            skillDelay = 0;
            isAttack = false;
            yield  break;
        }

        animator.SetTrigger("IsSkill");
        var rotator = Managers.Pool.MakeObj(rotatorfall);
        rotator.SetActive(true);

        skillDelay = 0;
        isAttack = false;

        yield return null;
    }
}
