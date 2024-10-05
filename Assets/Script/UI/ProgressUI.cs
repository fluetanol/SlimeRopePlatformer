using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class ProgressUI : UIDocumentMonoBehavior, IVisibleUI
{
    private ProgressBar _progressBar;
    private VisualElement _visualElementContainer;
    public string _sceneName ="GameWorld1";


    protected override void SetUIComponent()
    {
        _progressBar = _root.Q<ProgressBar>();
        _visualElementContainer = _root.Q<VisualElement>("VisualElementContainer");
    }

    protected override void SetUIEvent()
    {
        //AsyncOperation asyncOP= SceneManager.LoadSceneAsync(_sceneName);
        //StartCoroutine(LoadSceneProgress(asyncOP));
    }



    public IEnumerator<YieldInstruction> LoadSceneProgress(AsyncOperation asyncOP)
    {
        asyncOP.allowSceneActivation = false;
        // 진행 부분의 스타일 변경
//        _progressBar.Q("unity-progress-bar__background").style.backgroundColor = new StyleColor(Color.red);

        // 전체 프로그래스바 스타일 변경
        //_progressBar.style.backgroundColor = new StyleColor(Color.black);

        while (!asyncOP.isDone)
        {
             _progressBar.value = asyncOP.progress * 100;
             _progressBar.title = $"{_progressBar.value}%";
            if (asyncOP.progress >= 0.9f)
            {
                _progressBar.value += 5;
                yield return new WaitForSeconds(0.5f);
                asyncOP.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void Visible()
    {
        _visualElementContainer.RemoveFromClassList(UIElementOperation.INVISIBLE);
        AsyncOperation asyncOP= SceneManager.LoadSceneAsync(_sceneName);
        StartCoroutine(LoadSceneProgress(asyncOP));
    }

    public void Disappear()
    {
        _visualElementContainer.AddToClassList(UIElementOperation.INVISIBLE);
    }
}
