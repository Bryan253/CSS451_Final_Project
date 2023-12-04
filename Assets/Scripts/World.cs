using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance = null;
    public SceneNode head = null;
    public GameObject player;

    void Awake()
    {
        if(!instance)
            instance = this;
        Debug.Assert(head);
        Debug.Assert(player);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerMatrix();
    }

    void UpdatePlayerMatrix()
    {
        var m = Matrix4x4.identity;
        head.GetSelfMatrix(ref m);
    }
}
