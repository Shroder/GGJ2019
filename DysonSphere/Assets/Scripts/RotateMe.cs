﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour
{
    public Vector3 Target;
    public float RotSpeed = 200f;

    public float DeltaAngle { get; private set; }

    public float ThrustPos = 0f;
    public float ThrustVel = 0f;

    public Vector2 ShipVel { get { return _parentRB.velocity; } private set { } }

    private Rigidbody2D _parentRB;

    private void Start()
    {
        _parentRB = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            var targetRotationLookAt = (transform.parent.position - Target).normalized;
            DeltaAngle = Mathf.Acos(Vector3.Dot(transform.up, targetRotationLookAt));
            var axis = Vector3.Cross(transform.up, targetRotationLookAt);

            var amountToRotateBy = Mathf.Clamp(DeltaAngle, -Time.deltaTime * RotSpeed, Time.deltaTime * RotSpeed);
            transform.RotateAround(transform.parent.position, axis, amountToRotateBy);
        }
        else
        {
            float translation = ThrustPos * RotSpeed;

            Vector3 axis;
            if (translation > 0)
                axis = new Vector3(0, 0, -1);
            else
                axis = new Vector3(0, 0, 1);

            transform.RotateAround(transform.parent.position, axis, Time.deltaTime * Mathf.Abs(translation));
        }

        Vector2 direction = transform.localPosition.normalized;
        _parentRB.AddForce(direction * -10f * ThrustVel);
    }
}
