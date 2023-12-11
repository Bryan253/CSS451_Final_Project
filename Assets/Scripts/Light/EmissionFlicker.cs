using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionFlicker : MonoBehaviour
{
    public Material _enabled, _disabled;
    public FlickerScript flicker;

    void Update()
    {
        if (flicker.LightIsOn() && GetComponent<Renderer>().material == _disabled)
        {
            GetComponent<Renderer>().material = _enabled;
        }
        else if (!(flicker.LightIsOn()) && GetComponent<Renderer>().material == enabled)
        {
            GetComponent<Renderer>().material = _disabled;
        }
    }
}
