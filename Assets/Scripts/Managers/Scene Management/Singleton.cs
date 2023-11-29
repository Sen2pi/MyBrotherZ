using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (transform.parent == null)
        {
            GameObject singletonParent = new GameObject("SingletonParent");
            transform.SetParent(singletonParent.transform);
            DontDestroyOnLoad(singletonParent);
        }

        instance = GetComponent<T>();
    }
}
