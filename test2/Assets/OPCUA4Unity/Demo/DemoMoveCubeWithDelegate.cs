using game4automation;
using UnityEngine;

public class DemoMoveCubeWithDelegate: MonoBehaviour
{
    public OPCUA_Interface Interface;
    public string NodeId;
    public float PositionY;

    private OPCUA_Node node;
    private OPCUANodeSubscription subscription;    
    
    // Start is called before the first frame update
    void Start()
    {
            if (Interface != null)
                subscription = Interface.Subscribe(NodeId, NodeChanged);   /// Subscribe to the nodeid with NodeChanged
    }
    
    public void NodeChanged(OPCUANodeSubscription sub, object obj)  // Is called when Node Value of Node nodeid is changed
    {
        PositionY = (float) obj;    // sets the new position based on the new value 
    }

    // Update is called once per frame
    void Update()
    {
      transform.rotation = Quaternion.Euler(new Vector3(0,PositionY,0));
    }
}
