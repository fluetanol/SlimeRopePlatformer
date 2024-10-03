using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public interface ILoadAnotherUI{
    public IEnumerator<YieldInstruction> LoadAnotherUI();
}

public class MainUI : UIDocumentMonoBehavior, IVisibleUI, ILoadAnotherUI
{
    private VisualElement _contentsContainer;
    private List<Button> ButtonList;
    private List<VisualElement> panelList;
    private DisplayStyle _displayStyle;
    private bool _isInvoke = true;   


    protected override void SetUIComponent(){
        _contentsContainer = _root.Q<VisualElement>();
        panelList = _root.Children().ToList();
        ButtonList = panelList.SelectMany(x => x.Query<Button>().ToList()).ToList();
    }

    protected override void SetUIEvent(){
        ButtonList[0].clicked += OnGameStartButton;
        ButtonList[1].clicked += OnGameSettingButton;
        ButtonList[2].clicked += OnGameExitButton;
    }

    void OnGameStartButton(){
        UITreeBehavior._uiTree.SetNextUINode(0);
        Disappear();
    }

    void OnGameSettingButton(){
        UITreeBehavior._uiTree.SetNextUINode(1);
        Disappear();
    }

    void OnGameExitButton(){
        Debug.Log("Game Exit");
        Application.Quit();
    }

    public IEnumerator<YieldInstruction> LoadAnotherUI(){
        float time = 0.5f;
        while (time > 0){
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _contentsContainer.style.display = _displayStyle;
        if (_isInvoke) UITreeBehavior.GetCurrentNode().Visible();
    }

    public void Visible(){
        _displayStyle = DisplayStyle.Flex;
        _isInvoke = false;
        StartCoroutine(LoadAnotherUI());
        _contentsContainer.RemoveFromClassList(UIElementOperation.INVISIBLE);
    }

    public void Disappear(){
        _displayStyle = DisplayStyle.None;
        _isInvoke = true;
        _contentsContainer.AddToClassList(UIElementOperation.INVISIBLE);
        StartCoroutine(LoadAnotherUI());
    }
}

