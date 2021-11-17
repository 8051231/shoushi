// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  


using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace game4automation
{
    //! OPCUA Interface Script which is importing OPC UA Nodes and acting as the Interface towards the OPCConnectiong.
#if GAME4AUTOMATION
    public class OPCUA_Interface : Game4AutomationBehavior
#else
    public class OPCUA_Interface : MonoBehaviour
#endif
    
    {
        public string Server = "opc.tcp://localhost:4840"; //!< The address of the OPC Server (default is "opc.tcp://localhost:4840")
        public bool ConnectToServer = true; //!< If false no connection to the OPC server is made
        public string ApplicationName = "game4automation"; //!<  The application name of the OPC Client
        public string ApplicationURI = "urn::game4automation"; //!<  The appliction URi of the OPC Client
        public int SessionTimeoutMs = 60000; //!< The time in ms before session timeout
        public string SessionName = "game4automation"; //!< The session name of the OPC Session
        public bool AutoAcceptServerCertificates = true;  //!< True if all Server certificates should be accepted
        public bool ClientAutoGenerateCertificate = true; //!<  True if the Client should automatically generate its own certificate
        public string CertificatePath; //!< The Path of the Certificates. Home is always under StreamingAssets. You can define here a sub-path under streaming assets
        public string ClientCertificatePfx;   //!< The Path of the client certificate. If emtpy it is generated automatically
        public bool UseOnlySecureSendpoints = false;  //!<  True if only secure Endpoints should be used
        public bool UseHighLevelEndpointSecurity = false;  //!<  True if only the highest level of security provided by the Server should be used
        public string UserName = ""; //!< The username - if blank anonymous user will be used
        public string Password = ""; //!< The password for the User
        public string TopNodeId;  //!< The top Node ID under which the Nodes will be imported

        public int WatchThreadMinCycleTimeMs = 10; //!<  The minimum Cycle time of the WatchNodes Thread
#if GAME4AUTOMATION
        [ReadOnly] public string Status; //!<  The Status of the opc connection
        [ReadOnly] public int WatchThreadCycleNr; //!<  The current Cycle number of the WatchNodes Thread
        [ReadOnly] public int WatchThreadCycleMs;  //!<  The current Cycle time of the  WatchNodes Thread (might be higher than MinCommCycleMs im Communication load is heavy)
        [ReadOnly] public int WatchMaxCommTimeMs; //!<  The current max communication Time in  WatchNodes Thread since simulation startup
#else
        public string Status; //!<  The Status of the opc connection
        public int WatchThreadCycleNr; //!<  The current Cycle number of the WatchNodes Thread
        public int WatchThreadCycleMs;  //!<  The current Cycle time of the  WatchNodes Thread (might be higher than MinCommCycleMs im Communication load is heavy)
        public int WatchMaxCommTimeMs; //!<  The current max communication Time in  WatchNodes Thread since simulation startup
#endif
        
        private OPCUAConnection connection;

        //! Gets an OPCUA_Node  with the NodeID in all the Childrens of the Interface 
        public OPCUA_Node GetOPCUANode(string nodeid)
        {
            OPCUA_Node[] children = transform.GetComponentsInChildren<OPCUA_Node>();
          
            foreach (var child in children)
            {
                if (child.NodeId == nodeid)
                {
                    return child;
                }
            }
            return null;
        }

        //! Reads an OPC_UA Node and returns a new full OPCUANode class with all OPCUA Information (might be blocking if used to much)
        public OPCUANode ReadNode(string nodeid)
        {
            if (connection == null)
                Connect();
            if (connection == null)
                return new OPCUANode();
            else
            {
                return connection.ReadNode(nodeid,null);
            }
        }
        
        //! Reads an OPC_UA Node and updates the OPCUANode class with all OPCUA Information (might be blocking if used to much)
        public OPCUANode ReadNode(string nodeid, OPCUANode opcNode)
        {
            if (connection == null)
                Connect();
            if (connection == null)
            {
                opcNode.StatusBad = true;
                opcNode.StatusGood = false;
                return opcNode;
            }
            else
            {
                return connection.ReadNode(nodeid, opcNode);
            }
        }
        
        //! Reads a Node Value, might be blocking, it is usually better to subscribe to the Node
        public object ReadNodeValue(string nodeid)
        {
            if (connection == null)
                Connect();
            if (connection == null)
                return null;
            else
            {
                return connection.ReadNodeValue(nodeid);
            }
        }
        
        //! Writes a node value and returns true if successfull, function might be blocking it is better to use Watched Nodes which are using a parallel task
        public bool WriteNodeValue(string nodeid, object value)
        {
            if (connection == null)
                Connect();
            if (connection == null)
                return false;
            else
            {
                return connection.WriteNodeValue(nodeid, value);
            }
        }
        
        //! Calls the Method with NodeId methodid in the node nodeid and passes parameters - method returns object array as result
        public object[] CallMethod(string nodeid, string methodid, params object[] parameters)
        {
            if (connection == null)
                Connect();
            if (connection == null)
                return null;
            else
            {
                return connection.CallMethod(nodeid,methodid, parameters);
            }
        }
        
        //! Adds a Node to the Watched Nodes List.Watched Nodes are sending in a parallel Task new Node Value data if Node.Value is changed
        public OPCUANode AddWatchedNode(string nodeid)
        {
            if (connection != null)
                return connection.AddWatchedNode(nodeid);
            else
            {
                return null;
            }
        }

        //! Subscribes to an OPCUA node, delegate function gets called when node value is updated on OPCUA server
        public OPCUANodeSubscription Subscribe(string nodeid, NodeUpdateDelegate del)
        {
       
            OPCUANodeSubscription sub;
            if (ConnectToServer)
           
                sub = connection.SubscribeNodeDataChange(nodeid,del);
            else
            {
                sub = null;
            }
            if (sub==null)
            {
                if (ConnectToServer)
                    Debug.Log("Subscription to " + Server + " with node " + nodeid + " not possible because Server is not reachable!");
                return null;
            }
            else
            {
                return sub;
            }
        }

        //! Gets al Subnodes of topnode nodeid including all childrens
        public List<OPCUANode> GetAllSubnodes(string nodeid)
        {
            List<OPCUANode> res = new List<OPCUANode>();
            if (connection == null)
                Connect();
            if (connection != null)
            {
                res = connection.GetAllSubNodes(TopNodeId);
            }

            return res;
        }
        
        //! Gets al Subnodes of topnode nodeid only for one level
        public List<OPCUANode> GetSubnodes(string nodeid)
        {
            List<OPCUANode> res = new List<OPCUANode>();
            if (connection == null)
                Connect();
            if (connection != null)
            {
                res = connection.GetSubNodes(TopNodeId);
            }

            return res;
        }
        
        //! Imports all OPCUANodes under TopNodeId and creates GameObjects.
        //! If GameObject with NodeID is already existing the GameObject will be updated.
        //! Does not deletes any Nodes. If Game4Automation Framework is existent (Compiler Switch GAME4AUTOMATION) also Game4Automation
        //! PLCInputs and PLCOutputs are created or updatedfor all nodes with suitable data types.
        public void ImportNodes()
        {
            var nodes = GetAllSubnodes(TopNodeId);        
            CreateNodes(nodes,gameObject);       
            OPCUA_Node[] opcuanodes = GetComponentsInChildren<OPCUA_Node>();
            
#if GAME4AUTOMATION
            foreach (var node in opcuanodes)
            {
                node.UpdatePLCSignal();
            }
#endif
            Debug.Log("All nodes imported from " + Server + " " + TopNodeId);
        
        }
        
        private string GetString(string data)
        {
            var guidData = System.Convert.FromBase64String(data);

            var guid = new System.Guid(guidData);
            return guid.ToString("B").ToUpper();
        }
        
        
        //! Connects to the OPCUA server
        public void Connect()
        {
            // If already connected first disconnect
            if (connection == null)
            {
                connection = new OPCUAConnection();
            }
            
            connection.Server = Server;
            connection.ApplicationName = ApplicationName;
            connection.ApplicationURI = ApplicationURI;
            connection.SessionTimeoutMs = SessionTimeoutMs;
            connection.SessionName = SessionName;
            connection.AutoAcceptServerCertificates = AutoAcceptServerCertificates;
            connection.ClientAutoGenerateCertificate = ClientAutoGenerateCertificate;
            connection.UseOnlySecureSendpoints = UseOnlySecureSendpoints;
            connection.UseHighLevelEndpointSecurity = UseHighLevelEndpointSecurity;
            connection.UserName = UserName;
            connection.Password = Password;
            connection.MinCommCycleMs = WatchThreadMinCycleTimeMs;
            connection.ClientCertificatePfx = ClientCertificatePfx;
            #if !UNITY_IOS && !UNITY_ANDROID
                  connection.CertifcatePath = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath,  CertificatePath));
                  if (connection.ClientCertificatePfx!="")
                     connection.ClientCertificatePfx = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, ClientCertificatePfx));
                 
            #endif
            #if UNITY_IOS || UNITY_ANDROID
                    connection.CertifcatePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, CertificatePath));

                if (connection.ClientCertificatePfx!="")
                     connection.ClientCertificatePfx = Path.GetFullPath(Path.Combine(Application.persistentDataPath, ClientCertificatePfx));
            #endif
            connection.Connect();
