using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance = null;
    void Start()
    {
        if(!instance)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
