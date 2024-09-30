
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;


public class SceneNames{
    public const string GameStartScene = "GameScene";
}

public class MainUI : MonoBehaviour
{
    private UIDocument uiDoc;
    private VisualElement _root;

    private List<VisualElement> panelList;
    private ProgressBar progressBar;
    public List<Button> ButtonList;





    void Awake(){

        uiDoc = GetComponent<UIDocument>();
        _root = uiDoc.rootVisualElement;
        
        Button button = _root.Q<Button>("Button");
        panelList = _root.Children().ToList();
        progressBar = _root.Q<ProgressBar>("ProgressBar");
        ButtonList = panelList.SelectMany(x => x.Query<Button>().ToList()).ToList();
    }

    void Start()
    {
        ButtonList[0].clicked += OnGameStartButton;
        ButtonList[1].clicked += OnGameSettingButton;
        ButtonList[2].clicked += OnGameExitButton;
    }

    // Update is called once per frame
    void Update()
    {
    }


    void OnGameStartButton(){
        AsyncOperation asyncOP = SceneManager.LoadSceneAsync(SceneNames.GameStartScene);
        StartCoroutine(LoadSceneProgress(asyncOP));
    
    }

    void OnGameExitButton(){
        Debug.Log("Game Exit");
        Application.Quit();
    
    }

    void OnGameSettingButton(){
        Debug.Log("Game Setting");
    }

    private IEnumerator<YieldInstruction> LoadSceneProgress(AsyncOperation asyncOP){
        asyncOP.allowSceneActivation = false;
        progressBar.visible = true;
        Easing.Linear(3);
        while (!asyncOP.isDone){
            progressBar.value = asyncOP.progress * 100;
            if(asyncOP.progress >= 0.9f){
                progressBar.value += 5;
                yield return new WaitForSeconds(0.5f);
                asyncOP.allowSceneActivation = true;
                
            }
            yield return null;
        }
    }

}

