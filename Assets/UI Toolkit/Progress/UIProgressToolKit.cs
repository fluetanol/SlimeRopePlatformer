using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIProgressToolKit : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/UIProgressToolKit")]
    public static void ShowExample()
    {
        UIProgressToolKit wnd = GetWindow<UIProgressToolKit>();
        wnd.titleContent = new GUIContent("UIProgressToolKit");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
