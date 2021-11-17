using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LeftCollisionPoint : MonoBehaviour
{
    public LeftDrawLine line;

    // Start is called before the first frame update
    void Start()
    {
        line = FindObjectOfType<LeftDrawLine>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = line.collisionPoint;
    }
}
