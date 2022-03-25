using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonOnLoad<T> : MonoBehaviour
    where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (!Instance)
        {
            Instance = (T)FindObjectOfType(typeof(T));
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
