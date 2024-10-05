using UnityEngine;

public class UITreeBehavior : MonoBehaviour
{
    public static UITree _uiTree;

    public static UITree.UINode GetCurrentNode() => _uiTree.GetCurrentUINode();   

    void Start()
    {
        InitializeTree();
        SetMainUIChild();
        SetStageUIChild();
        SetProgressUIChild();
        SetGameSettingUIChild();
    }

    private void InitializeTree()
    {
        MainUI _mainUI = FindObjectOfType<MainUI>();
        _uiTree = new UITree(_mainUI, _mainUI);
    }

    private void SetMainUIChild()
    {
        var currentNode = _uiTree.Root;
        var stageNode = MakeUINode<StageUI>(currentNode, "StageUI");
        currentNode.AddNextUINode(stageNode);

        var SettingNode = MakeUINode<SettingUI>(currentNode, "SettingUI");
        currentNode.AddNextUINode(SettingNode);
    }

    private UITree.UINode MakeUINode<T>(in UITree.UINode currentNode, string name ="") where T : UIDocumentMonoBehavior, IVisibleUI{
        T UI = FindObjectOfType<T>();
        var newNode = _uiTree.MakeNewUINode(UI, name);
        newNode.AddPrevUINode(currentNode);
        return newNode;
    }


    private void SetStageUIChild(){
        _uiTree.SetNextUINode(0);
        var currentNode = _uiTree.GetCurrentUINode();
        var progressNode = MakeUINode<ProgressUI>(currentNode, "ProgressUI");
        currentNode.AddNextUINode(progressNode);
        _uiTree.SetRootUINode();
    }

    private void SetProgressUIChild(){
        //_uiTree.SetNextUINode(1);
       // var currentNode = _uiTree.GetCurrentUINode();
        _uiTree.SetRootUINode();
    }

    private void SetGameSettingUIChild(){
        //_uiTree.SetNextUINode(1);
        //var currentNode = _uiTree.GetCurrentUINode();
        _uiTree.SetRootUINode();
    }
}
