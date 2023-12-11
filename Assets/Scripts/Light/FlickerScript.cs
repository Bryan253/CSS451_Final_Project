using UnityEngine;
using Random = UnityEngine.Random;

public class FlickerScript : MonoBehaviour
{
    public Light _light;
    public float minTime, maxTime;
    public bool isOn = true;
    private float timer;
    
    void Start()
    {
        timer = Random.Range(minTime, maxTime);
    }


    void Update()
    {
        Flicker();
    }

    void Flicker()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            _light.enabled = !(_light.enabled);
            isOn = !(isOn);
            timer = Random.Range(minTime, maxTime);
        }
    }

    public bool LightIsOn()
    {
        return isOn;
    }
}
