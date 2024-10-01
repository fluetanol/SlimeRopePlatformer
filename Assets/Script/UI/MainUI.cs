using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class SceneNames{
    public const string GameStartScene = "GameScene";
}

public class MainUI : MonoBehaviour, IVisibleUI
{
    private IVisibleUI _stageVisibleUI;
    private IVisibleUI _settingVisibleUI;

    private UIDocument _uiDoc;
    private VisualElement _root;
    private VisualElement _contentsContainer;
    private List<Button> ButtonList;
    private List<VisualElement> panelList;

    private event Action OnGameStart;
    private event Action OnSetting;
    private event Action OnExit;



    void Awake(){
        GetObjectComponent();
        GetUIComponent();
    }

    void OnEnable() {
        SetUIEvent();

    }

    void GetObjectComponent(){
        _uiDoc = GetComponent<UIDocument>();
        _stageVisibleUI = FindObjectOfType<StageUI>(); 
    }

    void GetUIComponent(){
        _root = _uiDoc.rootVisualElement;
        _contentsContainer = _root.Q<VisualElement>();
        panelList = _root.Children().ToList();
        ButtonList = panelList.SelectMany(x => x.Query<Button>().ToList()).ToList();
    }

    void SetUIEvent(){
        ButtonList[0].clicked += OnGameStartButton;
        ButtonList[1].clicked += OnGameSettingButton;
        ButtonList[2].clicked += OnGameExitButton;

        OnGameStart += _stageVisibleUI.Visible;
    }



    void OnGameStartButton(){
        Disappear();
    }

    void OnGameExitButton(){
        Debug.Log("Game Exit");
        Application.Quit();
    
    }

    void OnGameSettingButton(){
        Debug.Log("Game Setting");
    }


    private IEnumerator<YieldInstruction> LoadAnotherUI(DisplayStyle displayStyle, bool isInvoke = true){
        float time = 0.5f;
        while (time > 0){
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        if(isInvoke) OnGameStart?.Invoke();
        _contentsContainer.style.display = displayStyle;
    }

    public void Visible()
    {
        StartCoroutine(LoadAnotherUI(DisplayStyle.Flex, false));
        _contentsContainer.RemoveFromClassList("mainDisappear");
    }

    public void Disappear()
    {
        _contentsContainer.AddToClassList("mainDisappear");
        StartCoroutine(LoadAnotherUI(DisplayStyle.None));
    }
}

