using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour
{
    public Transform player;
    float speed = 4f;
    float rotRate = 90f;
    
    void Start()
    {
        player = World.instance.player.transform;
    }

    void Update()
    {
        PlayerMovement();
        HeadMovement();
    }

    void PlayerMovement()
    {
        var dt = Time.deltaTime;
        var deltaP = Vector3.zero;

        var deltaAngle = rotRate * dt;
        var deltaRot = Quaternion.identity;
        
        // Player forward backward
        if(Input.GetKey(KeyCode.W))
            deltaP += player.forward * speed * dt;
        if(Input.GetKey(KeyCode.S))
            deltaP -= player.forward * speed * dt;


        if(Input.GetKey(KeyCode.A))
            deltaRot *= Quaternion.AngleAxis(-deltaAngle, player.up);
        if(Input.GetKey(KeyCode.D))
            deltaRot *= Quaternion.AngleAxis(deltaAngle, player.up);

        // Some codition check here if needed
        player.position += deltaP;
        Camera.main.transform.position += deltaP;
        var rot = player.localRotation;
        rot = deltaRot * rot;
        player.localRotation = rot;
    }

    void HeadMovement()
    {
        // Key press to move head
    }
}
