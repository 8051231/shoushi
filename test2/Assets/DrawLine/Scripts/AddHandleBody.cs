using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHandleBody : MonoBehaviour
{
    GameObject HandleBody1 = null;
    public Material[] baseMats2;
    public Material[] normalMats2;
    static public int colliderFlag = 0;
    void Awake()
    {
        HandleBody1 = this.transform.Find("Model/Handle").gameObject;
        if (null == HandleBody1)
        {
            return;
        }
        baseMats2 = HandleBody1.GetComponent<MeshRenderer>().materials;
        if (null == baseMats2)
        {
            return;
        }
        normalMats2 = new Material[2] { new Material(Shader.Find("Standard (Specular setup)")), new Material(Shader.Find("Standard (Specular setup)")) };
        normalMats2[0] = baseMats2[0];
        HandleBody1.GetComponent<MeshRenderer>().materials = normalMats2;
    }

    /// <summary>
    /// 进入触发范围会调用一次
    /// </summary>
    /// <param name="collider">参与碰撞的碰撞体</param>
    /// <returns>void</returns>
    void OnTriggerEnter(Collider collider)
    {
        if (null == HandleBody1)
        {
            return;
        }
        HandleBody1.GetComponent<MeshRenderer>().materials = baseMats2;
        colliderFlag = 1;
    }
    void OnTriggerStay(Collider collider)
    {
        if (null == HandleBody1)
        {
            return;
        }
        colliderFlag = 1;
    }
    /// <summary>
    ///  碰撞结束会调用一次
    /// </summary>
    /// <param name="collider">参与碰撞的碰撞体</param>
    /// <returns>void</returns>
    void OnTriggerExit(Collider collider)
    {
        if (null == HandleBody1)
        {
            return;
        }
        HandleBody1.GetComponent<MeshRenderer>().materials = normalMats2;
        colliderFlag = 0;
    }

}
