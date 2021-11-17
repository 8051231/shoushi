using HI5;
using Hi5_Interaction_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlMove : MonoBehaviour
    {
        public Transform CameraRig_HI5_Interaction;
        public static bool IsCtrlMoveEnabled = true;
        static public bool moveLeftFlag = false;
        static public bool moveRightFlag = false;
        static public bool moveForwardFlag = false;
        static public bool moveBackFlag = false;
        static public bool moveDownFlag = false;
        static public bool moveUpFlag = false;

        static public bool RotateLeftFlag = false;
        static public bool RotateRightFlag = false;
        static public bool RotateUpFlag = false;
        static public bool RotateDownFlag = false;

        public float leftRunSpeed = 0.5f;
        public float rightRunSpeed = 0.5f;
        public float forwardRunSpeed = 0.5f;
        public float backRunSpeed = 0.5f;
        public float upAndDownSpeed = 0.1f;
        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.SetActive(true);
            //Hide();
           this.gameObject.transform.localScale = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (Hi5_Glove_Gesture_Recognition.isLeftFist == 0)
            {
                Debug.Log("12345AAAA");
                return;
                /*  moveLeftFlag = false;
                  moveRightFlag = false;
                  moveRightFlag = false;
                  moveBackFlag = false;
                //  return;*/


            }
            if (IsCtrlMoveEnabled == false)
            {
                return;
            }
        return;
            if (moveLeftFlag)
            {
                // CameraRig_HI5_Interaction.transform.position = Vector3.MoveTowards(CameraRig_HI5_Interaction.transform.position, CameraRig_HI5_Interaction.transform.forward, Time.deltaTime);
                CameraRig_HI5_Interaction.transform.position += CameraRig_HI5_Interaction.transform.forward * Time.deltaTime * leftRunSpeed;
                //  float ver2 = 0.8f;
                //print(transform.localPosition.y);
                //  if (ver2 > 0 && CameraRig_HI5_Interaction.localPosition.y > -2f)
                // {
                //向下
                //    CameraRig_HI5_Interaction.transform.position += -CameraRig_HI5_Interaction.transform.up * Time.deltaTime * RunSpeed;
                //向上
                //  CameraRig_HI5_Interaction.transform.position += CameraRig_HI5_Interaction.transform.up * ver2 * Time.deltaTime;
                //向左右  -向左，+向右
                //  CameraRig_HI5_Interaction.transform.position += CameraRig_HI5_Interaction.transform.forward * ver2 * Time.deltaTime * RunSpeed;
                //前后 -向后，+向前
                //  CameraRig_HI5_Interaction.transform.position += -CameraRig_HI5_Interaction.transform.right * ver2 * Time.deltaTime * RunSpeed;

                //print(transform.position);
                //    }
            }
            if (moveRightFlag)
            {
                CameraRig_HI5_Interaction.transform.position += -CameraRig_HI5_Interaction.transform.forward * Time.deltaTime * rightRunSpeed;
            }
            if (moveForwardFlag)
            {
                CameraRig_HI5_Interaction.transform.position += CameraRig_HI5_Interaction.transform.right * Time.deltaTime * forwardRunSpeed;
            }
            if (moveBackFlag)
            {
                CameraRig_HI5_Interaction.transform.position += -CameraRig_HI5_Interaction.transform.right * Time.deltaTime * backRunSpeed;
            }

            // flag = false;

        }
        private void OnTriggerStay(Collider other)
        {
            if (other.transform.parent.name == "Human_LeftHand")
            {
                if ("CubeLeft" == transform.name)
                {
                    //   HI5_Manager.EnableLeftVibration(200);
                    moveLeftFlag = true;
                }

                if ("CubeRight" == transform.name)
                {
                    moveRightFlag = true;
                }
                if ("CubeForward" == transform.name)
                {
                    moveForwardFlag = true;
                }
                if ("CubeBack" == transform.name)
                {
                    moveBackFlag = true;
                }
                if ("CubeUp" == transform.name)
                {
                    moveUpFlag = true;
                }
                if ("CubeDown" == transform.name)
                {
                    moveDownFlag = true;
                }
            }
        if (other.transform.parent.name == "Human_RightHand")
        {
            if ("CubeLeft" == transform.name)
            {
                //   HI5_Manager.EnableLeftVibration(200);
                RotateLeftFlag = true;
            }
            if ("CubeRight" == transform.name)
            {
                //   HI5_Manager.EnableLeftVibration(200);
                RotateRightFlag = true;
            }
            if ("CubeUp" == transform.name)
            {
                RotateUpFlag = true;
            }
            if ("CubeDown" == transform.name)
            {
                RotateDownFlag = true;
            }
        }
    }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent.name == "Human_LeftHand")
            {
                if ("CubeLeft" == transform.name)
                {
                    //   HI5_Manager.EnableLeftVibration(200);
                    moveLeftFlag = true;
                }

                if ("CubeRight" == transform.name)
                {
                    moveRightFlag = true;
                }
                if ("CubeForward" == transform.name)
                {
                    moveForwardFlag = true;
                }
                if ("CubeBack" == transform.name)
                {
                    moveBackFlag = true;
                }
                if ("CubeUp" == transform.name)
                {
                    moveUpFlag = true;
                }
                if ("CubeDown" == transform.name)
                {
                    moveDownFlag = true;
                }
            }
            if (other.transform.parent.name == "Human_RightHand")
            {
                if ("CubeLeft" == transform.name)
                {
                    //   HI5_Manager.EnableLeftVibration(200);
                    RotateLeftFlag = true;
                }
                if ("CubeRight" == transform.name)
                {
                    //   HI5_Manager.EnableLeftVibration(200);
                    RotateRightFlag = true;
                }
                if ("CubeUp" == transform.name)
                {
                    RotateUpFlag = true;
                }
                if ("CubeDown" == transform.name)
                {
                    RotateDownFlag = true;
                }
            }
    }
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.parent.name == "Human_LeftHand")
            {
                if ("CubeLeft" == transform.name)
                {
                    moveLeftFlag = false;
                }
                if ("CubeRight" == transform.name)
                {
                    moveRightFlag = false;
                }
                if ("CubeForward" == transform.name)
                {
                    moveForwardFlag = false;
                }
                if ("CubeBack" == transform.name)
                {
                    moveBackFlag = false;
                }
                if ("CubeUp" == transform.name)
                {
                    moveUpFlag = false;
                }
                if ("CubeDown" == transform.name)
                {
                    moveDownFlag = false;
                }
            }
        if (other.transform.parent.name == "Human_RightHand")
        {
            if ("CubeLeft" == transform.name)
            {
                //   HI5_Manager.EnableLeftVibration(200);
                RotateLeftFlag = false;
            }
            if ("CubeRight" == transform.name)
            {
                //   HI5_Manager.EnableLeftVibration(200);
                RotateRightFlag = false;
            }
            if ("CubeUp" == transform.name)
            {
                RotateUpFlag = false;
            }
            if ("CubeDown" == transform.name)
            {
                RotateDownFlag = false;
            }
        }
    }
        /*
        void OnCollisionEnter(Collision collision)
        {

            if ("CubeLeft" == transform.name)
            {
             //   HI5_Manager.EnableLeftVibration(200);
                moveLeftFlag = true;
            }
            if ("CubeRight" == transform.name)
            {
                moveRightFlag = true;
            }
            if ("CubeForward" == transform.name)
            {
                moveForwardFlag = true;
            }
            if ("CubeBack" == transform.name)
            {
                moveBackFlag = true;
            }
        }

        void OnCollisionExit(Collision collision)
        {

            if ("CubeLeft" == transform.name)
            {
                moveLeftFlag = false;
            }
            if ("CubeRight" == transform.name)
            {
                moveRightFlag = false;
            }
            if ("CubeForward" == transform.name)
            {
                moveForwardFlag = false;
            }
            if ("CubeBack" == transform.name)
            {
                moveBackFlag = false;
            }
        }
         */
        void Show()
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }

        void Hide()
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }

    }
