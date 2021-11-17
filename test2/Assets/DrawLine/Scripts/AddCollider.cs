using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimedpos = this.transform.position;
        RaycastHit[] hits = Physics.SphereCastAll(aimedpos, (float)1, transform.forward, (float)2);
        for (int i = 0; i < hits.Length; ++i)
        {
           // if (transform.gameObject.tag == "MenFa")
            {
                  print("name = " + hits[i].collider.name);
            }
            if (hits[i].transform.GetComponent<MeshRenderer>())
            {
                print("以准心为原点，半径为1米球体，扫描长度2米范围内碰撞到了什么 = " + hits[i].collider.name);
                // hits[i].transform.GetComponent<MeshRenderer>().material.color = Color.grey;
            }
        }
    }
}
