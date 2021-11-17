using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Opc.Ua;
using UnityEngine;

namespace game4automation
{
    //! OPCUA Node Unity Script
    //! Represents an OPCUA node, holds the Nodes Attributes and Reads and Writes nodes values
    public class OPCUA_Node : MonoBehaviour
    {
        public OPCUA_Interface Interface; //!< The OPCUA Interface the node is part of / connected to
        public bool WriteValue; //!< True if the value should be written to the OPC server
        public bool ReadValue; //!< True if the value should be read from the OPC server
        public bool SubscribeValue; //!< True if value changes should be subscribed
        public string NodeId; //!< Nodeid from OPC UA Node (e.g. "ns=4;s=PLC.PLCOut_PNEUMATICZ_Einfahren")
#if GAME4AUTOMATION
        [ReadOnly] public string Name; //!< Name of the node
        [ReadOnly] public bool Connected; //!< true if connected
        [ReadOnly] public string Value; //!< Value as string of OPC Node
        [ReadOnly] public string Type; //!< Type as string of OPC Node
        [ReadOnly] public bool ValueIsArray; //!< true if Value is Array
        [ReadOnly] public string ValueArrayElementType; //!< Type of Array Element
        [ReadOnly] public int ValueArraySize; //!< Size of Array, 0 if no Array

        [ReadOnly]
        public List<string>
            ValuesArray = new List<string>(); //!< Array as string values, mainly for displaying purposes

        [ReadOnly] public string Status; //!< The status of the OPC Node
        [ReadOnly] public bool StatusGood; //!< true if Status is Good
        [ReadOnly] public bool StatusBad; //!< true if Status is Bad
        [ReadOnly] public string ServerTimestamp; //!< Servertimestamp as string
        [ReadOnly] public string SourceTimestamp; //!< Sourcetimestamp as string
        [ReadOnly] public string AccessLevel; //!< Access level
        [ReadOnly] public string UserAccessLevel; //!< User Access level 
        [ReadOnly] public string Description; //!< Description of Node
        [ReadOnly] public int NamespaceIndex; //!< Namespaceindex
#endif
#if !GAME4AUTOMATION
        public string Name; //!< Name of the node
        public bool Connected; //!< true if connected
        public string Value; //!< Value as string of OPC Node
        public string Type; //!< Type as string of OPC Node
        public bool ValueIsArray; //!< true if Value is Array
        public string ValueArrayElementType;
        public int ValueArraySize;
        public List<string> ValuesArray = new List<string>();
        public string Status;
        public bool StatusGood;
        public bool StatusBad;
        public string ServerTimestamp;
        public string SourceTimestamp;
        public string AccessLevel;
        public string UserAccessLevel;
        public string Description;

        public int NamespaceIndex;
#endif

        public object SignalValue;

        private OPCUANode _opcNode;

#if GAME4AUTOMATION
        private Signal signal;
#endif

        private OPCUANodeSubscription subscription;

#if GAME4AUTOMATION
        private Signal CreateGame4AutomationSignal(System.Type type)
        {
            Signal comp = (Signal) GetComponent(type);
            if (comp == null)
            {
                Signal exist = GetComponent<Signal>();
                if (exist != null)
                    DestroyImmediate(exist);

                comp = (Signal) gameObject.AddComponent(type);
            }

            return comp;
        }
#endif

        //! Writes the node value 
        public bool WriteNodeValue(object value)
        {
            Value = value.ToString();
            SignalValue = value;
            return Interface.WriteNodeValue(NodeId, value);
        }

        //! Reads the node value
        public object ReadNodeValue(object value)
        {
            return Interface.ReadNodeValue(NodeId);
        }

        public void SetValue(object value)
        {
            Value = value.ToString();
            SignalValue = value;
            if (_opcNode != null)
                _opcNode.Value = value;
        }


