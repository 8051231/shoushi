using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil
{
    public static Vector3 CalculateVelocity(Vector3 force, float mass)
    {
        var velocity = (force / mass) * Time.fixedDeltaTime;
		return velocity;
    }
    public static Vector3 CalculatePosition(Vector3 velocity, float time, bool useGravity)
    {
        var distance = velocity * time;
        if (useGravity)
        {
            var hight = 0.5f * Physics.gravity * time * time;
            distance += hight;
        }
        return distance;
    }
}
