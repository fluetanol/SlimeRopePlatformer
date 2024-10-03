using System.Collections.Generic;
using UnityEngine;

public class UITree
{
    public enum UIOrder
    {
        MainUI = -1,
        StageUI = 0,
        SettingUI = 1,
        ExitUI = 2
    }

    public class UINode
    {
        public UINode(IVisibleUI visibleUI, string name)
        {
            this.IVisibleUI = visibleUI;
            this.Name = name;
        }

        public string Name = "";
        public List<UINode> NextUINodes = null; //다음 노드 리스트가 비어있으면 게임 씬을 이동해야 합니다.
        public List<UINode> PrevUINodes = null; //이전 노드 리스트가 비어있다면 그냥 루트임
        private IVisibleUI IVisibleUI = null;

        public void Visible() => IVisibleUI.Visible();
        public void Disappear() => IVisibleUI.Disappear();

        public void AddNextUINode(UINode UINode)
        {
            if (NextUINodes == null)
            {
                NextUINodes = new List<UINode>();
            }
            NextUINodes.Add(UINode);
            Debug.Log(NextUINodes.Count);
        }

        public void AddPrevUINode(UINode UINode)
        {
            if (PrevUINodes == null)
            {
                PrevUINodes = new List<UINode>();
            }
            PrevUINodes.Add(UINode);
        }

        public void SetUINodeValue(IVisibleUI visibleUI)
        {
            this.IVisibleUI = visibleUI;
        }

    }

    public UINode Root
    {
        get;
        private set;
    }

    public UINode CurrentUINode
    {
        get;
        private set;
    }


    //root 초기화
    public UITree(IVisibleUI IVisible, ILoadAnotherUI IanotherUI)
    {
        Root = MakeNewUINode(IVisible);
        CurrentUINode = Root;
        Root.Name = "Root";
    }

    //현재 노드 가져오기
    public UINode GetCurrentUINode()
    {
        return CurrentUINode;
    }

    public void SetNextUINode(int num){
        CurrentUINode = CurrentUINode.NextUINodes[num];
    }

    public void SetPrevUINode(int num)
    {
        CurrentUINode = CurrentUINode.PrevUINodes[num];
    }

    public void SetRootUINode()
    {
        CurrentUINode = Root;
    }

    //새 노드 만들기
    public UINode MakeNewUINode(IVisibleUI visibleUI, string name = "")
    {
        return new UINode(visibleUI, name);
    }
}