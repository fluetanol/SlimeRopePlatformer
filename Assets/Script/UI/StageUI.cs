using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


public class StageUI : MonoBehaviour
{
    private UIDocument uiDoc;
    private VisualElement root;
    private ScrollView scrollView;
    private VisualElement contentContainer; // 스크롤뷰의 컨텐츠 영역 참조
    private List<GroupBox> scrollViewBoxes;

    private Vector2 startPointerPosition;  // 마우스 클릭 시작 위치
    private bool isDragging = false;       // 드래그 상태 확인
    float maxScrollOffsetX;                // 스크롤 가능한 최대 오프셋
    
    private Scroller scroller;

    

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        VisualElement e = root.Q<VisualElement>("StageElement");
        
        scrollView = e.Q<ScrollView>();
        scroller = scrollView.horizontalScroller;
        contentContainer = scrollView.contentContainer;
        scrollViewBoxes = contentContainer.Query<GroupBox>().ToList();
    }

    void Start()
    {
        // 레이아웃이 변경되었을 때 또는 처음 설정될 때 호출
        contentContainer.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            // 스크롤 가능한 범위 계산
            maxScrollOffsetX = Mathf.Max(0, scroller.highValue);
            for(int i=0; i< scrollViewBoxes.Count; i++){
                scrollViewBoxes[i].style.opacity = 1 - ((float)i / 6.0f);
            }
            Debug.Log($"Max scroll offset: {maxScrollOffsetX}"); // 값이 제대로 나오는지 확인
        });

        // PointerDown 이벤트: 마우스를 클릭한 순간을 잡아준다.
        scrollView.RegisterCallback<PointerDownEvent>(evt =>
        {
            isDragging = true;
            startPointerPosition = evt.position; // 마우스 클릭 위치 기억
            evt.StopPropagation();               // 이벤트 전파 중단
        });

        // PointerMove 이벤트: 마우스를 움직이는 동안 스크롤뷰를 이동
        scrollView.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (isDragging)
            {
                // 현재 위치에서 드래그 시작 위치의 차이를 계산
                Vector2 delta = (Vector2)evt.position - startPointerPosition;
                // 현재 스크롤 오프셋 계산 (이동 방향에 맞게 반대로 설정)
                Vector2 newScrollOffset = scrollView.scrollOffset - new Vector2(delta.x, 0);
                // 스크롤 오프셋을 허용 범위 내로 제한
                newScrollOffset.x = Mathf.Clamp(newScrollOffset.x, 0, maxScrollOffsetX);
                // 스크롤 오프셋 적용
                scrollView.scrollOffset = newScrollOffset;
                // 새로운 마우스 위치를 시작점으로 갱신
                startPointerPosition = evt.position;

            }
            evt.StopPropagation();
        });

        // PointerUp 이벤트: 마우스를 떼면 드래그 상태를 종료
        scrollView.RegisterCallback<PointerUpEvent>(evt =>
        {
            isDragging = false;
            evt.StopPropagation();
        });

        scrollView.RegisterCallback<PointerLeaveEvent>(evt =>
        {
            isDragging = false;
            evt.StopPropagation();
        });


    }

    // Update is called once per frame
    void Update()
    {



    }
}
//https://discussions.unity.com/t/scrollview-with-drag-scrolling/861606/4 참고
