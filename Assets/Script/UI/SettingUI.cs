using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public struct UIElementOperation{
    public const string DEFAULT = "default";
    public const string INVISIBLE = "invisible";
}
public struct SceneNames{
    public const string LoadingScene = "LoadingScene";
    public const string GameWorld1 = "GameWorld1";
    public const string GameWorld2 = "GameWorld2";
}

public class SettingUI : UIDocumentMonoBehavior, IVisibleUI, ILoadAnotherUI
{
    private VisualElement _visualContainer;
    private Button _confirmButton;
    private Button _cancelButton;

    protected override void SetUIComponent(){
       _visualContainer = _root.Q<VisualElement>("VisualContainer");
       _confirmButton = _visualContainer.Q<Button>("ConfirmButton");
       _cancelButton = _visualContainer.Q<Button>("CancelButton");
    }

    protected override void SetUIEvent()
    {   
        _confirmButton.clicked += OnConfirmButton;
        _cancelButton.clicked += OnCancelButton;
    }

    void OnConfirmButton(){
        UITreeBehavior._uiTree.SetPrevUINode(0);
        Disappear();
        UITreeBehavior.GetCurrentNode().Visible();
    }

    void OnCancelButton(){
        UITreeBehavior._uiTree.SetPrevUINode(0);
        Disappear();
        UITreeBehavior.GetCurrentNode().Visible();
    }


    public void Visible(){
        _visualContainer.RemoveFromClassList(UIElementOperation.INVISIBLE);
    }

    public void Disappear(){
        _visualContainer.AddToClassList(UIElementOperation.INVISIBLE);
    }

    public IEnumerator<YieldInstruction> LoadAnotherUI(){
        throw new System.NotImplementedException();
    }
}
