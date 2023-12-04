using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneNode : MonoBehaviour
{
    public List<GameObject> nodes;
    public List<GameObject> primitives;
    public Vector3 origin = Vector3.zero;
    private Vector3 initialScale;
    private Quaternion initialQ;

    void Start()
    {
        initialScale = transform.localScale;
        initialQ = transform.localRotation;
    }

    // Referenced the example code
    public void GetSelfMatrix(ref Matrix4x4 parentTransform)
    {
        
        Matrix4x4 orgT = Matrix4x4.Translate(origin);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        
        // Assuming localP is 0, the below operation is parentT * orgT * localR * localS
        var selfTransform = parentTransform * orgT * trs;

        // propagate to all children
        foreach (var node in nodes.Select(n => n.GetComponent<SceneNode>()))
            node.GetSelfMatrix(ref selfTransform);
        
        // disseminate to primitives
        foreach (var p in primitives.Select(p => p.GetComponent<NodePrimative>()))
            p.LoadShaderMatrix(ref selfTransform);

    }
}
