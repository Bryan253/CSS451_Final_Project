using System;
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
    public GameObject lightPrefab;
    public AudioSource sfx;
    List<GameObject> walls;
    List<GameObject> orbs;
    List<GameObject> poppedOrbs = new();
    List<GameObject> ceilingLights;

    void Awake()
    {
        if(!instance)
            instance = this;
        Debug.Assert(baseNode);
        Debug.Assert(player);
    }

    void Start()
    {
        
        var p = GameObject.Find("Corner(Clone)").transform.position;
        player.transform.position = p;
        p.y = 10;
        Camera.main.transform.position = p;
        
        walls = GameObject.FindGameObjectsWithTag("Wall").ToList();
        orbs = GameObject.FindGameObjectsWithTag("Orb").ToList();
        ceilingLights = GameObject.FindGameObjectsWithTag("CeilingLight").ToList();
    }

    void Update()
    {
        UpdatePlayerMatrix();
        DiffuseHierchy(baseNode);
    }

    public void UpdatePlayerMatrix()
    {
        var m = Matrix4x4.identity;
        baseNode.GetSelfMatrix(ref m);
    }

    public bool HasContactedTerrain(Vector3 pt, float radius)
    {
        // Check contact with orbs
        foreach(var orb in orbs)
            if(HasContactedOrb(pt, radius, orb))
                return true;
        
        // Check contact with each wall
        foreach(var wall in walls)
            if(HasContactedWall(pt, radius, wall))
                return true;
        
        return false;
    }

    bool HasContactedWall(Vector3 pt, float radius, GameObject target)
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

    bool HasContactedOrb(Vector3 pt, float radius, GameObject target)
    {
        var targetPt = target.transform.position;
        var targetRadius = target.transform.lossyScale.x; // Assume sphere
        var distance = (pt - targetPt).magnitude;
        
        // Check for colloision
        if(distance > radius + targetRadius)
            return false;
        
        // Collision happened and do something
        sfx.Play();
        poppedOrbs.Add(Instantiate(lightPrefab, target.transform.position, Quaternion.identity));
        orbs.Remove(target);
        Destroy(target);
        return true;
    } 

    void DiffuseHierchy(SceneNode n)
    {
        foreach(var nodePrim in n.primitives.Select(n => n.GetComponent<NodePrimative>()))
        {
            var p = nodePrim.selfMatrix.GetPosition();
            var diameter = nodePrim.transform.lossyScale.x; // Assume sphere
            var c1 = Color.black;
            var c2 = Color.black;
            var l1 = new Vector3(1000, 1000, 1000); // Just far enough to not be closer by accident
            var l2 = new Vector3(1000, 1000, 1000);
            var i1 = 0f;
            var i2 = 0f;
            var r1 = 0f;
            var r2 = 0f;

            // Add diffuse data from ceiling lamps
            foreach(var light in ceilingLights)
            {
                // Disable light do not emmit light
                if(!light.GetComponent<Light>().enabled)
                    continue;
                
                var lightP = light.transform.position;
                var distance = (lightP - p).magnitude;
                var lightToP = p - lightP;
                lightToP.Normalize();

                // Set to the closest lamp data
                if(distance < (l1 - p).magnitude)
                {
                    var lightComp = light.GetComponent<Light>();
                    
                    l1 = lightP;
                    i1 = lightComp.intensity;
                    c1 = lightComp.color;
                    r1 = lightComp.range;
                }
            }

            // Add diffuse data (2) from orbs
            foreach(var orb in orbs)
            {
                var lightP = orb.transform.position;
                var lightToP = p - lightP;
                var distance = lightToP.magnitude;
                lightToP.Normalize();

                var lightComp = orb.GetComponent<Light>();
                var orbR = lightComp.range;

                // Check if the light is completely out of range
                if(distance > orbR + diameter)
                    continue;

                // Set to the closest lamp data
                if(distance < (l2 - p).magnitude)
                {
                    
                    l2 = lightP;
                    i2 = lightComp.intensity;
                    c1 = lightComp.color;
                    r2 = orbR;
                    break; // Not possible to contact multiple orb light source
                }
            }
            
            // Replace diffuse data (2) with brighter popped orb
            foreach(var popOrb in poppedOrbs)
            {
                var lightP = popOrb.transform.position;
                var lightToP = p - lightP;
                var distance = lightToP.magnitude;
                lightToP.Normalize();

                var lightComp = popOrb.transform.GetChild(0).GetComponent<Light>();
                var orbR = lightComp.range;

                // Check if the light is completely out of range
                if(distance > orbR + diameter)
                    continue;

                // Set to the closest lamp data
                if(distance < (l2 - p).magnitude)
                {
                    l2 = lightP;
                    i2 = lightComp.intensity;
                    c1 = lightComp.color;
                    r2 = orbR;
                    break; // Not possible to contact multiple orb light source
                }
            }


            var renderer = nodePrim.GetComponent<Renderer>();
            renderer.material.SetVector("lightPos1", l1);
            renderer.material.SetColor("lightCol1", c1);
            renderer.material.SetVector("lightPos2", l2);
            renderer.material.SetColor("lightCol2", c2);
            renderer.material.SetFloat("lightI1", i1);
            renderer.material.SetFloat("lightI2", i2);
            renderer.material.SetFloat("lightRange1", r1);
            renderer.material.SetFloat("lightRange2", r2);
        }

        foreach (var node in n.nodes.Select(n => n.GetComponent<SceneNode>()))
            DiffuseHierchy(node);
    }
}
