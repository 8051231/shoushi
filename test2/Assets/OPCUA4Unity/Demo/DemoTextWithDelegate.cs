using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game4automation;
using UnityEngine.UI;

public class DemoTextWithDelegate : MonoBehaviour
{
    public OPCUA_Interface Interface;
    public string NodeId;
    public string TextFromOPCUANode;
    
    private OPCUANodeSubscription subscription;
    private Text text;
    
  
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();   
        if (Interface != null)
            subscription = Interface.Subscribe(NodeId, NodeChanged);
    }
    
    public void NodeChanged(OPCUANodeSubscription sub, object obj)
    {
        TextFromOPCUANode = (string) obj;
      
     
    }


    // Update is called once per frame
    void Update()
    {
        text.text = TextFromOPCUANode;
    }
}
