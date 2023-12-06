using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePrimative : MonoBehaviour
{
    public Color color = Color.black;
    public Vector3 pivot = Vector3.zero;
    public Matrix4x4 selfMatrix;

    // Refereced example code
    public void LoadShaderMatrix(ref Matrix4x4 nodeMatrix)
    {
        Matrix4x4 p = Matrix4x4.Translate(pivot);
        Matrix4x4 invp = Matrix4x4.Translate(-pivot);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        // The operation below is nodeMatrix * p * localP * localR * localS * invp, start from rhs
        Matrix4x4 m = nodeMatrix * p * trs * invp;
        selfMatrix = m;
        GetComponent<Renderer>().material.SetMatrix("MyTRSMatrix", m);
        GetComponent<Renderer>().material.SetColor("MyColor", color);
    }
}
