using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingSceneController : UI_Base
{
    enum Images
    {
        Progressbar
    }


    Image progressBar;
    public override void Init()
    {
        Bind<Image>(typeof(Images));

        progressBar = GetImage((int)Images.Progressbar);


        StartCoroutine(LoadSceneProcess());
    }


    IEnumerator LoadSceneProcess()
    {
        //LoadSceneAsync()의 방식은 비동기 방식으로 일시 중지가 발생하지 않는 방식이다.
        AsyncOperation op = SceneManager.LoadSceneAsync(Managers.Scene.loadSceneName);
        op.allowSceneActivation = false; // 로딩이 끝나면 자동으로 불러올지 --> 로딩씬이 너무 빨리 끝날까봐 fake로딩, 리소스 로딩이 끝나길 기다림

        float timer = 0f;

        while(!op.isDone) // 끝나지 않았을때
        {
            yield return null;

            if(op.progress < 0.9f) //fake
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); // 1초에 걸려서 다 채우기
                if(progressBar.fillAmount>= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
