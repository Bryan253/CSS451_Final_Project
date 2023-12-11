using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    public Light _light;
    public float maxInten, minInten, speed;
    private float maxRange, minRange;

    void Start()
    {
        maxRange = maxInten / 2f;
        minRange = minInten;
        StartCoroutine(ExecuteAfterTime(10));
    }

    void Update()
    {
        // light intensity and range go back and forth to simulate glowing effect
        float time = Mathf.PingPong(Time.time * speed, 1);
        _light.intensity = Mathf.Lerp(maxInten, minInten, time);
        _light.range = Mathf.Lerp(maxRange, minRange, time);
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        // slow down to compensate decreasing intensity and range
        speed = 0.5f;

        // start gradually decreasing the intensity and range
        while (maxInten >= 0.1f || _light.range >= 0.1f)
        {
            if (maxInten >= 0.1f) 
            {
                maxInten -= Time.deltaTime * 0.1f;
            }

            if (_light.range >= 0.1f) 
            {
                _light.range -= Time.deltaTime * 0.1f;
            }
        }

        // set to reasonable speed (0.0 - 0.1)
        speed = 0.001f;
    }
}
