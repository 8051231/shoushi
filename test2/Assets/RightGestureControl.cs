using Hi5_Interaction_Core;
using Ladder.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightGestureControl : MonoBehaviour
{

    // public float rotSpeed = 10f;
    // public bool isGrab = false;
    public Transform bodyTrans;
    public Transform handTrans;

    // public Transform camTrans;

    private Vector3 startVec;
    private Vector3 curVec;
    // private bool grabState;

    // public float speed = 10.0f;
    public float rotationSpeed = 10.0f;
    public float moveSpeed = 5.0f;
    public Transform obj;
    public Transform objView;

    bool start = false;
    bool startViewUpAndDown = false;
    // Use this for initialization
    void Start()
    {
      //  startVec = handTrans.localPosition - bodyTrans.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(FirstPersonController.IsRightHandEnabled == false)
        {
            return;
        }
        if ((Hi5_Glove_Gesture_Recognition.isRightFist == 0))
        {
            start = false;
        }
        if ((Hi5_Glove_Gesture_Recognition.isRightFist == 1) && start == false)
        {
            start = true;
            startVec = handTrans.localPosition - bodyTrans.localPosition;
            return;
        }
        if ((Hi5_Glove_Gesture_Recognition.isRightFist == 1))
        {
            curVec = handTrans.localPosition - bodyTrans.localPosition;
           
            Vector3 axis = curVec - startVec;

            float movey = curVec.y - startVec.y;
            if (Mathf.Abs(movey) > 0.03f)
            {
               //  float rotationV = -axis.y * rotationSpeed;
               //   rotationV *= Time.deltaTime;
               //   obj.Rotate(rotationV, 0f, 0f);
                // transform.parent.Translate(0,Time.deltaTime * movey * (16), 0);
                objView.Translate(0, Time.deltaTime * movey * -moveSpeed, 0);
            }

            if (Mathf.Abs(axis.x) > 0.01f)
            {
                float rotationH = axis.x * rotationSpeed;
                rotationH *= Time.deltaTime;
                obj.Rotate(0f, rotationH, 0f);
            }
     /*
            //float move= curVec.magnitude - startVec.magnitude;
            float movez = curVec.z - startVec.z;
            //float move= curVec.magnitude - startVec.magnitude;
            float movex = curVec.x - startVec.x;

            if (Mathf.Abs(movez) > 0.05f)
            {
                transform.parent.Translate(0, 0, Time.deltaTime * movez*(-6));
            }
            if (Mathf.Abs(movex) > 0.05f)
            {
                transform.parent.Translate(Time.deltaTime * movex * (-6), 0, 0);
            }
            Debug.Log("move:"+ movex);
  */
        }
    }                                                                    
}   
