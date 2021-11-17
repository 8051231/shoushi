using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dunxia1 : MonoBehaviour
{


    public bool isDown;
    public bool isUp;


    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        
       // if (GameCtrl.Instance.isUpingLadder) return;
      //  if (GameCtrl.Instance.isDowingLadder) return;
      
        //print("移动2");
        float ver2 = Input.GetAxis("Updown");

       //print(transform.localPosition.y);
        if (ver2 > 0 && transform.localPosition.y > -1.9f)
        {
            isDown = true;
            transform.position += -transform.up * ver2 * Time.deltaTime;
            //print(transform.position);
        }

         if (ver2 < 0 && transform.localPosition.y < 2.4f)
        if (ver2 < 0 )
        {
            isUp = true;
            transform.position += -transform.up * ver2 * Time.deltaTime;
          //  print(transform.position);

        }
    }
}