#if !UNITY_IOS && !UNITY_ANDROID
            Debug.Log(
                "OPCUA4Unity - System pathes are Common Application Data: [" +
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) +
                "] Application Data: [" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)+"]");
           #endif
            Debug.Log("Connected to OPC Client " + Server + " with Status " + connection.GetStatus());
            
            
            
        }
            
        

        //! Disconnects from the  OPCUA server
        public void Disconnect()
        {
            if (connection != null)
            {
                connection.Disconnect();
                connection = null;
                Debug.Log("Disconnected to OPC Client" + Server);
            }
        }

        private void UpdateInfo(OPCUA_Node info, OPCUANode opcNode)
        {
           info.UpdateNode(opcNode);
        }
        
        private void CreateNodes(List<OPCUANode> nodes, GameObject topobject)
        {
            GameObject newnode;
            OPCUA_Node parentnode;
            foreach (OPCUANode node in nodes)
            {
                OPCUA_Node info = GetOPCUANode(node.NodeId);
                // If not create node
                if (info == null)
                {
                    newnode = new GameObject(node.Name);
                    newnode.transform.parent = topobject.transform;
                    info = newnode.AddComponent<OPCUA_Node>();
                    UpdateInfo(info,node);
                    info.NodeId = node.NodeId;
                    info.Interface = this;
                    
                    if (info.UserAccessLevel == "CurrentRead" || info.UserAccessLevel == "CurrentReadOrWrite")
                    {
                        info.ReadValue = true;
                        info.WriteValue = false;
                        info.SubscribeValue = true;
                    }
                    if (info.UserAccessLevel == "CurrentWrite")
                    {
                        info.ReadValue = false;
                        info.WriteValue = true; 
                    }
               
                }
                else
                {
                    newnode = info.gameObject;
                    UpdateInfo(info,node);
                }
                
            
 
                // if subnodes then create them
                if (node.SubNodes.Count > 0)
                {
                  CreateNodes(node.SubNodes,newnode);
                }
            }
        }
      
        public new void Awake()
        {
            if (ConnectToServer)
                 Connect();
        }
     
        public void OnApplicationQuit()
        {
            Disconnect();
        }
        
        public void Update()
        {
            if (connection != null)
            {
                Status = connection.Status;
                WatchThreadCycleMs = connection.CommCycleMs;
                WatchThreadCycleNr = connection.CommCycleNr;
                WatchMaxCommTimeMs = connection.MaxCommTimeMs;
            }

        }
    }
}