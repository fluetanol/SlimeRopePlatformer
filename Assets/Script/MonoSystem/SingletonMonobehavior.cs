using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehavior<T> : MonoBehaviour
{
    public static T Instance;

    [SerializeField] private bool isNotDestroyed;

    protected void Awake()
    {
        if (Instance == null) Instance = GetComponent<T>();
        if (isNotDestroyed) DontDestroyOnLoad(gameObject);
    }
}

