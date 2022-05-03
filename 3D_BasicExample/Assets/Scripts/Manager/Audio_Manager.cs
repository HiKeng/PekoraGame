using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : SingletonBase<Audio_Manager>
{
    public override void Awake()
    {
        //base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
