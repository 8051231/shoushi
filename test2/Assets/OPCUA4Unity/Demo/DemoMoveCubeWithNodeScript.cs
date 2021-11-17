using System.Collections;
using System.Collections.Generic;
using game4automation;
using UnityEngine;

public class DemoMoveCubeWithNodeScript : MonoBehaviour
{
    private OPCUA_Node node;

    public float PositionX;
    // Start is called before the first frame update
    void Start()
    {
        node = GetComponent<OPCUA_Node>();  // Values are transfered from connected OPCUA_Node scritp
    }

    // Update is called once per frame
    void Update()
    {
        if (node.SignalValue !=null) // on Startup the Value can be 0 before first update from OPC Server
            PositionX = (float) node.SignalValue;
      transform.rotation = Quaternion.Euler(new Vector3(PositionX,0,0));

    }
}
