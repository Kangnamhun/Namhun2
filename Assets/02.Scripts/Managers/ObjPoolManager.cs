using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPoolManager 
{
    GameObject[] arrow;
    GameObject[] fireBall;
    GameObject[] rotatorFireBall;
    GameObject[] bombSlimePool;

    GameObject[] targetPool;

    

    public Transform objPoolManager { get; private set; }





    public void Init()
    {
        // 오브젝트 풀 담을 오브젝트 생성
        objPoolManager = new GameObject { name = "ObjPoolManager" }.transform;
        Object.DontDestroyOnLoad(objPoolManager);
        //각 최대개수 설정
        arrow = new GameObject[200];
        fireBall = new GameObject[20];
        rotatorFireBall = new GameObject[3];
        bombSlimePool = new GameObject[4];

        for (int i =0; i< arrow.Length;i++)
        {
            arrow[i] = Managers.Resource.Instantiate("Arrow", objPoolManager);

            arrow[i].SetActive(false);
        }
        for (int i = 0; i < fireBall.Length; i++)
        {
            fireBall[i] = Managers.Resource.Instantiate("Magic fire", objPoolManager);
            fireBall[i].SetActive(false);
        }
        for (int i = 0; i < rotatorFireBall.Length; i++) 
        {
            rotatorFireBall[i] =Managers.Resource.Instantiate("RotatorFireBall", objPoolManager);
            Managers.Sound._rotatorFireBallSource = rotatorFireBall[i].GetComponent<AudioSource>();
            rotatorFireBall[i].SetActive(false);
        }
        for (int i = 0; i < bombSlimePool.Length; i++)
        {
            bombSlimePool[i] = Managers.Resource.Instantiate("Boom_Slime_A", objPoolManager);
            bombSlimePool[i].GetOrAddComponent<QuestReporter>();
            bombSlimePool[i].SetActive(false);
        }
        
    }

    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "Arrow":
                targetPool = arrow;
                break;
            case "FireBall":
                targetPool = fireBall;
                break;
            case "Boom_Slime_A":
                targetPool = bombSlimePool;
                break;
            case "RotatorFireBall":
                targetPool = rotatorFireBall;
                break;
        }
        for(int i =0; i< targetPool.Length;i++)
        {
            if(!targetPool[i].activeSelf) // 비활성화라면
            {
                return targetPool[i];
            }
        }

        return null;
    }
    public void ActiveFalse(string type)
    {
        switch (type)
        {
            case "Arrow":
                targetPool = arrow;
                break;
            case "FireBall":
                targetPool = fireBall;
                break;
            case "Boom_Slime_A":
                targetPool = bombSlimePool;
                break;
            case "RotatorFireBall":
                targetPool = rotatorFireBall;
                break;
        }
        for(int i =0; i< targetPool.Length;i++)
        {
            if(targetPool[i].activeSelf) // 비활성화라면
            {
                targetPool[i].SetActive(false);
            }
        }

    }
}
