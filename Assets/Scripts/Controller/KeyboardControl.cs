using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class KeyboardControl : MonoBehaviour
{
    // Player data & Cache
    Transform player;
    public Transform head;
    float speed = 4f;
    float rotRate = 90f;

    // Head Data
    float headMaxRot = 60f;
    float headAccumulatedRot = 0f;

    // Joint Data
    public List<Transform> joints = new();
    Quaternion shoulderInitQ;
    Quaternion elbowInitQ;
    float jointMaxRot = 45f;
    float currentShoulderRot = 0;
    float currentElbowRot = 0;

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
        shoulderInitQ = joints[0].localRotation;
        elbowInitQ = joints[2].localRotation;

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
        ArmMovement();
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

    void ArmMovement()
    {
        var dt = Time.deltaTime;
        var deltaAngle = rotRate * dt;

        var oldShoulderRot = currentShoulderRot;
        var oldElblowRot = currentElbowRot;
        var oldShoulderQ = joints[0].localRotation;
        var oldElbowQ = joints[2].localRotation;

        // Rotate Elbow with left / right mouse button
        if(Input.GetKey(KeyCode.Mouse0))
            currentElbowRot -= deltaAngle;
        if(Input.GetKey(KeyCode.Mouse1))
            currentElbowRot += deltaAngle;
        currentElbowRot = Mathf.Clamp(currentElbowRot, -jointMaxRot, jointMaxRot);
        var q = Quaternion.AngleAxis(currentElbowRot, Vector3.right);
        q = elbowInitQ * q;
        joints[2].localRotation = q;
        joints[3].localRotation = q;

        // Rotate Elbow with left / right mouse button
        if(Input.GetKey(KeyCode.Space))
            currentShoulderRot -= deltaAngle;
        if(Input.GetKey(KeyCode.LeftShift))
            currentShoulderRot += deltaAngle;
        currentShoulderRot = Mathf.Clamp(currentShoulderRot, -jointMaxRot, jointMaxRot);
        q = Quaternion.AngleAxis(currentShoulderRot, Vector3.right);
        q = shoulderInitQ * q;
        joints[0].localRotation = q;
        joints[1].localRotation = q;
        
        UpdateAndGetPlayerMatrix();

        // Check if any circle collide with walls
        for(int i = 0; i < playerPrimatives.Count; i++)
        {
            // Check if target location NOT touches the terrain
            if(!World.instance.HasContactedTerrain(playerMatrices[i].GetColumn(3), primativesRadius[i]))
                continue;

            // Revert changes if touches terrain
            currentShoulderRot = oldShoulderRot;
            currentElbowRot = oldElblowRot;
            joints[0].localRotation = oldShoulderQ;
            joints[1].localRotation = oldShoulderQ;
            joints[2].localRotation = oldElbowQ;
            joints[3].localRotation = oldElbowQ;
            UpdateAndGetPlayerMatrix();
            break;
        }
    }

    void UpdateAndGetPlayerMatrix()
    {
        World.instance.UpdatePlayerMatrix();
        playerMatrices.Clear();
        foreach(var np in playerPrimatives)
            playerMatrices.Add(np.selfMatrix);
    }
}
