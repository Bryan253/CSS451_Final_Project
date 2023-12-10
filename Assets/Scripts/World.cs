using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance = null;
    public SceneNode baseNode = null;
    public GameObject player;
    List<GameObject> walls;

    void Awake()
    {
        if(!instance)
            instance = this;
        Debug.Assert(baseNode);
        Debug.Assert(player);
    }

    void Start()
    {
        player.transform.position = GameObject.Find("Corner(Clone)").transform.position;
        walls = GameObject.FindGameObjectsWithTag("Wall").ToList();
    }

    void Update()
    {
        UpdatePlayerMatrix();
    }

    public void UpdatePlayerMatrix()
    {
        var m = Matrix4x4.identity;
        baseNode.GetSelfMatrix(ref m);
    }

    public bool HasContactedTerrain(Vector3 pt, float radius)
    {
        // Check contact with each wall
        foreach(var wall in walls)
            if(HasContacted(pt, radius, wall))
                return true;
        
        return false;
    }

    // Assumes target is a quad
    public bool HasContacted(Vector3 pt, float radius, GameObject target)
    {
        // Find distance between pt and infinte plane
        var targetPt = target.transform.position;
        var direction = targetPt - pt;
        var distance = Vector3.Dot(direction, target.transform.forward);
        
        // Check if pt touching the infinte plane
        if(Mathf.Abs(distance) > radius)
            return false;

        // Find contact point and check if it is on the plane
        var wallWidth = target.transform.lossyScale.x;
        var closestPt = pt + distance * target.transform.forward;

        // Raise y to same spot to check if closestPT is within Width
        closestPt.y = targetPt.y;
        
        // a = sqrt(b^2 + c^2)
        var ptDistance = 
            Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(Mathf.Abs(distance), 2));

        if((targetPt - closestPt).magnitude > wallWidth / 2 + ptDistance)
            return false;
        
        return true;
    }
}
