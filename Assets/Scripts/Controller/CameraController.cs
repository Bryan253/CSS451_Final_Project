using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float rotationRate = 0.1f;
    Vector3 tumbleStartP = Vector3.zero;
    Transform playerT;
    Transform cT;
    
    void Start()
    {
        playerT = World.instance.player.transform;
        cT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
            CameraMovement();
    }

    void CameraMovement()
    {
        CameraTumble();
        CameraZoom();
    }

    void CameraTumble()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
            tumbleStartP = Input.mousePosition;
        
        // No tumble if not holding left mouse button
        if(!Input.GetKey(KeyCode.Mouse0))
            return;

        // Compute how much to rotate
        var currentPos = Input.mousePosition;
        var xDiff = currentPos.x - tumbleStartP.x;
        var yDiff = tumbleStartP.y - currentPos.y;
        xDiff *= rotationRate;
        yDiff *= rotationRate;

        // Computer rotation
        var q = Quaternion.AngleAxis(xDiff, cT.up);
        q *= Quaternion.AngleAxis(yDiff, cT.right);

        // Compute new matrix for camera
        var cM = Matrix4x4.TRS(cT.position, cT.rotation, cT.localScale);
        var m = RotateFromPivot(playerT.position, q, cM);
        cT.position = m.GetPosition();
        CameraLookatAxis(); // Need to fix up if use m.rotation

        tumbleStartP = Input.mousePosition;
    }

    void CameraZoom()
    {
        var mouseScroll = Input.mouseScrollDelta.y;
        var p = cT.position;
        p += cT.forward * mouseScroll;
        if((playerT.position - p).magnitude < 2) // Check camera distance
            p = playerT.position - cT.forward * 2;
        cT.position = p;
    }

    Matrix4x4 RotateFromPivot(Vector3 pivot, Quaternion rotation, Matrix4x4 target)
    {
        // Note that this function doesn't consider scale, for camera only
        
        var invp = Matrix4x4.Translate(-pivot);
        var r = Matrix4x4.Rotate(rotation); // Rotate by rotation with pivot as origin
        var p = Matrix4x4.Translate(pivot);
        return  p * r * invp * target;
    }

    void CameraLookatAxis()
    {   
        // Obtain vector to compute q
        var direction = playerT.position - cT.position;
        direction.Normalize();

        // I belive that we can use LookRotation???
        cT.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
