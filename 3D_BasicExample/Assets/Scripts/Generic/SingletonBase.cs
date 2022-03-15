using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour
{
    public static T Instance;

    private void Awake()
    {
        Instance = this.GetComponent<T>();
    }
}
