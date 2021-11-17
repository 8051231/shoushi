using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;
using UnityEngine;
using game4automation;
//using DG.Tweening;
using System;
using System.Data;
using JetBrains.Annotations;
using Hi5_Interaction_Core;
using Hi5_Interaction_Interface;
using Ladder.Scripts;

public enum MenFaType
{
    MV2,
    MV1,
    PUMP,
    TY,
    TV2,
    F1,
    T1,
    L1
}
namespace Famen
{ 
public class FaMenRotate : MonoBehaviour
{
    [Header("OPCUA")]
    public OPCUA_Interface Interface;
    public string NodeId;
    public static FaMenRotate Instance;
    private OPCUANode node;
    private OPCUANodeSubscription subscription;

    private float horizontalMove, verticalMove;
    public float moveSpeed = 60;
    public int percent = 0;
    public float angle = 0;
    public float rotateangle = 0;
    //public Vector3 camPos;  //顶头相机位置
    //public float camYOffset=3;
    public Vector3 camEuler = new Vector3(90, 0, 0);

    public Color32 color32 = new Color32(196, 196, 196, 255);
    Color32 initColor;
    public MenFaType menFaType = MenFaType.MV2;

    public string currenMenFaName;
    public string FMName;
    public double openingValue;

    //角色对象
    private CharacterController m_CharacterController;

        //红灯绿灯相关变量
        public Transform switchTrans;
    public int currentAngle = 0;
    //public DengCtrl redCtrl;
    //public DengCtrl greenCtrl;
    //public PointerAni pointerAni;
    public float zhiizhenJiaoDu = 0f;
    public string pumpKaiGuanStatus = "关闭";
    //public bool currentPumpStatus = false;  //当前阀门状态
    public double currentPumpStatus;
    public bool isOpenOrClosePump = false;  //是否开启或关闭阀门
    string gameStatus = "";
    public double shuiwei = 1;
    public Transform Tpointer;
    public Transform Handle;
    public bool isInUpdate = false;
    //水位
    public TextMesh shuiWeiTextMesh;

    //public Transform pointer; //指针

    public string status = "良好"; //状态

    public string infoFamen = "";
    public static bool isOpenOrCloseShoushi = false;  //是否开启或关闭阀门的手势触发标识
    public static bool isEntryCtrl = false;  //是否进入控制模式
    //.GetComponent<脚本>().变量;
    GameObject mHand;
    GameObject mRightHand;
    Hi5_Glove_Gesture_Recognition_State gloveStateTmp = Hi5_Glove_Gesture_Recognition_State.ENone;
    public static Vector3 m_OriginalCameraPosition;
    public static Vector3 m_OriginalCameraEulerAngles;
    public static Vector3 m_OriginalEulerAngles;
    public static Vector3 m_OriginalPosition;
    private Vector3 m_MoveDir = Vector3.zero;

    private void Awake()
    {
        Instance = this;
        if (transform.parent.transform.Find("Body") != null)
        {
            initColor = transform.parent.transform.Find("Body").GetComponent<MeshRenderer>().materials[0].color;
        }
        else if (transform.Find("Model/Body") != null)
        {
            initColor = transform.Find("Model/Body").GetComponent<MeshRenderer>().materials[0].color;
        }

        if (transform.parent.name.Contains("PumpType"))
        {
            switchTrans = transform.Find("OnSwitch").transform;
            //redCtrl = transform.Find("RedLed").GetComponent<DengCtrl>();
            //greenCtrl = transform.Find("GreenLed").GetComponent<DengCtrl>();
            //pointerAni = transform.Find("PumpBody/PPointer").GetComponent<PointerAni>();
        }
        if (transform.name.Contains("@T1"))
        {
            //switchTrans = this.transform;

            //pointerAni = transform.Find("Model/Pointer").GetComponent<PointerAni>();
            Tpointer = transform.Find("Model/Pointer");
        }

        if (transform.name.Contains("@F1"))
        {
            //   shuiWeiTextMesh = transform.Find("Model/Body/ShowText").GetComponent<TextMesh>();
        }

        if (transform.name.Contains("@MV1"))
        {
            currenMenFaName = transform.name;
            Handle = transform.Find("Model/Handle");
        }
        if (transform.name.Contains("@MV2"))
        {
            currenMenFaName = transform.name;
            Handle = transform.Find("Model/Handle");
        }
        else if (transform.name.Contains("@TV2") || transform.name.Contains("@T1") || transform.name.Contains("@F1") || transform.name.Contains("@TY") || transform.name.Contains("@L1"))
        {
            currenMenFaName = transform.name;
        }
        else if (transform.parent.name.Contains("PumpType"))
        {
            currenMenFaName = transform.name;

        }

    }

    private void Start()
    {
         m_CharacterController = gameObject.GetComponent<CharacterController>();

            //   NodeId = NodeManager.Instance.GetNodeId(currenMenFaName);
            //  FMName = VNameManager.Instance.GetNodeIdd(currenMenFaName);
            // print("111111111111");
            // print("111111111111");

            if (NodeId != null && NodeId != "#N/A")
        {
            transform.gameObject.tag = "MenFa";
        }
        else
        {
            transform.gameObject.tag = "Untagged";
            //Debug.Log(transform.gameObject.name);
        }
        if (Interface != null)
            subscription = Interface.Subscribe(NodeId, NodeChanged);
        //   if (menFaType == MenFaType.PUMP)
        //   {
        //      Debug.Log("1111-----");
        //  }
        double openingValue = (double)Interface.ReadNodeValue(NodeId);

        angle = (float)openingValue * 3.6f;
        node = Interface.AddWatchedNode(NodeId);
        //if (transform.parent.parent.name.Contains("Pump"))
        //{
        //    greenCtrl.SetMaterial(false);
        //    redCtrl.SetMaterial(false);
        //}
        //mGlove_Gesture_Recognition  = GameObject.Find("Hi5_Glove_Gesture_Recognition");
            
    }

