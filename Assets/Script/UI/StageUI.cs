using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;


public interface IVisibleUI{
    void Visible();
    void Disappear();
}

public interface ISceneMove{
    IEnumerator<YieldInstruction> LoadSceneProgress(AsyncOperation asyncOP);
}

public class StageUI : UIDocumentMonoBehavior, IVisibleUI, ILoadAnotherUI
{
    private ScrollView _scrollView;
    private VisualElement contentContainer; // 스크롤뷰의 컨텐츠 영역 참조
    private List<GroupBox> scrollViewBoxes;
    private Button _backButton;

    private Vector2 startPointerPosition;  // 마우스 클릭 시작 위치
    private bool isDragging = false;       // 드래그 상태 확인
    float maxScrollOffsetX;                // 스크롤 가능한 최대 오프셋
    private Scroller scroller;

    protected override void SetUIComponent(){
        VisualElement titleElement = _root.Q<VisualElement>("TitleElement");
        VisualElement stageElement = _root.Q<VisualElement>("StageElement");

        _backButton = titleElement.Q<Button>();
        _scrollView = stageElement.Q<ScrollView>();
        scroller = _scrollView.horizontalScroller;
        contentContainer = _scrollView.contentContainer;
        scrollViewBoxes = contentContainer.Query<GroupBox>().ToList();
    }

    protected override void SetUIEvent()
    {
        _backButton.clicked += OnBackButton;
        SetStageButtonEvent();
        SetScrollViewEvent();
    }

    private void SetStageButtonEvent(){
        for(int i=0; i<scrollViewBoxes.Count; i++){
            int temp = i;
            scrollViewBoxes[i].Q<Button>().clicked += () => OnStageButton(temp);
        }
    }

    private void SetScrollViewEvent(){
        contentContainer.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            // 스크롤 가능한 범위 계산
            maxScrollOffsetX = Mathf.Max(0, scroller.highValue);
            Debug.Log($"Max scroll offset: {maxScrollOffsetX}"); // 값이 제대로 나오는지 확인
        });

        // PointerDown 이벤트: 마우스를 클릭한 순간을 잡아준다.
        _scrollView.RegisterCallback<PointerDownEvent>(evt =>
        {
            isDragging = true;
            startPointerPosition = evt.position; // 마우스 클릭 위치 기억
            evt.StopPropagation();               // 이벤트 전파 중단
        });

        // PointerMove 이벤트: 마우스를 움직이는 동안 스크롤뷰를 이동
        _scrollView.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (isDragging)
            {
                // 현재 위치에서 드래그 시작 위치의 차이를 계산
                Vector2 delta = (Vector2)evt.position - startPointerPosition;
                // 현재 스크롤 오프셋 계산 (이동 방향에 맞게 반대로 설정)
                Vector2 newScrollOffset = _scrollView.scrollOffset - new Vector2(delta.x, 0);
                // 스크롤 오프셋을 허용 범위 내로 제한
                newScrollOffset.x = Mathf.Clamp(newScrollOffset.x, 0, maxScrollOffsetX);
                // 스크롤 오프셋 적용
                _scrollView.scrollOffset = newScrollOffset;
                // 새로운 마우스 위치를 시작점으로 갱신
                startPointerPosition = evt.position;
            }
            evt.StopPropagation();
        });

        // PointerUp 이벤트: 마우스를 떼면 드래그 상태를 종료
        _scrollView.RegisterCallback<PointerUpEvent>(evt =>
        {
            isDragging = false;
            evt.StopPropagation();
        });

        _scrollView.RegisterCallback<PointerLeaveEvent>(evt =>
        {
            isDragging = false;
            evt.StopPropagation();
        });
    }


    void OnStageButton(int k){
        print("stage : " + k);
        UITreeBehavior._uiTree.SetNextUINode(0);
        UITreeBehavior.GetCurrentNode().Visible();
    }


    void OnBackButton(){
        UITreeBehavior._uiTree.SetPrevUINode(0);
        Disappear();
        UITreeBehavior.GetCurrentNode().Visible();
    }

    public void Visible(){
        VisualElement a = _root.Q<VisualElement>("VisualElementContainer");
        a.RemoveFromClassList(UIElementOperation.INVISIBLE);
    }

    public void Disappear(){
        VisualElement a = _root.Q<VisualElement>("VisualElementContainer");
        a.AddToClassList(UIElementOperation.INVISIBLE);
    }

    public IEnumerator<YieldInstruction> LoadAnotherUI()
    {
        float time = 0.5f;
        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        UITreeBehavior.GetCurrentNode().Visible();
    }
 
}
//https://discussions.unity.com/t/scrollview-with-drag-scrolling/861606/4 참고
