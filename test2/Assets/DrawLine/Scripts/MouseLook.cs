using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
//using static Rotate;

namespace Ladder.Scripts
{
    ///<summary>
    ///��Ҫ�Ǵ�������;�ͷ����ת
    ///</summary>
    [Serializable]
    public class MouseLook
    {
        //x���y���������
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        //�Ƿ�����x�����ת�Ƕȣ���ת�Ľ��ͼ���
        public bool clampVerticalRotation = true;
        //��������С�����Ƕ�
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        //�Ƿ�ʹ�ò�ֵ����
        public bool smooth;
        //��ֵ��ʱ��
        public float smoothTime = 5f;
        //�Ƿ�Ҫ�������ָ��
        public bool lockCursor = false;

        private float rotateToSmoothTime = 8f;
        //��ɫ�������Ŀ����ת�Ƕ�
        private Quaternion m_CharacterTargetRot;

        private Quaternion m_CameraTargetRot;
        //��ǰ���ָ���Ƿ�������ֻ��lockCursorΪtrue��ʱ�򣬸�ֵ�Ż���Ч
        private bool m_cursorIsLocked = false;

        // ��ʼ���������ɫ���������ת�Ƕ�
        public void Init(Transform character, Transform camera)
        {
            //��ɫ����ת�Ƕ�
            m_CharacterTargetRot = character.localRotation;
            //�������ת�Ƕ�
            m_CameraTargetRot = camera.localRotation;
        }

        // FirstPersonControllerÿ֡�����
        // ��������ƶ���λ�õ�����ͷ����ת
        public void LookRotation(Transform character, Transform camera, Transform ladderTrigger, int rotateType)
        {
            if (ladderTrigger == null)
            {

                float yRot = 0f; 
                // ��ȡ����λ�ã����������ȣ���ȡx�����y�����ת
                //      float yRot = CrossPlatformInputManager.GetAxis("zuoyou") * XSensitivity;
                if (rotateType == 1)
                {
                    yRot = -1f * XSensitivity;
                }
                if (rotateType == 2)
                {
                    yRot = 1f * XSensitivity;
                }
                //������ת
                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            }
            else
            {
                Vector3 direction = ladderTrigger.forward;
                direction.y = 0;

                Quaternion lookRotation = Quaternion.LookRotation(direction);

                m_CharacterTargetRot = lookRotation;
            //    m_CharacterTargetRot = Quaternion.Euler(0,180,0);

            }
            // ��ȡ����λ�ã����������ȣ���ȡx�����y�����ת
            //   float xRot = -1*CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            float xRot = -1 * 1f * YSensitivity;
            //mod by cxl �޸���ҡ�˵ķ��� -xRotΪxRot 
            //������ת ������漰��Ұ��ת��ʹ��m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
            //m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            // ����x�����ת�Ƕ�
            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (ladderTrigger != null)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
            rotateToSmoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    rotateToSmoothTime * Time.deltaTime);
            }
            else if (smooth) //ʹ�ò�ֵ����ƽ����ת�ٶ�
            {
                // Slerpʹ�����ν��в�ֵ
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else //ֱ�Ӹ�ֵ
            {
                //��������
                character.localRotation = m_CharacterTargetRot;
                //��������
                camera.localRotation = m_CameraTargetRot;
            }
            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {
                //we force unlock the cursor if the user disable the cursor locking helper
                // ����û���������lockCursor����InternalLockUpdate����Ӱ���Ƿ�����ָ�룬������ǿ�ƽ���ָ��
                // Ҳ����˵��lockCursorΪtrueʱ��ݼ�Ӱ������ָ�룬lockCursorΪfalse��ʱ���ݼ���Ӱ�죬��ǿ�Ʋ���ָ��
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        // ���ָ�������йأ�FirstPersonController��Update�л����һ���⣬MouseLook��LookRotation��Ҳ�����һ��
        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            //�������lockCursor������ȷ���Ƿ�Ҫ����ָ��
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            //����esc���˳�ָ������
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))  //������������ָ������
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;  //ָ��״̬��Ϊ����
                Cursor.visible = false; //������
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None; //�ͷ����ָ��
                Cursor.visible = true;
            }
        }
        // ������ͷ����ת�Ƕȣ���ͷֻ��X����ת�й�
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