    public void NodeChanged(OPCUANodeSubscription sub, object obj)  // Is called when Node Value of Node nodeid is changed
    {
        openingValue = (double)obj;
        switch (menFaType)
        {
            case MenFaType.MV1:
                gameStatus = openingValue == 1 ? "开启" : "关闭";
                //print("NodeChanged:" + openingValue);
                //print(gameStatus);
                break;
            case MenFaType.MV2:
                angle = (float)openingValue * 3.6f;
                break;
            case MenFaType.PUMP:
                pumpKaiGuanStatus = openingValue == 1 ? "开启" : "关闭";
                zhiizhenJiaoDu = openingValue == 1 ? 370f : 0f;
              //  greenCtrl.SetMaterial(openingValue == 1);
              //  redCtrl.SetMaterial(openingValue == 0);
                //currentPumpStatus = openingValue == 0;
                // currentPumpStatus = openingValue == 1;
                isOpenOrClosePump = openingValue == 0;
                break;
            case MenFaType.TY:
                //    shuiwei = openingValue;
                break;
            case MenFaType.F1:
                // shuiwei = openingValue  ;
                break;

            case MenFaType.TV2:
                break;
            case MenFaType.T1:
                break;
            case MenFaType.L1:
                break;
        }

        //print(angle);
        //this.transform.Rotate(new Vector3(0, angle, 0), Space.Self);
        // transform.localEulerAngles = new Vector3(0, 180, 0f);// sets the new position based on the new value 
    }
    //void Start()
    //{
    //    //Debug.Log("测试坐标:" + this.transform.position);
    //    camPos = new Vector3(this.transform.position.x,this.transform.position.y+camYOffset,this.transform.position.z);
    //}

