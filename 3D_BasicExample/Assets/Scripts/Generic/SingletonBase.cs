using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour
{
    public static T Instance;

    public virtual void Awake()
    {
        //Debug.Log(this.name + " == " + Instance.GetType().ToString());

        //if (Instance != null && Instance.GetType() == typeof(T))
        //{

        //    Destroy(this);
        //}
        //else
        //{
        //    Instance = this.GetComponent<T>();
        //}

        Instance = this.GetComponent<T>();
    }
}
