using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRotate : MonoBehaviour
{
    public float RunSpeed = 3;
    public static bool enableRotate = false;

    //摄像机
    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //    mHand = gameObject.GetComponent<Hi5_Hand_Visible_Hand>();
        if (enableRotate)
        {
            Debug.Log("111 enableRotate");

            //print("移动2");
            float ver2 = 0.2f;
            Debug.Log("ver2=" + ver2);
            //print(transform.localPosition.y);
            if (ver2 > 0 && m_Camera.transform.localPosition.y > 0.2f)
            {

                m_Camera.transform.position += -transform.up * ver2 * Time.deltaTime;
                //print(transform.position);
            }
            /*
            if (ver2 < 0 && transform.localPosition.y < 4.4f)
                if (ver2 < 0)
                {

                    transform.position += -transform.up * ver2 * Time.deltaTime;
                    //  print(transform.position);

                }*/

        }

    }
}
