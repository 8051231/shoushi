using game4automation;
using UnityEngine;

public class DemoCallMethod : MonoBehaviour
{
    public OPCUA_Interface Interface;   
    public string NodeId;
    public string MethodId;
    public double MethodParameterA;
    public double MethodParameterB;
    public double Result;
    private double _a, _b;
    
    void Update()
    {
        // if parameter changed
        if (_a != MethodParameterA || _b != MethodParameterB)
        {
            var res = Interface.CallMethod(NodeId, MethodId,MethodParameterA,MethodParameterB); // calls a method with NodeID MethodID in Node NodeId with Parameters MethodParameterA and MethodParameterB
            Result = (double) res[0]; // The result is returned as an object array
        }
        _a = MethodParameterA;
        _b = MethodParameterB;
    }
}
