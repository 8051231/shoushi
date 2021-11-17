using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]

public class RightDrawLine : MonoBehaviour
{
    public static bool enableRightLine = false;
    public LineRenderer line;
    public int pointCount = 10;
    public float velocity = 100f;
    public float scale = 1f;
    public Vector3 collisionPoint;
    public string collisionTag = "TransPlane";
    public Transform player;
    //摄像机
    private Camera m_Camera;
    private Vector3 m_OriginalCameraPosition;
    private Vector3 moveDirection = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        m_Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //    Camera.main.transform.localPosition = Vector3.zero;
        Debug.Log("x:" + Camera.main.transform.localPosition.x + "y:" + Camera.main.transform.localPosition.y + "z:" + Camera.main.transform.localPosition.z);
        if (enableRightLine)
        {
            line.enabled = true;
            CalculateCastLine();
            CalculateCollision();
        }
        else
        {
            line.enabled = false;
            player.position = collisionPoint;
        }
        enableRightLine = false;

    }

    void CalculateCastLine()
    {
        line.positionCount = pointCount;

        Vector3 v3v = transform.forward * velocity;
        for (int i = 0; i < pointCount; i++)
        {
            float t = i * scale;
            float pz = v3v.z * t;
            float px = v3v.x * t;
            float py = v3v.y * t - 5f * t * t;
            line.SetPosition(i, new Vector3(px, py, pz) + transform.position);
        }
    }

    void CalculateCollision()
    {
        for (int i = pointCount - 1; i > 0; i--)
        {
            Vector3 startPoint = line.GetPosition(i - 1);
            Vector3 endPoint = line.GetPosition(i);

            Vector3 outVector = endPoint - startPoint;
            RaycastHit hit;
            Debug.DrawRay(startPoint, outVector.normalized, Color.red);
            if (Physics.Raycast(startPoint, outVector.normalized, out hit, outVector.magnitude))
            {
                if (hit.collider.tag == collisionTag)
                {
                    collisionPoint = hit.point;
                    Debug.Log("Hit position: " + hit.point);
                    break;
                }
            }
        }
    }
}
