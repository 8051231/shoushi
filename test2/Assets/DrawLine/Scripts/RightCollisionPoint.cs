using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RightCollisionPoint : MonoBehaviour
{
    public RightDrawLine line;

    // Start is called before the first frame update
    void Start()
    {
        line = FindObjectOfType<RightDrawLine>();
    }

    // Update is called once per frame
    void Update()
    {
    //    transform.position = line.collisionPoint;
    }
}
