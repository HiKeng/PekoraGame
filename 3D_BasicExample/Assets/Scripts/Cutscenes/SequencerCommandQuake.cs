using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandQuake : SequencerCommand
    {
        Vector3 originalPos;
        float shakeDuration;

        public void Start()
        {
            shakeDuration = GetParameterAsFloat(0);
            originalPos = Camera.main.transform.localPosition;
        }
        public void Update()
        {
            if(shakeDuration > 0)
            {
                Camera.main.transform.localPosition = originalPos + Random.insideUnitSphere * 0.1f;
                shakeDuration -= Time.deltaTime;
            }
            else
            {
                Camera.main.transform.localPosition = originalPos;
                Stop();
            }
        }
    }
}