        //! Reads the node properties from OPC Server and Updates displayed porperties
        public void UpdateNode()
        {
            OPCUANode opcNode = Interface.ReadNode(NodeId);
            if (opcNode != null)
            {
                UpdateNode(opcNode);
#if GAME4AUTOMATION
                UpdatePLCSignal();
#endif
            }
        }

#if GAME4AUTOMATION
        //! Only used for Game4Automation Framework, Updates the PLCInput or PLCOutput Signal scripts corresponding to the Node direction and Node datatype
        public Signal UpdatePLCSignal()
        {
            Signal sig = null;
            if ((ReadValue || WriteValue) && !ValueIsArray)
            {
                if (ReadValue)
                {
                    switch (Type)
                    {
                        case "Boolean":
                            sig = CreateGame4AutomationSignal(typeof(PLCOutputBool));
                            break;
                        case "Double":
                        case "Float":
                            sig = CreateGame4AutomationSignal(typeof(PLCOutputFloat));
                            break;
                        case "Byte":
                        case "Int16":
                        case "Int32":
                        case "Int64":
                        case "UInt16":
                        case "UInt32":
                        case "UInt64":
                            sig = CreateGame4AutomationSignal(typeof(PLCOutputInt));
                            break;
                    }
                }

                if (WriteValue)
                {
                    switch (Type)
                    {
                        case "Boolean":
                            sig = CreateGame4AutomationSignal(typeof(PLCInputBool));
                            break;
                        case "Double":
                        case "Float":
                            sig = CreateGame4AutomationSignal(typeof(PLCInputFloat));
                            break;
                        case "Byte":
                        case "Int16":
                        case "Int32":
                        case "Int64":
                        case "UInt16":
                        case "UInt32":
                        case "UInt64":
                            sig = CreateGame4AutomationSignal(typeof(PLCInputInt));
                            break;
                    }
                }
            }

            if (sig != null)
                sig.SetValue(SignalValue);
            return sig;
        }
#endif

        //! Reads the node properties based on node object
        public void UpdateNode(OPCUANode opcNode)
        {
            // copy values of node to opcuainfo
            if (opcNode != null)
            {
                Type = opcNode.Type;
                Status = opcNode.Status;
                Value = opcNode.ValueString;
                Name = opcNode.Name;
                Description = opcNode.Description;
                Status = opcNode.Status;
                StatusGood = opcNode.StatusGood;
                StatusBad = opcNode.StatusBad;
                Connected = opcNode.Connected;
                ValueIsArray = opcNode.ValueIsArray;
                ValueArraySize = opcNode.ValueArraySize;
                ValueArrayElementType = opcNode.ValueArrayElementType;
                ServerTimestamp = opcNode.ServerTimestamp;
                SourceTimestamp = opcNode.SourceTimestamp;
                AccessLevel = opcNode.AccessLevel;
                UserAccessLevel = opcNode.UserAccessLevel;
                ValuesArray = opcNode.ValuesArray;
                SignalValue = opcNode.Value;
            }
        }

        //! Node Changed event for subscription
        private void NodeChanged(OPCUANodeSubscription sub, object obj)
        {
            SignalValue = obj;
            Value = obj.ToString();
            ServerTimestamp = sub.ServerTimestamp.ToString();
            SourceTimestamp = sub.SourceTimeStamp.ToString();
            Status = sub.Status;
            StatusGood = sub.StatusGood;
            StatusBad = sub.StatusBad;
#if GAME4AUTOMATION
            signal.SetValue(obj);
            if (StatusGood)
                signal.SetStatusConnected(true);
            else
            {
                signal.SetStatusConnected(false);
            }
#endif
        }

#if GAME4AUTOMATION
        //! Write signal changed event subscription
        private void WriteSignalChanged(Signal signal)
        {
            if (_opcNode != null)
            {
                _opcNode.Value = signal.GetValue();
                if (Interface.Status == "Connected")
                    signal.SetStatusConnected(true);
                else
                {
                    signal.SetStatusConnected(false);
                }
            }
        }
#endif
        //! Unity Awake Event - Subscibes to Game4Automation PLCInput Signal changes (only for Game4Automation Framework)
        public void Awake()
        {
#if GAME4AUTOMATION
            signal = GetComponent<Signal>();
            if (signal != null)
            {
                if (WriteValue)
                    signal.SignalChanged += WriteSignalChanged;
            }

#endif
        }

        //! Unity Start Event - Subscibes to the Changes of Nodes on OPCUA Server, Node Value will always be updated if SubscribeValue=true
        public void Start()
        {
            if (Interface != null)
            {
                if (Interface.ConnectToServer)
                {
#if GAME4AUTOMATION
                    UpdatePLCSignal();
#endif
                    if (WriteValue)
                    {
                        _opcNode = Interface.AddWatchedNode(NodeId);
                    }

                    if (SubscribeValue)
                    {
                        if (Interface != null)
                        {
                            subscription = Interface.Subscribe(NodeId, NodeChanged);
                        }
                    }
                }
            }
        }
    }
}