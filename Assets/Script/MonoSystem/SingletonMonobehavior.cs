using UnityEngine;

public class SingletonMonobehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance {
        get{
           if(_instance == null){
               _instance = FindObjectOfType<T>();
           }
           return _instance;
       }
    }
    

    [SerializeField] private bool isNotDestroyed;


    protected void Awake()
    {
        if (Instance == null) _instance = GetComponent<T>();
        if (isNotDestroyed) DontDestroyOnLoad(gameObject);
    }
}