    // Update is called once per frame
    void Update()
    {
        if (!isInUpdate)
        {
            if (menFaType == MenFaType.PUMP && node != null)
            {
                if (pumpKaiGuanStatus == "关闭")
                // if (node.Value = (double)0)
                {
                    node.Value = (double)0;
                    double temp = 0;
                    if (temp == 0)
                    {
                        gameObject.transform.Find("PumpBody/PPointer").localEulerAngles = new Vector3(0f, 0, 0);
                    }

                    Interface.WriteNodeValue(NodeId, temp);
                    //greenCtrl.SetMaterial(false);
                   // redCtrl.SetMaterial(true);
                    currentPumpStatus = 0;
                }
                else if (pumpKaiGuanStatus == "开启")
                {
                    node.Value = (double)1;
                    double temp = 1;
                    if (temp == 1)
                    {
                        gameObject.transform.Find("PumpBody/PPointer").localEulerAngles = new Vector3(90f, 0, 0);
                    }

                    Interface.WriteNodeValue(NodeId, temp);
                   // greenCtrl.SetMaterial(true);
                   // redCtrl.SetMaterial(false);
                    currentPumpStatus = 1;
                }
            }
            else if (menFaType == MenFaType.MV1)
            {
                if (gameStatus == "开启")
                {
                    Handle.transform.localEulerAngles = new Vector3(0, 90, 0f);
                }
                else
                {
                    Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);
                }
            }
            else if (menFaType == MenFaType.T1)
            {

                Tpointer.transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            isInUpdate = true;
        }


        Debug.Log("currenMenFaName A=" + currenMenFaName);
            //if (GameCtrl.Instance.isEnterJieZhiFa&& GameCtrl.Instance.menFaName == currenMenFaName)
        if (GameCtrl.Instance.menFaName == currenMenFaName)
        {
                Debug.Log("currenMenFaName B="+ currenMenFaName);
                //     Hi5_Glove_Gesture_Recognition
                //           GetRecognitionState
                mHand = GameObject.Find("HI5_Left_Human_Hand_Visible");
            Hi5_Glove_Gesture_Recognition_State gloveState = Hi5_Glove_Gesture_Recognition_State.ENone;
            if(mHand != null)
            {
                gloveState = mHand.GetComponent<Hi5_Interface_Hand>().GetRecognitionState();
                Debug.Log("Hi5_Glove_Gesture_Recognition_State=" + gloveState);
            }
            if((gloveState == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) &&(gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.ENone))
            {
                gloveStateTmp = gloveState;
                GameObject cameraPlayer11 = GameObject.Find("[CameraRig]_HI5_Interaction");
                GameObject cameraPlayer = GameObject.Find("FPSController");
                m_OriginalCameraPosition = cameraPlayer.transform.position;
                m_OriginalCameraEulerAngles = cameraPlayer.transform.eulerAngles;
                m_OriginalPosition = cameraPlayer11.transform.localPosition;
                m_OriginalEulerAngles = cameraPlayer11.transform.eulerAngles;
                if (GameCtrl.Instance.menFaName == "P0901B_DIS@MV1_DN200")
                {
                    cameraPlayer.transform.position = new Vector3(-1.09f, 0.8799999f, -12.1f); //-0.538   0.436 0.519
                    cameraPlayer11.transform.eulerAngles = new Vector3(0f, 180f, 0f); //
                }
                if (GameCtrl.Instance.menFaName == "FV0902BF@MV2_DN200")
                {
                        //  cameraPlayer.transform.position = new Vector3(-1.09f, 0.8799999f, -10.88f); //-0.538   0.436 0.519
                    cameraPlayer.transform.position = new Vector3(7.00269f, 0.8799998f, 0.7852135f); // 7.002696 0.8799998 0.7852135
                    cameraPlayer11.transform.eulerAngles = new Vector3(90f, 0f, 0f); //
                    cameraPlayer11.transform.localPosition = new Vector3(-0.05f, 1.07f, -0.33f);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
            //上下碰撞触发显示
                    GameObject objCubeDown = GameObject.Find("CubeDown");
                    GameObject objCubeUp = GameObject.Find("CubeUp");
                    //objCubeDown.SetActive(true);
                    //objCubeUp.SetActive(true);
                    objCubeDown.gameObject.transform.localScale = new Vector3(0.6071323f, 2.14243f, 2.21835f);
                    objCubeUp.gameObject.transform.localScale = new Vector3(0.7753376f, 1.689273f, 1.838122f);
                 //   objCubeDown.GetComponent<Renderer>().enabled = false;
                 //   objCubeUp.GetComponent<Renderer>().enabled = false;
                    }

                    if (Hi5_Glove_Gesture_Recognition.isLeftFist == 0)
                {
                }
                FirstPersonController.IsInputEnabled = false;
                FirstPersonController.IsHandEnabled = false;
                FirstPersonController.IsRightHandEnabled = false;
                //       dunxia.IsInputEnabled = false;
                //       CtrlMove.IsCtrlMoveEnabled = false;
                }
            if ((gloveState == Hi5_Glove_Gesture_Recognition_State.EOk))
            {
                if (GameCtrl.Instance.menFaName == "FV0902BF@MV2_DN200")
                {
                    //上下碰撞触发显示
                    GameObject objCubeDown = GameObject.Find("CubeDown");
                    GameObject objCubeUp = GameObject.Find("CubeUp");
                    //objCubeDown.SetActive(true);
                    //objCubeUp.SetActive(true);
                    objCubeDown.gameObject.transform.localScale = Vector3.zero;
                    objCubeUp.gameObject.transform.localScale = Vector3.zero;
                }
                if (gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint)
                {
                    GameObject cameraPlayer = GameObject.Find("FPSController");
                    cameraPlayer.transform.eulerAngles = m_OriginalCameraEulerAngles;
                    cameraPlayer.transform.position = m_OriginalCameraPosition;
                    GameObject cameraPlayer11 = GameObject.Find("[CameraRig]_HI5_Interaction");
                    cameraPlayer11.transform.eulerAngles = m_OriginalEulerAngles;
                    cameraPlayer11.transform.localPosition = m_OriginalPosition;
                    gloveStateTmp = Hi5_Glove_Gesture_Recognition_State.ENone;
                }
         //       dunxia.IsInputEnabled = true;
         //       CtrlMove.IsCtrlMoveEnabled = true;
                FirstPersonController.IsInputEnabled = true;
                FirstPersonController.IsHandEnabled = true;
                FirstPersonController.IsRightHandEnabled = true;
                }
                ///   Cube.transform.localEulerAngles = tmp;
                //   gameObject.transform.Find("3D/[CameraRig]_HI5_Interaction").localEulerAngles = new Vector3(0f, 0, 0);
                /*    if (isEntryCtrl)
                {

                        transform.Rotate(Vector3.up * Time.deltaTime * 100);
                        horizontalMove = moveSpeed * Time.deltaTime;
                        angle += horizontalMove;
                        angle = Mathf.Clamp(angle, 0, 360);
                        Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                    }*/
                /*
                            if (menFaType == MenFaType.MV2)
                            {
                                if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button3))
                                //float h = Input.GetAxis("FaMenLeftAndRightCtrl");
                                {
                                    horizontalMove = moveSpeed * Time.deltaTime;
                                    angle += horizontalMove;
                                    angle = Mathf.Clamp(angle, 0, 360);
                                    Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                                    //  print(angle);
                                    //  print(Handle.transform.localEulerAngles);
                                    //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                                    //horizontalMove = moveSpeed * Time.deltaTime;
                                    //angle += horizontalMove;
                                    //angle = Mathf.Clamp(angle, 0, 360);
                                    //transform.localEulerAngles = new Vector3(0, angle, 0f);

                                    // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                                    // JieZhiFaUI.Instance.Show(percent + "%");

                                    //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                                    // print("angle:" + angle);
                                    double temp = Mathf.RoundToInt(angle / 360.0f * 100);

                                    Interface.WriteNodeValue(NodeId, temp);
                                    node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                                    //  print("node:" + node.Value);

                                    //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                                    //  "百分比:" + node.Value + "%\n" +
                                    //  "状态:" + status;
                                    //MenFaPercentText.Instance.Show(percent + "%");
                                    //  FaMenPercentText.Instance.Show(infoStr);
                                }
                                if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2))
                                //float h = Input.GetAxis("Horizontal");
                                {
                                    horizontalMove = moveSpeed * Time.deltaTime;
                                    angle -= horizontalMove;
                                    angle = Mathf.Clamp(angle, 0, 360);
                                    //    print(angle);
                                    Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                                    //  print(Handle.transform.localEulerAngles);
                                    node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                                    double temp = Mathf.RoundToInt(angle / 360.0f * 100);
                                    Interface.WriteNodeValue(NodeId, temp);
                                }
                                infoFamen = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n"
                            + (Mathf.RoundToInt(angle / 360.0f * 100)).ToString("#0") + "%\n" +
                             "状态:" + status;
                                //MenFaPercentText.Instance.Show(percent + "%");
                                FaMenPercentText.Instance.Show(infoFamen);

                            }
                */
                if (menFaType == MenFaType.MV2)
                {
                    Hi5_Glove_Gesture_Recognition_State rightgloveState = Hi5_Glove_Gesture_Recognition_State.ENone;
                    mRightHand = GameObject.Find("HI5_Right_Human_Hand_Visible");
                    if (mRightHand != null)
                    {
                        rightgloveState = mRightHand.GetComponent<Hi5_Interface_Hand>().GetRecognitionState();
                        Debug.Log("rightgloveState=" + rightgloveState);
                    }
                    //if (Input.GetKey(KeyCode.M) || isOpenOrCloseShoushi)
                    //     if (GameCtrl.Instance.menFaName == "FV0902BF@MV2_DN200")
                    {
                        if (Input.GetKey(KeyCode.M) || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && (gloveState == Hi5_Glove_Gesture_Recognition_State.EFist
                            || (rightgloveState == Hi5_Glove_Gesture_Recognition_State.EFist))))
                        //float h = Input.GetAxis("FaMenLeftAndRightCtrl");
                        {
                            if (GameCtrl.Instance.menFaName == "FV0902BF@MV2_DN200")
                            {
                                if(CtrlMove.moveUpFlag || CtrlMove.RotateDownFlag)
                                {
                                    horizontalMove = moveSpeed * Time.deltaTime;
                                    angle += horizontalMove;
                                    angle = Mathf.Clamp(angle, 0, 360);
                                    Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                                }
                                if (CtrlMove.moveDownFlag || CtrlMove.RotateUpFlag)
                                {
                                    horizontalMove = moveSpeed * Time.deltaTime;
                                    angle -= horizontalMove;
                                    angle = Mathf.Clamp(angle, 0, 360);
                                    Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                                }
                            }
                            else
                            {
                            /*isOpenOrCloseShoushi = false;
                            horizontalMove = moveSpeed * Time.deltaTime;
                            angle += horizontalMove;
                            angle = 360;
                            angle = Mathf.Clamp(angle, 0, 360);
                            Handle.transform.localEulerAngles = new Vector3(0, 90, 0f);*/
                            //  print(angle);
                            //  print(Handle.transform.localEulerAngles);
                            //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                            //horizontalMove = moveSpeed * Time.deltaTime;
                            //angle += horizontalMove;
                            //angle = Mathf.Clamp(angle, 0, 360);
                            //transform.localEulerAngles = new Vector3(0, angle, 0f);

                            // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                            // JieZhiFaUI.Instance.Show(percent + "%");

                            //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                            // print("angle:" + angle);
                            double temp = Mathf.RoundToInt(angle / 360.0f * 100);

                                //  Interface.WriteNodeValue(NodeId, temp);
                                //   node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                                //  print("node:" + node.Value);

                                //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                                //  "百分比:" + node.Value + "%\n" +
                                //  "状态:" + status;
                                //MenFaPercentText.Instance.Show(percent + "%");
                                //  FaMenPercentText.Instance.Show(infoStr);
                            }
                        }
                        if (Input.GetKey(KeyCode.N) || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.moveUpFlag && gloveState == Hi5_Glove_Gesture_Recognition_State.EFist)
                             || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.RotateUpFlag && rightgloveState == Hi5_Glove_Gesture_Recognition_State.EFist))
                        {
                           /* isOpenOrCloseShoushi = false;
                            horizontalMove = moveSpeed * Time.deltaTime;
                            angle += horizontalMove;
                            angle = 0;
                            angle = Mathf.Clamp(angle, 0, 360);
                            Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);*/
                            //  print(angle);
                            //  print(Handle.transform.localEulerAngles);
                            //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                            //horizontalMove = moveSpeed * Time.deltaTime;
                            //angle += horizontalMove;
                            //angle = Mathf.Clamp(angle, 0, 360);
                            //transform.localEulerAngles = new Vector3(0, angle, 0f);

                            // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                            // JieZhiFaUI.Instance.Show(percent + "%");

                            //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                            // print("angle:" + angle);
                            double temp = Mathf.RoundToInt(angle / 360.0f * 100);

                            //  Interface.WriteNodeValue(NodeId, temp);
                            //   node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                            //  print("node:" + node.Value);

                            //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                            //  "百分比:" + node.Value + "%\n" +
                            //  "状态:" + status;
                            //MenFaPercentText.Instance.Show(percent + "%");
                            //  FaMenPercentText.Instance.Show(infoStr);                 
                        }
                        /*
                        if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2)||
                            ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.moveUpFlag && gloveState == Hi5_Glove_Gesture_Recognition_State.EFist)
                            || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.RotateDownFlag && gloveState == Hi5_Glove_Gesture_Recognition_State.EFist))
                        //float h = Input.GetAxis("Horizontal");
                        {
                            horizontalMove = moveSpeed * Time.deltaTime;
                            angle -= horizontalMove;
                            angle = Mathf.Clamp(angle, 0, 360);
                            //    print(angle);
                            Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                            //  print(Handle.transform.localEulerAngles);
                        //    node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                            double temp = Mathf.RoundToInt(angle / 360.0f * 100);
                        //    Interface.WriteNodeValue(NodeId, temp);
                        }*/
                        infoFamen = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n"
                    + (Mathf.RoundToInt(angle / 360.0f * 100)).ToString("#0") + "%\n" +
                     "状态:" + status;
                        //MenFaPercentText.Instance.Show(percent + "%");
                        FaMenPercentText.Instance.Show(infoFamen);

                    }
                }

