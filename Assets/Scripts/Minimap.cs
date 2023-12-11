using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        GetComponent<Camera>().fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * 1000f * Time.deltaTime;
        transform.position = new UnityEngine.Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }
}
