using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dunxia : MonoBehaviour
{
    public static bool IsInputEnabled = true;
    public float RunSpeed = 3;
/*    public static Vector3 m_OriginalCameraPosition;
    public static Vector3 m_OriginalCameraEulerAngles;
    public static Vector3 CameraPosition;
    public static Vector3 CameraEulerAngles;
    public static Vector3 CameralocalPosition;
    public static Vector3 CameralocalEulerAngles;
*/
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //test code
        /*
        GameObject testcameraPlayer = GameObject.Find("[CameraRig]_HI5_Interaction");
        CameraPosition = testcameraPlayer.transform.position;
        CameraEulerAngles = testcameraPlayer.transform.eulerAngles;
        CameralocalPosition = testcameraPlayer.transform.localPosition;
        CameralocalEulerAngles = testcameraPlayer.transform.localEulerAngles;
        Debug.Log("Camera Position local x" + CameralocalPosition.x + "y" + CameralocalPosition.y + "z" + CameralocalPosition.z);
        Debug.Log("Camera EulerAngles local x" + CameralocalEulerAngles.x + "y" + CameralocalEulerAngles.y + "z" + CameralocalEulerAngles.z);
        Debug.Log("Camera Position x" + CameraPosition.x + "y" + CameraPosition.y + "z" + CameraPosition.z);
        Debug.Log("Camera EulerAngles x" + CameraEulerAngles.x + "y" + CameraEulerAngles.y + "z" + CameraEulerAngles.z);
        */
        //end of test code

        if (IsInputEnabled == false)
        {
            return;
        }
        // if (GameCtrl.Instance.isUpingLadder) return;
        //  if (GameCtrl.Instance.isDowingLadder) return;
        return;
        //print("移动2");
        float ver2 = Input.GetAxis("Updown");
        Debug.Log("ver2="+ ver2);

        //print(transform.localPosition.y);
        if (ver2 > 0 && transform.parent.localPosition.y > -2f)
        {

            transform.parent.position += -transform.parent.up * ver2 * Time.deltaTime;
            //print(transform.position);
        }

        if (ver2 < 0 && transform.parent.parent.localPosition.y < 4.4f)
            if (ver2 < 0)
            {
    
                transform.parent.position += -transform.parent.up * ver2 * Time.deltaTime;
                //  print(transform.position);

            }

float ver = Input.GetAxis("Horizontal");
{
   // if (ver == 0)
   // {
       // ver = Input.GetAxis("qianhou");
       // transform.position += transform.forward * ver * Time.deltaTime * RunSpeed;
   // }
   // else
    {
        transform.parent.position += transform.parent.forward * -ver * Time.deltaTime * RunSpeed;
    }
}

float right = Input.GetAxis("Vertical");
{
  //  if (right == 0)
  //  {
  //      right = Input.GetAxis("zuoyou2");
  //      transform.position += transform.right * right * Time.deltaTime * RunSpeed;
 //   }
    transform.parent.position += transform.parent.right * right * Time.deltaTime * RunSpeed;
}



    }
}