                else if (menFaType == MenFaType.MV1)
            {

                if (gameStatus == "开启")
                {
                    Handle.transform.localEulerAngles = new Vector3(0, 90, 0f);
                }
                else
                {
                    Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);
                }

                if (Input.GetKeyDown(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button3))
                {
                    Handle.transform.localEulerAngles = new Vector3(0, 90, 0f);
                    node.Value = (double)1;
                    double temp = 1;
                    Interface.WriteNodeValue(NodeId, temp);
                    //percent = 100;
                    //MenFaPercentText.Instance.Show(percent + "%");
                }
                else if (Input.GetKeyDown(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2))
                {
                    Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);
                    node.Value = (double)0;
                    double temp = 0;
                    Interface.WriteNodeValue(NodeId, temp);
                    //percent = 0;
                    //MenFaPercentText.Instance.Show(percent + "%");
                }

                if (Handle.transform.localEulerAngles.y == 90)
                {
                    gameStatus = "开启";
                }
                else if (Handle.transform.localEulerAngles.y == 0)
                {
                    gameStatus = "关闭";
                }

                string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                 gameStatus + "\n" +
                 "状态:" + status;
                //MenFaPercentText.Instance.Show(percent + "%");
                FaMenPercentText.Instance.Show(infoStr);
            }
            else if (menFaType == MenFaType.PUMP)
            {

                if (pumpKaiGuanStatus == "关闭")
                {
                    node.Value = (double)0;
                    double temp = 0;
                    if (temp == 0)
                    {
                        gameObject.transform.Find("PumpBody/PPointer").localEulerAngles = new Vector3(0f, 0, 0);
                    }

                    Interface.WriteNodeValue(NodeId, temp);
                  //  greenCtrl.SetMaterial(false);
                  //  redCtrl.SetMaterial(true);
                    currentPumpStatus = 0;
                }
                else if (pumpKaiGuanStatus == "开启")
                {
                    node.Value = (double)1;
                    double temp = 1;
                    if (temp == 1)
                    {
                        gameObject.transform.Find("PumpBody/PPointer").localEulerAngles = new Vector3(90f, 0, 0);
                    }

                    Interface.WriteNodeValue(NodeId, temp);
              //      greenCtrl.SetMaterial(true);
              //      redCtrl.SetMaterial(false);
                    currentPumpStatus = 1;
                }
                if (!isOpenOrClosePump)
                {
                    if (currentPumpStatus == 0)
                    {

                        if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button3))
                        {
                            isOpenOrClosePump = true;
                            node.Value = (double)0;
                            double temp = 0;
                            Interface.WriteNodeValue(NodeId, temp);
                            /*
                            switchTrans.DOLocalRotate(new Vector3(-90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                            {
                                switchTrans.DOLocalRotate(new Vector3(90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                {
                                    greenCtrl.SetMaterial(false);
                                    redCtrl.SetMaterial(true);
                                    //     zhiizhenJiaoDu = 0f;
                                    pumpKaiGuanStatus = "开启";
                                    //currentPumpStatus = false;
                                    currentPumpStatus = 1;
                                    isOpenOrClosePump = false;
                                });
                            });
                            pointerAni.transform.DOLocalRotate(new Vector3(90, 0, 0), 2f, RotateMode.LocalAxisAdd).OnComplete(() =>
                            {
                            });
                            */

                        }
                    }
                    else if (currentPumpStatus == 1)
                    {
                        if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2))
                        {
                            isOpenOrClosePump = true;
                            node.Value = (double)1;
                            double temp = 1;
                            Interface.WriteNodeValue(NodeId, temp);
                            /*switchTrans.DOLocalRotate(new Vector3(90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                            {
                                switchTrans.DOLocalRotate(new Vector3(-90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                {
                                    greenCtrl.SetMaterial(true);
                                    redCtrl.SetMaterial(false);

                                    //  zhiizhenJiaoDu = 370f;
                                    pumpKaiGuanStatus = "关闭";
                                    // currentPumpStatus = true;
                                    currentPumpStatus = 0;
                                    isOpenOrClosePump = false;
                                });
                            });
                            pointerAni.transform.DOLocalRotate(new Vector3(-90, 0, 0), 2f, RotateMode.LocalAxisAdd).OnComplete(() =>
                            {

                            });
                            */
                        }

                    }
                }

                string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" +
                 pumpKaiGuanStatus + "\n" +
                 "状态:" + status + "\n";
                //"示值:" + zhiizhenJiaoDu + "\n";
                FaMenPercentText.Instance.Show(infoStr);


            }
            else if (menFaType == MenFaType.TY || menFaType == MenFaType.L1)
            {
                // shuiWeiTextMesh.text = shuiwei.ToString("#0.00");
                string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                     "示值：" + openingValue.ToString("#0.00") + "%" + "\n" +
                 "状态:" + status;
                FaMenPercentText.Instance.Show(infoStr);
                //node.Value = Convert.ToDouble(shuiWeiTextMesh.text);
            }
            else if (menFaType == MenFaType.TV2)
            {
                string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                     "示值：" + openingValue.ToString("#0.00") + "%" + "\n" +
                 "状态:" + status;
                FaMenPercentText.Instance.Show(infoStr);
                //print(infoStr);
                //print(infoStr);
            }
            else if (menFaType == MenFaType.F1)
            /* {
                 shuiWeiTextMesh.text = shuiwei.ToString("#0.00") + "\n" + "    KG/H";
                 string infoStr = "阀门名称:高压煤浆泵出口流量" + "\n" +
                     "阀门编号："+LYHelpString.StripExt(transform.name, "@") + "\n" +
                 // "示值：" + shuiWeiTextMesh.text + "KG/H" + "\n" +
                //   "现场仪表读数：" + openingValue.ToString("#0.00") + "KG/H" + "\n" +
                   "DCS监控界面示数：" + openingValue.ToString("#0.00") + "KG/H" + "\n" +
                  "设备状态:" + status ;
                 FaMenPercentText.Instance.Show(infoStr);
                 //node.Value = Convert.ToDouble(shuiWeiTextMesh.text);
             }*/
            {
                // shuiWeiTextMesh.text = shuiwei.ToString("#0.00") + "\n" + "    KG/H";
                string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                     "示值：" + openingValue.ToString("#0.00") + "KG/H" + "\n" +
                 "状态:" + status;
                FaMenPercentText.Instance.Show(infoStr);
                //node.Value = Convert.ToDouble(shuiWeiTextMesh.text);
            }

            else if (menFaType == MenFaType.T1)
            {

                Tpointer.transform.localEulerAngles = new Vector3(0, 0, 90f);
                string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                     "示值：" + openingValue.ToString("#0.00") + "℃" + "\n" +
                 "状态:" + status;
                FaMenPercentText.Instance.Show(infoStr);
            }

        }

        if (GameCtrl.Instance.menFaName != currenMenFaName)
        {
            if (menFaType == MenFaType.MV1 || menFaType == MenFaType.MV2)
            {
                //print("能进?");

                SetColor(false);
            }
            else if (menFaType == MenFaType.PUMP)
            {
             //   pointerAni.Reset();
                SetColor(false);
                //greenCtrl.SetMaterial(false);
                //redCtrl.SetMaterial(false);
            }
            else if (menFaType == MenFaType.TY || menFaType == MenFaType.L1)
            {
                //print("可以进入吗?");
                // ShowOrHideShuiWeiText(false);
                SetColor(false);
            }
            else if (menFaType == MenFaType.TV2)
            {
                SetColor(false);
            }
            else if (menFaType == MenFaType.F1)
            {
                //print("可以进入吗?");
                //  ShowOrHideShuiWeiText(false);
                SetColor(false);
            }
            else if (menFaType == MenFaType.T1)
            {
                //pointerAni.Reset();
                //Tpointer.transform.localEulerAngles = new Vector3(0, 0, 0f);
                SetColor(false);
            }
        }

    }

    //水位显示或隐藏
    // public void ShowOrHideShuiWeiText(bool isShow)
    //  {
    //       shuiWeiTextMesh.gameObject.SetActive(isShow);
    //   }
    public void MenfaShoushi()
    {
        isEntryCtrl = true;
        return;
    }

