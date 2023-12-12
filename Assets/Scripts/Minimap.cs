using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    void Update()
    {
        var p = transform.position;
        p.y -= Input.GetAxis("Mouse ScrollWheel") * 5f;
        p.y = Mathf.Clamp(p.y, 5, 50);
        transform.position = p;
    }
}
