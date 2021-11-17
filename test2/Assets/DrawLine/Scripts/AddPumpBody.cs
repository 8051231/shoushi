using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPumpBody : MonoBehaviour
{
    GameObject PumpBody1 = null;
    public Material[] baseMats2;
    public Material[] normalMats2;
    void Awake()
    {
        PumpBody1 = this.transform.Find("Model/Body").gameObject;
        if (null == PumpBody1)
        {
            return;
        }
        baseMats2 = PumpBody1.GetComponent<MeshRenderer>().materials;
        if (null == baseMats2)
        {
            return;
        }
        normalMats2 = new Material[2] { new Material(Shader.Find("Standard (Specular setup)")), new Material(Shader.Find("Standard (Specular setup)")) };
        normalMats2[0] = baseMats2[0];
        PumpBody1.GetComponent<MeshRenderer>().materials = normalMats2;
    }

    /// <summary>
    /// 进入触发范围会调用一次
    /// </summary>
    /// <param name="collider">参与碰撞的碰撞体</param>
    /// <returns>void</returns>
    void OnTriggerEnter(Collider collider)
    {
        if (null == PumpBody1)
        {
            return;
        }
        PumpBody1.GetComponent<MeshRenderer>().materials = baseMats2;
        if (collider.name == "")
        {

        }
    }

    /// <summary>
    ///  碰撞结束会调用一次
    /// </summary>
    /// <param name="collider">参与碰撞的碰撞体</param>
    /// <returns>void</returns>
    void OnTriggerExit(Collider collider)
    {
        if (null == PumpBody1)
        {
            return;
        }
        PumpBody1.GetComponent<MeshRenderer>().materials = normalMats2;
    }

}
