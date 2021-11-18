using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Ladder.Scripts
{
    [Serializable]
    public class MouseLook1
    {
        //x轴和y轴的灵敏度
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        //是否锁定x轴的旋转角度，旋转的解释见下
        public bool clampVerticalRotation = true;
        //锁定的最小、最大角度
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        //是否使用插值运算
        public bool smooth;
        //插值的时间
        public float smoothTime = 5f;
        //是否要检测锁定指针
        public bool lockCursor = false;

        private float rotateToSmoothTime = 0.2f; //爬梯子时候，转向速度 0到1之间 增大
        //角色和相机的目标旋转角度
        private Quaternion m_CharacterTargetRot;

        private Quaternion m_CameraTargetRot;
        //当前鼠标指针是否锁定，只有lockCursor为true的时候，该值才会有效
        private bool m_cursorIsLocked = false;

        public float angle;

        // 初始化，传入角色和相机的旋转角度
        public void Init(Transform character, Transform camera)
        {
            //角色的旋转角度
            m_CharacterTargetRot = character.localRotation;
            //相机的旋转角度
            m_CameraTargetRot = camera.localRotation;
        }

        // FirstPersonController每帧会调用
        // 根据鼠标移动的位置调整镜头的旋转
        public void LookRotation(Transform character, Transform camera, Transform ladderTrigger)
        {
            if (ladderTrigger == null)
            {
                // 获取鼠标的位置，乘以灵敏度，获取x轴或者y轴的旋转
                float yRot = CrossPlatformInputManager.GetAxis("zuoyou") * XSensitivity;
                //左右旋转
                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            }
            else
            {
                Transform lad = null;
                Vector3 direction = ladderTrigger.forward;
                lad = ladderTrigger.transform.parent;
   //             Debug.Log("lad:"+ lad);
                direction.y = 0;

              //  Quaternion lookRotation = Quaternion.LookRotation(direction);

                angle = GetRotateAngle(lad.transform);
   //             Debug.Log("angle z: " + angle);
                //m_CharacterTargetRot = lookRotation;

                m_CharacterTargetRot = Quaternion.Euler(0, angle+180, 0);

            }
            // 获取鼠标的位置，乘以灵敏度，获取x轴或者y轴的旋转
            float xRot = -1*CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            //mod by cxl 修改左摇杆的方向 -xRot为xRot 
            //上下旋转 ，如果涉及视野旋转，使用m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
            //m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            // 锁定x轴的旋转角度
            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (ladderTrigger != null)
            {
                 character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
            rotateToSmoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    rotateToSmoothTime * Time.deltaTime);
            }
            else if (smooth) //使用插值运算平滑旋转速度
            {
                // Slerp使用球形进行插值
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else //直接赋值
            {
                //
                character.localRotation = m_CharacterTargetRot;
                //
                camera.localRotation = m_CameraTargetRot;
            }
            UpdateCursorLock();
        }

        float GetRotateAngle(Transform otherTrans)
        {
            float eulerAnglesY = otherTrans.transform.eulerAngles.y;
            return eulerAnglesY;
        }
        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {
                //we force unlock the cursor if the user disable the cursor locking helper
                // 如果用户主动设置lockCursor，即InternalLockUpdate不会影响是否锁定指针，则我们强制解锁指针
                // 也就是说，lockCursor为true时快捷键影响锁定指针，lockCursor为false的时候快捷键不影响，并强制不锁指针
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        // 鼠标指针锁定有关，FirstPersonController的Update中会调用一次外，MouseLook的LookRotation中也会调用一次
        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            //如果设置lockCursor，则检查确认是否要锁定指针
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            //按下esc，退出指针锁定
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))  //按下鼠标左键，指针锁定
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;  //指针状态变为锁定
                Cursor.visible = false; //并隐藏
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None; //释放鼠标指针
                Cursor.visible = true;
            }
        }
        // 锁定镜头的旋转角度，镜头只跟X轴旋转有关
        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}