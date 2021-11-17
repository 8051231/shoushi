using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using game4automation;

public class DemoReadNodeNotRecommended : MonoBehaviour
{
 
    public OPCUA_Interface Interface;
    public string NodeId;

    // Update is called once per frame
    void Update()
    {
        
        float myvar = (float)Interface.ReadNodeValue(NodeId);
    }
}
