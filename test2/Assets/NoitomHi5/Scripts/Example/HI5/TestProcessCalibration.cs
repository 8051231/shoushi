using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HI5;
public class TestProcessCalibration : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        Hi5_Message.GetInstance().RegisterMessage( MessageFun, Hi5_Message.Hi5_MessageMessageKey.messageCalibrationResult);
    }
   
    private void OnDisable()
    {
        Hi5_Message.GetInstance().UnRegisterMessage(MessageFun, Hi5_Message.Hi5_MessageMessageKey.messageCalibrationResult);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            Debug.Log("BPose.....................");
            gameObject.GetComponent<HI5_Glove_Calibration_Process_Interface>().StartCalibration(HI5_Pose.BPose);
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            Debug.Log("PPPPPPPPPPPPPPPPPose.....................");
            gameObject.GetComponent<HI5_Glove_Calibration_Process_Interface>().StartCalibration(HI5_Pose.PPose);
        }
    }

    void MessageFun(string messageKey, object param1, object param2)
    {
        if (string.Compare(messageKey, Hi5_Message.Hi5_MessageMessageKey.messageCalibrationResult) == 0)
        {
            if (param1 is CalibrationInterfaceResult)
            {
                CalibrationInterfaceResult result = param1 as CalibrationInterfaceResult;
                if (result.result == ECalibrationInterfaceResult.EBposFailed)
                {
                    Debug.Log("ECalibrationInterfaceResult.EBposFailed");
                }
                else if (result.result == ECalibrationInterfaceResult.EBposSuccess)
                {
                    Debug.Log("ECalibrationInterfaceResult.EBposSuccess");
                }
                else if (result.result == ECalibrationInterfaceResult.EPposComplete)
                {
                    Debug.Log("ECalibrationInterfaceResult.EPposComplete");
                }
            }
        }
    }
}
