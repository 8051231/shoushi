using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using Famen;

public class GameCtrl : MonoBehaviour
{
    public static GameCtrl Instance;
    public GameObject menFaCam;
    public string menFaName;
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.T)|| Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
          //  TipText.Instance.ShowOrHide();
        }

        if ( Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
          //  TestText.Instance.ShowOrHide();
        }



        Vector3 screenPos = Camera.main.WorldToScreenPoint(GameObject.Find("FirstPersonCrossHair").transform.position);
        screenPos.z = 0;
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        //Vector3 fwd = GameObject.Find("CrossHairSprite").transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        //if (Physics.Raycast(GameObject.Find("CrossHairSprite").transform.position, fwd, out hit, 2f))
        if (Physics.Raycast(ray, out hit,2f))
         {

            Debug.DrawLine(GameObject.Find("FirstPersonCrossHair").transform.position, hit.point, Color.red);
            if (hit.collider.gameObject.tag == "MenFa")
            {

          //      Debug.DrawLine(GameObject.Find("FirstPersonCrossHair").transform.position, hit.point, Color.red);
                if (hit.collider.gameObject.name.Contains("@MV1") )
                {


                    //  hit.collider.transform.Find("Model/Handle").GetComponent<FaMenRotate>().SetColor(true);
                    hit.collider.transform.gameObject.GetComponent<FaMenRotate>().SetColor(true);
                    menFaName = hit.transform.name;
                    
                   

                }
                else if (hit.collider.transform.parent.name.Contains("PumpType"))
                //else if (hit.collider.gameObject.tag == "Pump")
                {
             //       hit.collider.gameObject.GetComponent<FaMenRotate>().SetColor(true);
                    menFaName = hit.transform.name;
                    // print(menFaName);

                }
                else if (hit.collider.gameObject.name.Contains("@TY") || hit.collider.gameObject.name.Contains("@L1"))
                {
              //      hit.collider.gameObject.GetComponent<FaMenRotate>().SetColor(true);
                    menFaName = hit.transform.name;
                    //  hit.collider.gameObject.GetComponent<FaMenRotate>().ShowOrHideShuiWeiText(true);
                }
                else if (hit.collider.gameObject.name.Contains("@MV2"))
                {
                    // hit.collider.transform.Find("Model/Handle").GetComponent<FaMenRotate>().SetColor(true);
                //    hit.collider.transform.gameObject.GetComponent<FaMenRotate>().SetColor(true);
                    menFaName = hit.transform.name;

                }
                else if (hit.collider.gameObject.name.Contains("@TV2") || hit.collider.gameObject.name.Contains("@T1"))
                {
                //    hit.collider.gameObject.GetComponent<FaMenRotate>().SetColor(true);
                    menFaName = hit.transform.name;

                }

                else if (hit.collider.gameObject.name.Contains("@F1"))
                {
               //     hit.collider.gameObject.GetComponent<FaMenRotate>().SetColor(true);
                    menFaName = hit.transform.name;
                    //    hit.collider.gameObject.GetComponent<FaMenRotate>().ShowOrHideShuiWeiText(true);
                }
                else if (hit.collider.gameObject.name.Contains("DSG_DSG_MeiJiangBeng")|| hit.collider.gameObject.name.Contains("polySurface1289"))
                {
                //    hit.collider.gameObject.GetComponent<Information>().SetColor(true);
                    menFaName = hit.transform.name;
                    //    hit.collider.gameObject.GetComponent<FaMenRotate>().ShowOrHideShuiWeiText(true);
                }
            }
            else
            {
                GameCtrl.Instance.menFaName = "";
                
                FaMenPercentText.Instance.Hide();
            }
        }
        else
        {
            GameCtrl.Instance.menFaName = "";
            
            FaMenPercentText.Instance.Hide();
        }
    }
}
