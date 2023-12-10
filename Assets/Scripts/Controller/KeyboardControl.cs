using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour
{
    Transform player;
    public Transform head;
    float headMaxRot = 60f;
    float headAccumulatedRot = 0f;
    float speed = 4f;
    float rotRate = 90f;
    float sideLength = 30f;

    // Matrixes & Their source
    public List<NodePrimative> playerPrimatives = new();
    List<float> primativesRadius = new();
    List<Matrix4x4> playerMatrices = new();
    
    
    void Start()
    {
        player = World.instance.player.transform;
        Debug.Assert(player);
        Debug.Assert(head);

        speed *= player.lossyScale.x;

        primativesRadius.Clear();
        playerMatrices.Clear();
        foreach(var primatives in playerPrimatives)
        {
            primativesRadius.Add(primatives.transform.lossyScale.x);
            playerMatrices.Add(Matrix4x4.identity); // To prevent any unexpected error
        }
    }

    void Update()
    {
        PlayerMovement();
        HeadMovement();
    }

    void PlayerMovement()
    {
        var dt = Time.deltaTime;
        var p = player.position;
        var deltaP = Vector3.zero;

        var deltaAngle = rotRate * dt;
        var deltaRot = Quaternion.identity;
        
        // Player forward backward
        if(Input.GetKey(KeyCode.W))
            deltaP += player.forward * speed * dt;
        if(Input.GetKey(KeyCode.S))
            deltaP -= player.forward * speed * dt;

        // Player rotate left or right
        if(Input.GetKey(KeyCode.A))
            deltaRot *= Quaternion.AngleAxis(-deltaAngle, player.up);
        if(Input.GetKey(KeyCode.D))
            deltaRot *= Quaternion.AngleAxis(deltaAngle, player.up);

        // Make player only moveable in map
        p += deltaP;
        var clampP = p;
        clampP.x = Mathf.Clamp(clampP.x, -sideLength / 2, sideLength / 2);
        clampP.z = Mathf.Clamp(clampP.z, -sideLength / 2, sideLength / 2);
        if(p != clampP)
            deltaP += clampP - p;

        player.position += deltaP;
        Camera.main.transform.position += deltaP;
        var rot = player.localRotation;
        player.localRotation = deltaRot * rot;
        UpdateAndGetPlayerMatrix();

        // Check if any circle collide with walls
        for(int i = 0; i < playerPrimatives.Count; i++)
        {
            // Check if target location NOT touches the terrain
            if(!World.instance.HasContactedTerrain(playerMatrices[i].GetColumn(3), primativesRadius[i]))
                continue;

            // Revert changes if touches terrain
            player.position -= deltaP;
            Camera.main.transform.position -= deltaP;
            player.localRotation = rot;
            UpdateAndGetPlayerMatrix();
            break;
        }
    }

    void HeadMovement()
    {
        var dt = Time.deltaTime;
        var deltaAngle = rotRate * dt;

        // Calculated rotation of head
        if(Input.GetKey(KeyCode.Q))
            headAccumulatedRot -= deltaAngle;
        if(Input.GetKey(KeyCode.E))
            headAccumulatedRot += deltaAngle;
        headAccumulatedRot = Mathf.Clamp(headAccumulatedRot, -headMaxRot, headMaxRot);
        var q = Quaternion.AngleAxis(headAccumulatedRot, head.up);
        head.localRotation = q;
    }

    void UpdateAndGetPlayerMatrix()
    {
        World.instance.UpdatePlayerMatrix();
        playerMatrices.Clear();
        foreach(var np in playerPrimatives)
            playerMatrices.Add(np.selfMatrix);
    }
}