//手柄旋转变化
    public void HandleRotate()
    {
           //  isOpenOrCloseShoushi = true;
           //    return;
            Debug.Log("GameCtrl.Instance.menFaName =" + GameCtrl.Instance.menFaName+" "+ this.currenMenFaName);
            //if (GameCtrl.Instance.isEnterJieZhiFa&& GameCtrl.Instance.menFaName == currenMenFaName)
            if (GameCtrl.Instance.menFaName == this.currenMenFaName)
            {
                //     Hi5_Glove_Gesture_Recognition
                //           GetRecognitionState
                mHand = GameObject.Find("HI5_Left_Human_Hand_Visible");
                Hi5_Glove_Gesture_Recognition_State gloveState = Hi5_Glove_Gesture_Recognition_State.ENone;
                if (mHand != null)
                {
                    gloveState = mHand.GetComponent<Hi5_Interface_Hand>().GetRecognitionState();
                    Debug.Log("Hi5_Glove_Gesture_Recognition_State=" + gloveState);
                }
                if (menFaType == MenFaType.MV2)
                {
                    Hi5_Glove_Gesture_Recognition_State rightgloveState = Hi5_Glove_Gesture_Recognition_State.ENone;
                    mRightHand = GameObject.Find("HI5_Right_Human_Hand_Visible");
                    if (mRightHand != null)
                    {
                        rightgloveState = mRightHand.GetComponent<Hi5_Interface_Hand>().GetRecognitionState();
                        Debug.Log("rightgloveState=" + rightgloveState);
                    }
                    //if (Input.GetKey(KeyCode.M) || isOpenOrCloseShoushi)
                    if (Input.GetKey(KeyCode.M) || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) )
                        || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && rightgloveState == Hi5_Glove_Gesture_Recognition_State.EFist))
                    //float h = Input.GetAxis("FaMenLeftAndRightCtrl");
                    {

                        if(isOpenOrCloseShoushi == false)
                        {

                            horizontalMove = moveSpeed * Time.deltaTime;
                            angle += horizontalMove;
                            angle = 360;
                            angle = Mathf.Clamp(angle, 0, 360);
                            Handle.transform.localEulerAngles = new Vector3(0, 0f, 0f);
                            //  print(angle);
                            //  print(Handle.transform.localEulerAngles);
                            //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                            //horizontalMove = moveSpeed * Time.deltaTime;
                            //angle += horizontalMove;
                            //angle = Mathf.Clamp(angle, 0, 360);
                            //transform.localEulerAngles = new Vector3(0, angle, 0f);

                            // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                            // JieZhiFaUI.Instance.Show(percent + "%");

                            //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                            // print("angle:" + angle);
                            double temp = Mathf.RoundToInt(angle / 360.0f * 100);

                            //  Interface.WriteNodeValue(NodeId, temp);
                            //   node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                            //  print("node:" + node.Value);

                            //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                            //  "百分比:" + node.Value + "%\n" +
                            //  "状态:" + status;
                            //MenFaPercentText.Instance.Show(percent + "%");
                            //  FaMenPercentText.Instance.Show(infoStr);
                            isOpenOrCloseShoushi = true;
                        }
                        else      
                        {
                            horizontalMove = moveSpeed * Time.deltaTime;
                            angle += horizontalMove;
                            angle = 0;
                            angle = Mathf.Clamp(angle, 0, 360);
                            Handle.transform.localEulerAngles = new Vector3(0, 0, -90f);
                            //  print(angle);
                            //  print(Handle.transform.localEulerAngles);
                            //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                            //horizontalMove = moveSpeed * Time.deltaTime;
                            //angle += horizontalMove;
                            //angle = Mathf.Clamp(angle, 0, 360);
                            //transform.localEulerAngles = new Vector3(0, angle, 0f);

                            // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                            // JieZhiFaUI.Instance.Show(percent + "%");

                            //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                            // print("angle:" + angle);
                            double temp = Mathf.RoundToInt(angle / 360.0f * 100);

                            //  Interface.WriteNodeValue(NodeId, temp);
                            //   node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                            //  print("node:" + node.Value);

                            //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                            //  "百分比:" + node.Value + "%\n" +
                            //  "状态:" + status;
                            //MenFaPercentText.Instance.Show(percent + "%");
                            //  FaMenPercentText.Instance.Show(infoStr);   
                            isOpenOrCloseShoushi = false;
                        }

                    }
                    if (Input.GetKey(KeyCode.N) || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.moveUpFlag && gloveState == Hi5_Glove_Gesture_Recognition_State.EFist)
                         || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.RotateUpFlag && rightgloveState == Hi5_Glove_Gesture_Recognition_State.EFist))
                    {
                        isOpenOrCloseShoushi = false;
                        horizontalMove = moveSpeed * Time.deltaTime;
                        angle += horizontalMove;
                        angle = 0;
                        angle = Mathf.Clamp(angle, 0, 360);
                        Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);
                        //  print(angle);
                        //  print(Handle.transform.localEulerAngles);
                        //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                        //horizontalMove = moveSpeed * Time.deltaTime;
                        //angle += horizontalMove;
                        //angle = Mathf.Clamp(angle, 0, 360);
                        //transform.localEulerAngles = new Vector3(0, angle, 0f);

                        // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                        // JieZhiFaUI.Instance.Show(percent + "%");

                        //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                        // print("angle:" + angle);
                        double temp = Mathf.RoundToInt(angle / 360.0f * 100);

                        //  Interface.WriteNodeValue(NodeId, temp);
                        //   node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                        //  print("node:" + node.Value);

                        //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                        //  "百分比:" + node.Value + "%\n" +
                        //  "状态:" + status;
                        //MenFaPercentText.Instance.Show(percent + "%");
                        //  FaMenPercentText.Instance.Show(infoStr);                 
                    }
                    /*
                    if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2)||
                        ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.moveUpFlag && gloveState == Hi5_Glove_Gesture_Recognition_State.EFist)
                        || ((gloveStateTmp == Hi5_Glove_Gesture_Recognition_State.EIndexPoint) && CtrlMove.RotateDownFlag && gloveState == Hi5_Glove_Gesture_Recognition_State.EFist))
                    //float h = Input.GetAxis("Horizontal");
                    {
                        horizontalMove = moveSpeed * Time.deltaTime;
                        angle -= horizontalMove;
                        angle = Mathf.Clamp(angle, 0, 360);
                        //    print(angle);
                        Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                        //  print(Handle.transform.localEulerAngles);
                    //    node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                        double temp = Mathf.RoundToInt(angle / 360.0f * 100);
                    //    Interface.WriteNodeValue(NodeId, temp);
                    }*/
                    infoFamen = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n"
                + (Mathf.RoundToInt(angle / 360.0f * 100)).ToString("#0") + "%\n" +
                 "状态:" + status;
                    //MenFaPercentText.Instance.Show(percent + "%");
                    FaMenPercentText.Instance.Show(infoFamen);

                }

                else if (menFaType == MenFaType.MV1)
                {

                    if (gameStatus == "开启")
                    {
                        Handle.transform.localEulerAngles = new Vector3(0, 90, 0f);
                    }
                    else
                    {
                        Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);
                    }

                    if (Input.GetKeyDown(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button3))
                    {
                        Handle.transform.localEulerAngles = new Vector3(0, 90, 0f);
                        node.Value = (double)1;
                        double temp = 1;
                        Interface.WriteNodeValue(NodeId, temp);
                        //percent = 100;
                        //MenFaPercentText.Instance.Show(percent + "%");
                    }
                    else if (Input.GetKeyDown(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2))
                    {
                        Handle.transform.localEulerAngles = new Vector3(0, 0, 0f);
                        node.Value = (double)0;
                        double temp = 0;
                        Interface.WriteNodeValue(NodeId, temp);
                        //percent = 0;
                        //MenFaPercentText.Instance.Show(percent + "%");
                    }

                    if (Handle.transform.localEulerAngles.y == 90)
                    {
                        gameStatus = "开启";
                    }
                    else if (Handle.transform.localEulerAngles.y == 0)
                    {
                        gameStatus = "关闭";
                    }

                    string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                     gameStatus + "\n" +
                     "状态:" + status;
                    //MenFaPercentText.Instance.Show(percent + "%");
                    FaMenPercentText.Instance.Show(infoStr);
                }
                else if (menFaType == MenFaType.PUMP)
                {

                    if (pumpKaiGuanStatus == "关闭")
                    {
                        node.Value = (double)0;
                        double temp = 0;
                        if (temp == 0)
                        {
                            gameObject.transform.Find("PumpBody/PPointer").localEulerAngles = new Vector3(0f, 0, 0);
                        }

                        Interface.WriteNodeValue(NodeId, temp);
                        //  greenCtrl.SetMaterial(false);
                        //  redCtrl.SetMaterial(true);
                        currentPumpStatus = 0;
                    }
                    else if (pumpKaiGuanStatus == "开启")
                    {
                        node.Value = (double)1;
                        double temp = 1;
                        if (temp == 1)
                        {
                            gameObject.transform.Find("PumpBody/PPointer").localEulerAngles = new Vector3(90f, 0, 0);
                        }

                        Interface.WriteNodeValue(NodeId, temp);
                        //      greenCtrl.SetMaterial(true);
                        //      redCtrl.SetMaterial(false);
                        currentPumpStatus = 1;
                    }
                    if (!isOpenOrClosePump)
                    {
                        if (currentPumpStatus == 0)
                        {

                            if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button3))
                            {
                                isOpenOrClosePump = true;
                                node.Value = (double)0;
                                double temp = 0;
                                Interface.WriteNodeValue(NodeId, temp);
                                /*
                                switchTrans.DOLocalRotate(new Vector3(-90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                {
                                    switchTrans.DOLocalRotate(new Vector3(90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                    {
                                        greenCtrl.SetMaterial(false);
                                        redCtrl.SetMaterial(true);
                                        //     zhiizhenJiaoDu = 0f;
                                        pumpKaiGuanStatus = "开启";
                                        //currentPumpStatus = false;
                                        currentPumpStatus = 1;
                                        isOpenOrClosePump = false;
                                    });
                                });
                                pointerAni.transform.DOLocalRotate(new Vector3(90, 0, 0), 2f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                {
                                });
                                */

                            }
                        }
                        else if (currentPumpStatus == 1)
                        {
                            if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2))
                            {
                                isOpenOrClosePump = true;
                                node.Value = (double)1;
                                double temp = 1;
                                Interface.WriteNodeValue(NodeId, temp);
                                /*switchTrans.DOLocalRotate(new Vector3(90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                {
                                    switchTrans.DOLocalRotate(new Vector3(-90f, 0, 0), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                    {
                                        greenCtrl.SetMaterial(true);
                                        redCtrl.SetMaterial(false);

                                        //  zhiizhenJiaoDu = 370f;
                                        pumpKaiGuanStatus = "关闭";
                                        // currentPumpStatus = true;
                                        currentPumpStatus = 0;
                                        isOpenOrClosePump = false;
                                    });
                                });
                                pointerAni.transform.DOLocalRotate(new Vector3(-90, 0, 0), 2f, RotateMode.LocalAxisAdd).OnComplete(() =>
                                {

                                });
                                */
                            }

                        }
                    }

                    string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" +
                     pumpKaiGuanStatus + "\n" +
                     "状态:" + status + "\n";
                    //"示值:" + zhiizhenJiaoDu + "\n";
                    FaMenPercentText.Instance.Show(infoStr);


                }
                else if (menFaType == MenFaType.TY || menFaType == MenFaType.L1)
                {
                    // shuiWeiTextMesh.text = shuiwei.ToString("#0.00");
                    string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                         "示值：" + openingValue.ToString("#0.00") + "%" + "\n" +
                     "状态:" + status;
                    FaMenPercentText.Instance.Show(infoStr);
                    //node.Value = Convert.ToDouble(shuiWeiTextMesh.text);
                }
                else if (menFaType == MenFaType.TV2)
                {
                    string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                         "示值：" + openingValue.ToString("#0.00") + "%" + "\n" +
                     "状态:" + status;
                    FaMenPercentText.Instance.Show(infoStr);
                    //print(infoStr);
                    //print(infoStr);
                }
                else if (menFaType == MenFaType.F1)
                /* {
                     shuiWeiTextMesh.text = shuiwei.ToString("#0.00") + "\n" + "    KG/H";
                     string infoStr = "阀门名称:高压煤浆泵出口流量" + "\n" +
                         "阀门编号："+LYHelpString.StripExt(transform.name, "@") + "\n" +
                     // "示值：" + shuiWeiTextMesh.text + "KG/H" + "\n" +
                    //   "现场仪表读数：" + openingValue.ToString("#0.00") + "KG/H" + "\n" +
                       "DCS监控界面示数：" + openingValue.ToString("#0.00") + "KG/H" + "\n" +
                      "设备状态:" + status ;
                     FaMenPercentText.Instance.Show(infoStr);
                     //node.Value = Convert.ToDouble(shuiWeiTextMesh.text);
                 }*/
                {
                    // shuiWeiTextMesh.text = shuiwei.ToString("#0.00") + "\n" + "    KG/H";
                    string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                         "示值：" + openingValue.ToString("#0.00") + "KG/H" + "\n" +
                     "状态:" + status;
                    FaMenPercentText.Instance.Show(infoStr);
                    //node.Value = Convert.ToDouble(shuiWeiTextMesh.text);
                }

                else if (menFaType == MenFaType.T1)
                {

                    Tpointer.transform.localEulerAngles = new Vector3(0, 0, 90f);
                    string infoStr = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n" +
                         "示值：" + openingValue.ToString("#0.00") + "℃" + "\n" +
                     "状态:" + status;
                    FaMenPercentText.Instance.Show(infoStr);
                }

            }

            if (GameCtrl.Instance.menFaName != currenMenFaName)
            {
                if (menFaType == MenFaType.MV1 || menFaType == MenFaType.MV2)
                {
                    //print("能进?");

                    SetColor(false);
                }
                else if (menFaType == MenFaType.PUMP)
                {
                    //   pointerAni.Reset();
                    SetColor(false);
                    //greenCtrl.SetMaterial(false);
                    //redCtrl.SetMaterial(false);
                }
                else if (menFaType == MenFaType.TY || menFaType == MenFaType.L1)
                {
                    //print("可以进入吗?");
                    // ShowOrHideShuiWeiText(false);
                    SetColor(false);
                }
                else if (menFaType == MenFaType.TV2)
                {
                    SetColor(false);
                }
                else if (menFaType == MenFaType.F1)
                {
                    //print("可以进入吗?");
                    //  ShowOrHideShuiWeiText(false);
                    SetColor(false);
                }
                else if (menFaType == MenFaType.T1)
                {
                    //pointerAni.Reset();
                    //Tpointer.transform.localEulerAngles = new Vector3(0, 0, 0f);
                    SetColor(false);
                }
            }



            if (menFaType == MenFaType.MV2)
        {
            //   if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button3))
            //float h = Input.GetAxis("FaMenLeftAndRightCtrl");
            {
           /*     horizontalMove = moveSpeed * Time.deltaTime;
                angle += horizontalMove;
                angle = Mathf.Clamp(angle, 0, 360);
                Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);*/
                //  print(angle);
                //  print(Handle.transform.localEulerAngles);
                //     //openingValue = (int)(Mathf.Floor(angle / 360.0f * 100));
                //horizontalMove = moveSpeed * Time.deltaTime;
                //angle += horizontalMove;
                //angle = Mathf.Clamp(angle, 0, 360);
                //transform.localEulerAngles = new Vector3(0, angle, 0f);

                // percent = (int)(Mathf.Floor(angle / 360.0f * 100));
                // JieZhiFaUI.Instance.Show(percent + "%");

                //print("阀门名字:"+ LYHelpString.StripExt(transform.parent.parent.name,"@"));
                // print("angle:" + angle);
                double temp = Mathf.RoundToInt(angle / 360.0f * 100);
                /*
                Interface.WriteNodeValue(NodeId, temp);
                node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));*/
                //  print("node:" + node.Value);

                //  string infoStr = "阀门名称:" + LYHelpString.StripExt(transform.parent.parent.name, "@") + "\n" +
                //  "百分比:" + node.Value + "%\n" +
                //  "状态:" + status;
                //MenFaPercentText.Instance.Show(percent + "%");
                //  FaMenPercentText.Instance.Show(infoStr);
            }
            if (Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button2))
            //float h = Input.GetAxis("Horizontal");
            {
                horizontalMove = moveSpeed * Time.deltaTime;
                angle -= horizontalMove;
                angle = Mathf.Clamp(angle, 0, 360);
                //    print(angle);
                Handle.transform.localEulerAngles = new Vector3(0, angle, 0f);
                //  print(Handle.transform.localEulerAngles);
                node.Value = (double)(Mathf.RoundToInt(angle / 360.0f * 100));
                double temp = Mathf.RoundToInt(angle / 360.0f * 100);
                Interface.WriteNodeValue(NodeId, temp);
            }
     //       infoFamen = LYHelpString.StripExt(transform.name, "@") + "\n" + FMName + "\n"
     //   + (Mathf.RoundToInt(angle / 360.0f * 100)).ToString("#0") + "%\n" +
     //    "状态:" + status;
            //MenFaPercentText.Instance.Show(percent + "%");
         //   FaMenPercentText.Instance.Show(infoStr);

        }

    }
    //颜色变化
    public void PlayPointerAni()
    {
       
    }

    public void SetColor(bool isEnter)
    {
        if (isEnter)
        {
            if (menFaType == MenFaType.MV1 || menFaType == MenFaType.MV2)
            {
                // transform.parent.transform.Find("Body").GetComponent<MeshRenderer>().materials[0].color = color32;
                transform.Find("Model/Body").GetComponent<MeshRenderer>().materials[0].color = color32;
            }
            //else if (menFaType == MenFaType.MV2 || menFaType == MenFaType.PUMP  )
            // {
            //     transform.parent.transform.Find("Body").GetComponent<MeshRenderer>().materials[0].color = color32;
            //  }
            else if (menFaType == MenFaType.TV2 || menFaType == MenFaType.T1 || menFaType == MenFaType.F1 || menFaType == MenFaType.TY || menFaType == MenFaType.L1)
            {
                transform.Find("Model/Body").GetComponent<MeshRenderer>().materials[0].color = color32;
            }
            else if (menFaType == MenFaType.PUMP)
            {
                transform.Find("PumpBody").GetComponent<MeshRenderer>().materials[0].color = color32;
            }
        }
        else
        {
            if (menFaType == MenFaType.MV1 || menFaType == MenFaType.MV2)
            {
                // transform.GetComponent<MeshRenderer>().materials[0].color = initColor;
                transform.Find("Model/Body").GetComponent<MeshRenderer>().materials[0].color = initColor;
            }
            //  else if (menFaType == MenFaType.MV2 || menFaType == MenFaType.PUMP )
            // {
            //       transform.parent.transform.Find("Body").GetComponent<MeshRenderer>().materials[0].color = initColor;
            //   }
            else if (menFaType == MenFaType.TV2 || menFaType == MenFaType.T1 || menFaType == MenFaType.F1 || menFaType == MenFaType.TY || menFaType == MenFaType.L1)
            {
                transform.Find("Model/Body").GetComponent<MeshRenderer>().materials[0].color = initColor;
            }
            else if (menFaType == MenFaType.PUMP)
            {
                transform.Find("PumpBody").GetComponent<MeshRenderer>().materials[0].color = color32;
            }
        }
    }
}

}