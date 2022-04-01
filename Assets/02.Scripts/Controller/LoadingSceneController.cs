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
        //LoadSceneAsync()�� ����� �񵿱� ������� �Ͻ� ������ �߻����� �ʴ� ����̴�.
        AsyncOperation op = SceneManager.LoadSceneAsync(Managers.Scene.loadSceneName);
        op.allowSceneActivation = false; // �ε��� ������ �ڵ����� �ҷ����� --> �ε����� �ʹ� ���� ������� fake�ε�, ���ҽ� �ε��� ������ ��ٸ�

        float timer = 0f;

        while(!op.isDone) // ������ �ʾ�����
        {
            yield return null;

            if(op.progress < 0.9f) //fake
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); // 1�ʿ� �ɷ��� �� ä���
                if(progressBar.fillAmount>= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
