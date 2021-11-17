using System.Collections;
using System.Collections.Generic;
using game4automation;
using UnityEngine;

public class DemoWriteValueWithNodeScriptOnGameObject : MonoBehaviour
{
    private OPCUA_Node node;
    // Start is called before the first frame update
    void Start()
    {
        node = GetComponent<OPCUA_Node>();
    }

    // Update is called once per frame
    void Update()
    {
        #if GAME4AUTOMATION
        var signal = GetComponent<Signal>();
        if (signal != null)
            DestroyImmediate(signal);
        #endif
        node.SetValue(Random.Range(0,1000));
    }
}
