using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;
using Random = UnityEngine.Random;
using UnityStandardAssets.Utility;
using DG.Tweening;
using Hi5_Interaction_Core;

namespace Ladder.Scripts
{
    /// <summary>
    /// FPS player controller that has the ability to climb ladders
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        public static bool IsInputEnabled = true;
        public static bool IsHandEnabled = true;
        public static bool IsRightHandEnabled = true;
        private bool isTop = false;
        private float RunSpeed = 3;
        private float RotationSpeed = 25;
        public static FirstPersonController Instance;//实例化
        public float gravity;
        private Vector3 velocity;

        //判断是否在走
        [SerializeField]
        private bool m_IsWalking;
        //走路的速度
        [SerializeField]
        private float m_WalkSpeed;
        //奔跑的速度
        [SerializeField]
        private float m_RunSpeed;
        //模仿随机行走的速度
        [SerializeField]
        [Range(0f, 1f)]
        private float m_RunstepLenghten;
        //跳跃速度
        [SerializeField]
        private float m_JumpSpeed;
        //判断是否在空中，如果在空中接给一个下降的力
        [SerializeField]
        private float m_StickToGroundForce;
        //重力
        [SerializeField]
        private float m_GravityMultiplier;
        //视角控制脚本
        [SerializeField]
        private MouseLook m_MouseLook;
        [SerializeField]
        private bool m_UseFovKick;
        //FovKick脚本
        [SerializeField]
        private FOVKick m_FovKick = new FOVKick();
        [SerializeField]
        private bool m_UseHeadBob;
        [SerializeField]
        private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField]
        private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField]
        private float m_StepInterval;

        [SerializeField]
        private Transform aimed;
        [SerializeField]
        private Transform firstPersoncamera;
        //提示爬楼菜单
        [SerializeField]
        private GameObject aimTips;
        //提示爬楼菜单
        [SerializeField]
        private GameObject climbTips;
        //摄像机
        private Camera m_Camera;
        //跳跃标识
        private bool m_Jump;
        private float m_YRotation;
        //2维位置
        private Vector2 m_Input;
        //3维位置
        private Vector3 m_MoveDir = Vector3.zero;
        //角色对象
        private CharacterController m_CharacterController;
        //CharactorController的返回值，表示碰撞的信息
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        //玩家爬梯的速度
        private float m_ClimbSpeed; // How fast does the player climb the ladder
                                    //是否正在爬梯标识
        private bool m_isClimbing = false; // Are we currently climbing?
                                           //当前触发的梯子
        private Transform m_ladderTrigger; // The current trigger we hit for the ladder

        private GameObject player;
        GameObject cameraPlayer;
        GameObject viveObj;
        //角色触碰楼梯触发事件时，是否按X标识
        private bool isPressXKey = false;

        //角色触碰楼梯底部标识
        private bool isLowLadd = false;

        //角色触碰楼梯顶部标识
        private bool isUpLadd = false;

        //进入触发标识
        private bool isTrig = false;

        private void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {
            cameraPlayer = GameObject.Find("[CameraRig]_HI5_Interaction");
            //viveObj = GameObject.Find("Tracked Devices");
            //viveObj.GetComponent<Renderer>().enabled = false;
            //GetComponent<Renderer>().enabled = false;
            m_CharacterController = gameObject.GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_ClimbSpeed = 1f; //爬梯子速度
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);
        }
        void Update()
        {
            if (!IsInputEnabled)
            {
                return;
            }
            //update 
            //视角控制
            RotateView();

            // the jump state needs to read here to make sure it is not missed
            //跳转状态判断，跳转状态需要在此处读取以确保不遗漏
            if (!m_Jump)
            {
                m_Jump = (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button4));
            }
            if ((isLowLadd || isUpLadd) && !isPressXKey)
            {
                isPressXKey = (Input.GetKeyDown(KeyCode.Joystick1Button2)) || (Input.GetKeyDown(KeyCode.X));
         //       print("iPressXKey:" + isPressXKey);
            }

            //判断是否在地面上
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded && m_isClimbing)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                //  PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            //不在地面上，并且不在跳跃状态
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;


            float speed;
            GetInput(out speed);
         //   print("speed = " + speed);
            Vector3 desiredMove = Vector3.zero;
            Vector3 desiredMove1 = Vector3.zero;
            Vector3 desiredMove2 = Vector3.zero;
            Transform otherTrans = null;
            // Ladder climbing logic
            // 爬梯逻辑
            if (m_isClimbing)
            {
                //this.aimTips.SetActive(true);
                Transform trLadder = m_ladderTrigger.parent;

                if (_climbingTransition != TransitionState.None)
                {
                    // Get the next point to which we have to move while we are climbing the ladder
                    //当我们爬梯子的时候，我们要到达下一个点

                    // transform.position = trLadder.Find(_climbingTransition.ToString()).position;
                    //  desiredMove1 = trLadder.Find(_climbingTransition.ToString()).position;
                    //   transform.position = desiredMove1;
                    otherTrans = trLadder.Find(_climbingTransition.ToString()).transform;


                    this.transform.DOMove(otherTrans.transform.position, 3f).OnComplete(() =>
                    {
                        StartCoroutine(WaitToUp(otherTrans));
                    });
               //     print("xyz:" + transform.position.x + "," + transform.position.y + "," + transform.position.z + ",");
                }
                else
                {
                    // Attach the player to the ladder with the rotation angle of the ladder transform
                    //把玩家固定在梯子上，用梯子的旋转角度变换
                    TiZiTextClimb.Instance.Show("按左摇杆上下梯");
               //     print("m_Input.y" + m_Input.y);
                    desiredMove = trLadder.rotation * Vector3.forward * m_Input.y;
                    // desiredMove = Vector3.forward * m_Input.y;
               //     print("desiredMove:" + desiredMove.x + "," + desiredMove.y + "," + desiredMove.z + ",");
                    //m_ClimbSpeed为爬梯速度
                    m_MoveDir.y = desiredMove.y * m_ClimbSpeed;
                    m_MoveDir.x = desiredMove.x * m_ClimbSpeed;
                    m_MoveDir.z = desiredMove.z * m_ClimbSpeed;
                    //m_MoveDir.z = 0;
                    m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
                    // m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
             //       print("climb0" + m_MoveDir.x + "," + m_MoveDir.y + "," + m_MoveDir.z + ",");
                    // m_CharacterController.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
                    // m_MoveDir = Vector3.forward;
             //       print("climb1" + m_MoveDir.x + "," + m_MoveDir.y + "," + m_MoveDir.z + ",");
                }
            }
            else
            {
                //   climbTips.SetActive(false);
                // always move along the camera forward as it is the direction that it being aimed at
                // 始终沿着相机向前移动，因为它是它被瞄准的方向
                // 根据人物的前方和右方，再乘以输入值，获得最终的方向
                // 所谓人物的前方，根据手册的说法，就是编辑器中蓝色箭头的方向（z轴正方向）；而右方就是红色箭头（x轴正方向）
                desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;
                // get a normal for the surface that is being touched to move along it
                // 得到一个正常的表面，被触摸移动它
                RaycastHit hitInfo;
                //    * Physics.SphereCast, 进行一次球形的碰撞
                //    * param:
                //    *  transform.position, 触碰的起始点
                //    *  m_CharacterController.radius, 球形的半径
                //    *  Vector3.down, 碰撞的目标
                //    *  hitInfo, 碰撞的结果
                //    *  m_CharacterController.height / 2f, 碰撞到的最大距离
                //    *  layerMask, 层次标识，~0按位取反就是全1，也就是所有的都可以碰撞
                //    *  queryTriggerInteraction, 是否要触发triiger
                //    *
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f);
                // hitInfo.normal 触碰表面的法向量
                // Vector3.ProjectOnPlane, 将一个向量投射到一个垂直于平面的法线所定义的平面上，
                // 如果路是有坡度的，角色的y轴就会有速度
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;

                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump) //要跳跃，给与一个向上的速度
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        //PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    //不在地上时，作用一个重力影响
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
                //移动！
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
                //print("move" + m_MoveDir.x + "," + m_MoveDir.y + "," + m_MoveDir.z + ",");
                // ProgressStepCycle(speed); //处理脚步声音
                // UpdateCameraPosition(speed);//更新相机的摆动
                // m_MouseLook.UpdateCursorLock(); //直观印象就是更新鼠标会不会隐藏
            }
            //  Jump();
        }
        //选择视角到正常角度

        private void RotateView()
        {
            if(IsRightHandEnabled == false)
            {
                return;
            }
            int rotateType = 0;
            if (Hi5_Glove_Gesture_Recognition.isRightFist == 1)
            {
                if (CtrlMove.RotateLeftFlag == true)
                {
                    rotateType = 1;
                }
                if (CtrlMove.RotateRightFlag == true)
                {
                    rotateType = 2;
                }
                if (CtrlMove.RotateUpFlag == true)
                {
                    rotateType = 3;
                }
                if (CtrlMove.RotateDownFlag == true)
                {
                    rotateType = 4;
                }
            }
            if(rotateType == 3)
            {
                float ver2 = 2f;
                if (ver2 > 0 && cameraPlayer.transform.localPosition.y < 2.5f)
                {

                    cameraPlayer.transform.position += transform.up * ver2 * Time.deltaTime;
                }
            }
            if (rotateType == 4)
            {
                float ver2 = -2f;
                if (ver2 < 0 && cameraPlayer.transform.localPosition.y > -1f)
                {
                    cameraPlayer.transform.position += transform.up * ver2 * Time.deltaTime;
                }
            }
            if (!m_isClimbing)
            {
                m_MouseLook.LookRotation(transform, m_Camera.transform, null, rotateType);
            }
            else
            {
                // If we are climbing let's look to the direction of the ladder
                // 如果我们在爬梯，朝着梯子的方向
               // m_MouseLook.LookRotation(transform, m_Camera.transform, m_ladderTrigger);
            }
        }

        /// <summary>
        /// transition state for ladder climbing
        /// 爬梯的过渡状态
        /// </summary>
        private enum TransitionState
        {
            None = 0,
            ToLadder1 = 1,
            ToLadder2 = 2,
            ToLadder3 = 3
        }
        private TransitionState _climbingTransition = TransitionState.None;

        void FixedUpdate()
        {
            /*
            float ver = Input.GetAxis("Vertical");
            {
               // if (ver == 0)
               // {
                   // ver = Input.GetAxis("qianhou");
                   // transform.position += transform.forward * ver * Time.deltaTime * RunSpeed;
               // }
               // else
                {
                    transform.position += transform.forward * ver * Time.deltaTime * RunSpeed;
                }
            }

            float right = Input.GetAxis("Horizontal");
            {
              //  if (right == 0)
              //  {
              //      right = Input.GetAxis("zuoyou2");
              //      transform.position += transform.right * right * Time.deltaTime * RunSpeed;
             //   }
                transform.position += transform.right * right * Time.deltaTime * RunSpeed;
            }

            float ver1 = Input.GetAxis("zuoyou");

            transform.Rotate(0, ver1 * Time.deltaTime * RotationSpeed, 0);
            */

        }

        // 获取输入，计算方向和速度
        private void GetInput(out float speed)
        {
            // Read input
            // 读取输入
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            if (Hi5_Glove_Gesture_Recognition.isLeftFist == 1)
            {
                if (CtrlMove.moveLeftFlag == true)
                {
                    horizontal = -5f;
                }
                if (CtrlMove.moveRightFlag == true)
                {
                    horizontal = 5f;
                }
                if (CtrlMove.moveForwardFlag == true)
                {
                    vertical = 5f;
                }
                if (CtrlMove.moveBackFlag == true)
                {
                    vertical = -5f;
                }


            }

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            // 在独立构建中，按键可修改步行/跑步速度。
            // 跟踪角色是否正在行走或奔跑
            // 在PC平台上运行，根据left shift按键改变移动状态（行走、奔跑）
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
           // print("m_WalkSpeed : m_RunSpeed = " + m_WalkSpeed + "AAA" + m_RunSpeed);
            // set the desired speed to be walking or running
            // 将所需的速度设置为步行或跑步

            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);
            // normalize input if it exceeds 1 in combined length:
            // 如果长度超过1，则将其归1化
            // 这边使用了 m_Input.sqrMagnitude 求出了长度的平方，相比于 m_Input.magnitude 少求一个平方根，效率高
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            // 根据相机的角度，处理速度。只有人物在奔跑的时候才处理
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }
        //控制摄像机的视角移动
        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            // 使用头部摆动的情况下处理
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void ProgressStepCycle(float speed)
        {
            // 就是在移动的情况下增加一个时间量
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }
            // 时间量没到达跳出
            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }
            // 时间量到达了播发一个脚步声音
            m_NextStep = m_StepCycle + m_StepInterval;
        }


        /// <summary>
        /// 离开触发范围会调用一次
        /// </summary>
        /// <param name="collider">参与碰撞的碰撞体</param>
        /// <returns>void</returns>
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Ladder_Bottom")
            {
                isLowLadd = false;
                TiZiTextClimb.Instance.Hide();
            }
            if (other.tag == "Ladder_Top")
            {
                TiZiTextClimb.Instance.Hide();
                //角色碰撞楼梯底部方框标识为false
                isUpLadd = false;
                if (isTop)
                {
                    isTop = false;
                    //upAndDownTip.Show();
                }
                else //(m_isClimbing&&!isPressXKey)
                {
                    if ((m_isClimbing && !isPressXKey))
                    {
                        ToggleClimbing();
                    }
                }
            }
        }

        /// <summary>
        /// 保持触发范围会持续调用
        /// </summary>
        /// <param name="collider">参与碰撞的碰撞体</param>
        /// <returns>void</returns>
        void OnTriggerStay(Collider other)
        {
            if (other.tag == "Ladder_Bottom")
            {
                //角色碰撞楼梯底部方框标识为false
                //       isLowLadd = true;
                if (isLowLadd && isPressXKey)
                {
                    isPressXKey = false;
                    //  if (isPressXKey)
                    {
                        m_ladderTrigger = other.transform;
                        if (!m_isClimbing)
                        {
                            _climbingTransition = TransitionState.ToLadder1;
                            ToggleClimbing();
                        }
                        else
                        {
                            ToggleClimbing();
                            _climbingTransition = TransitionState.None;
                        }


                    }
                    isLowLadd = false;
                }
            }

            if (other.tag == "Ladder_Top")
            {
                //角色碰撞楼梯顶部方框标识为false
                //      isUpLadd = true;
                //      TiZiTextClimb.Instance.Show("按X下梯");
                if (isUpLadd && isPressXKey)
                {
                    isUpLadd = false;
                    m_ladderTrigger = other.transform;
                    //     upAndDownTip.Show();
                    // We hit the top trigger and come from the ladder
                    // 离开或者进入梯子，击中顶部的trigger
                    if (m_isClimbing)
                    {
                        // move to the upper point and exit the ladder
                        // 如果是爬梯状态，击中顶部的trigger，状态修改为3
                        _climbingTransition = TransitionState.ToLadder3;
                    }
                    else
                    {
                        // We seem to come from above, so let's move to tha ladder (point 2) again
                        // 如果不是爬梯状态，击中顶部的trigger，状态修改为2
                        _climbingTransition = TransitionState.ToLadder2;
                        ToggleClimbing();
                        isTop = true;
                    }
                    isPressXKey = false;
                }
            }

        }
        /// <summary>
        /// 进入触发范围会调用一次
        /// </summary>
        /// <param name="collider">参与碰撞的碰撞体</param>
        /// <returns>void</returns>
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ladder_Bottom")
            {
                isLowLadd = true;
                isPressXKey = false;
                TiZiTextClimb.Instance.Show("按X上梯");
                if (m_isClimbing)
                {
                    ToggleClimbing();
                }
            }
            else if (other.tag == "Ladder_Top")
            {
                isPressXKey = false;
                isUpLadd = true;
                if (m_isClimbing)
                {
                    _climbingTransition = TransitionState.ToLadder3;
                    TiZiTextClimb.Instance.Hide();
                }
                else
                {
                    TiZiTextClimb.Instance.Show("按X下梯");
                }

            }
        }
        //状态改变
        private void ToggleClimbing()
        {
            m_isClimbing = !m_isClimbing;
        }

        IEnumerator WaitToUp(Transform otherTrans)
        {
            yield return new WaitForEndOfFrame();
            _climbingTransition = TransitionState.None;
        }
    }

}