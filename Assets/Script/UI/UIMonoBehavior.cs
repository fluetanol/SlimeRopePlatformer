using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public abstract class UIDocumentMonoBehavior : MonoBehaviour
{
    protected UIDocument _uiDoc;
    protected VisualElement _root;

    protected void Awake()
    {
        SetBasicUI();
        SetUIComponent();
    }

    protected void OnEnable()
    {
        SetUIEvent();
    }

    private void SetBasicUI(){
        _uiDoc = GetComponent<UIDocument>();
        _root = _uiDoc.rootVisualElement;
    }

    protected abstract void SetUIComponent();
    protected abstract void SetUIEvent();
}
