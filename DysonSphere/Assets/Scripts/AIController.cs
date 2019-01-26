﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RotateMe))]
public class AIController : MonoBehaviour
{
    private const float CLOSE_DISTANCE = 20;
    private const float TARGET_DISTANCE = 4;
    private const float CLOSE_ANGLE = 0.35f;
    private const float SQR_CLOSE_DISTANCE = CLOSE_DISTANCE * CLOSE_DISTANCE;
    private const float SQR_TARGET_DISTANCE = TARGET_DISTANCE * TARGET_DISTANCE;

    private float speed = 0.7f;

    private RotateMe rotateMe;

    private void Start()
    {
        rotateMe = GetComponent<RotateMe>();
    }

    private void Update()
    {
        Vector3 target = GetBestResourceSource(transform.position);
        if (target != null && !float.IsNaN(target.x) && !float.IsNaN(target.y) && !float.IsNaN(target.z))
            Go(target);
        else
            ClearTarget();
    }

    private void Go(Vector3 target)
    {
        float targetThrustVelocity = 0;

        float sqrDistance = (target - transform.position).sqrMagnitude;
        if (rotateMe.DeltaAngle < CLOSE_ANGLE && rotateMe.DeltaAngle > -CLOSE_ANGLE)
        {
            float offCourseDamper = Mathf.Abs(rotateMe.DeltaAngle) / CLOSE_ANGLE;

            if (sqrDistance > SQR_CLOSE_DISTANCE)
                targetThrustVelocity = offCourseDamper;
            else if (sqrDistance > SQR_TARGET_DISTANCE)
                targetThrustVelocity = (sqrDistance - SQR_TARGET_DISTANCE) / (SQR_CLOSE_DISTANCE - SQR_TARGET_DISTANCE) * offCourseDamper;
        }

        rotateMe.LockOn = true;
        rotateMe.ThrustVel = targetThrustVelocity;
        rotateMe.RotSpeed = 500f;
        rotateMe.Target = target;
    }

    private void ClearTarget()
    {
        rotateMe.LockOn = false;
    }

    private static Vector3 GetBestResourceSource(Vector3 source)
    {
        Planet[] planets = Planet.Planets;

        Vector3 bestPosition = Vector3.zero;
        float bestScore = 0;
        foreach (Planet planet in planets)
        {
            Vector3 toTarget = planet.transform.position - source;
            float sqrToTargetDistance = toTarget.sqrMagnitude;
            float score = planet.TotalResources / sqrToTargetDistance;

            if (score > bestScore)
            {
                bestPosition = planet.transform.position;
                bestScore = score;
            }
        }
        return bestPosition;
    }

    private static Vector3 GetDirectionToClosestPlanet(Vector3 source)
    {
        return GetClosest(source, Planet.PlanetPositions) - source;
    }

    private static Vector3 GetClosest(Vector3 source, Vector3[] targets)
    {
        Vector3 closest = Vector3.zero;
        float closestSquareDistance = float.MaxValue;
        foreach (Vector3 target in targets)
        {
            Vector3 toTarget = target - source;
            float sqrToTargetDistance = toTarget.sqrMagnitude;

            if (sqrToTargetDistance < closestSquareDistance)
            {
                closest = target;
                closestSquareDistance = sqrToTargetDistance;
            }
        }
        return closest;
    }
}