using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogPrint : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void _PrintThis(string _text)
    {
        Debug.Log(_text);
    }

    public void _PrintThisWithObjectName(string _text)
    {
        Debug.Log(gameObject.name + ": " + _text);
    }
}
