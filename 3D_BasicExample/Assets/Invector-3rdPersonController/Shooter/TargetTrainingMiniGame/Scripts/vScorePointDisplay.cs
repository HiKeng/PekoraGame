using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class vScorePointDisplay : MonoBehaviour
{
    [SerializeField] protected UnityEngine.UI.Text _display;
    public string stringFormat;

    public UnityEngine.UI.Text display
    {
        get
        {
            if (_display == null) _display = GetComponent<UnityEngine.UI.Text>();
            return _display;
        }
    }

    const string StringFormatDefault = "{0}";

    public void ShowValue(float value)
    {
      
        if (string.IsNullOrEmpty(stringFormat)) stringFormat = StringFormatDefault;
        display.text = string.Format(stringFormat, value.ToString());
    }

    public void ShowValue(int value)
    {
        ShowValue((float)value);
    }
}
