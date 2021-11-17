using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRaycast : MonoBehaviour
{

    // To show/hide the crosshair from scripts 
    public static bool DisplayCrosshair = false;

    //鼠标在x/y轴上的偏移量
    private float RotationX;
    private float RotationY;

    //鼠标灵敏度
    public float SensitivityX = 10;
    public float SensitivityY = 10;

    //鼠标在y轴方向偏移范围
    public float max = 60;
    public float min = -60;

    public KeyCode submit = KeyCode.KeypadEnter;

    public float leftRightMoveScale = 1;
    public float upDownMoveScale = 1;
    public Camera eventCam;
    Vector3 lastMouse;
    private bool isButtonPressed;
    Transform pivot;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.gameObject.SetActive(true);
        lastMouse = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
      //  if (DisplayCrosshair)
        {
            KeyControl();
            Debug.DrawRay(transform.position, transform.position - eventCam.transform.position, Color.red);
           // CurvedUIInputModule.CustomControllerRay = new Ray(transform.position, transform.position - eventCam.transform.position);
          //  CurvedUIInputModule.CustomControllerButtonState = isButtonPressed;
        }
    }

    public float speed = 2;  // 速度

    private void KeyControl()
    {
        /*
        if (Input.GetAxis("Mouse Y") > 0f)
        {
            transform.RotateAround(transform.parent.transform.position, Vector3.left, moveScale * Time.deltaTime);
        }
        if (Input.GetAxis("Mouse Y") < 0f)
        {
            transform.RotateAround(transform.parent.transform.position, Vector3.right, moveScale * Time.deltaTime);
        }*/
        /*
        if (Input.GetAxis("Mouse XX") > 0f)
        {
            transform.RotateAround(transform.parent.transform.position, Vector3.up, leftRightMoveScale * Time.deltaTime);
        }
        if (Input.GetAxis("Mouse XX") < 0f)
        {
            transform.RotateAround(transform.parent.transform.position, Vector3.down, leftRightMoveScale * Time.deltaTime);
        }
        if (Input.GetAxis("Mouse YY") > 0f)
        {
            transform.localPosition += new Vector3(0f, Time.deltaTime * upDownMoveScale, 0f);
        }
        if (Input.GetAxis("Mouse YY") < 0f)
        {
            transform.localPosition -= new Vector3(0f, Time.deltaTime * upDownMoveScale, 0f);
        }*/
        /*  if (Input.GetAxis("Mouse X") > 0f)
         {
             transform.localPosition += new Vector3(Time.deltaTime * moveScale, 0f, 0f);
         }
         if (Input.GetAxis("Mouse X") < 0f)
         {
             transform.localPosition -= new Vector3(Time.deltaTime * moveScale, 0f, 0f);
         }*/

        /*
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        this.transform.localEulerAngles += new Vector3(-v, h, 0) * moveScale;
        */
        if (Input.GetKey(submit))
        {
            isButtonPressed = true;
        }
        else
        {
            isButtonPressed = false;
        }
    }
}
