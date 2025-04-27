using System;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    public float speed;

    [Range(30, 60)][SerializeField] private float turnSensitivity;

    [Header("Car Settings")]
    [SerializeField] private Transform centerOfMass;

    [Header("Engine Settings")]
    public float currentSpeed;

    [Range(7, 12)][SerializeField] private float maxSpeed;
    [Range(0.5f, 5)][SerializeField] private float accelerationRate;
    [Range(1500, 3000)][SerializeField] private float motorForce = 1500f;

    [Header("Brake Settings")]
    private bool isBraking;
    [Range(4, 10)] public float brakeSensitivity = 5;
    [Range(4000, 6000)] public float brakeForce = 5000;

    [Header("Drift Settings")]
    [Range(0, 1)][SerializeField] private float frontDriftFactor = 0.5f;
    [Range(0, 1)][SerializeField] private float rearDriftFactor = 0.5f;
    [SerializeField] private float driftDuration = 1;
    private float driftTimer;

    private Car_Wheel[] wheels;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
        controls = ControlsManager.instance.controls;
        ControlsManager.instance.SwitchToCarControls();

        wheels = GetComponentsInChildren<Car_Wheel>();

        AssignInputEvents();
    }

    private void FixedUpdate()
    {
        HandleWheelAnimation();
        HandleDriving();
        HandleSteering();
        HandleBraking();
        HandleSpeedLimit();

        if (isBraking)
        {
            HandleDrift();
        }
        else
        {
            StopDrift();
        }
    }

    private void Update()
    {
        speed = rb.linearVelocity.magnitude;
        driftTimer -= Time.deltaTime;
        if (driftTimer < 0)
        {
            isBraking = false;
        }

    }


    private void HandleDriving()
    {
        currentSpeed = moveInput * accelerationRate * Time.deltaTime;
        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (wheel.axleType == AxelType.Rear) // Apply motor torque to rear wheels
            {
                wheel.cd.motorTorque = motorTorqueValue;
            }

            if (wheel.axleType == AxelType.Front)// Apply motor torque to front wheels
            {
                wheel.cd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void HandleSpeedLimit()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void HandleSteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axleType == AxelType.Front)
            {
                float targetSteeringAngle = turnInput * turnSensitivity;
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteeringAngle, 0.5f);
            }
        }
    }

    private void HandleBraking()
    {
        float newBrakeForce = brakeForce * brakeSensitivity * Time.deltaTime;
        float currentBrakeTorque = isBraking ? newBrakeForce : 0;

        foreach (var wheel in wheels)
        {
            if (wheel.axleType == AxelType.Rear) // Apply brake force to front wheels, may be adjusted to rear wheel
            {
                wheel.cd.brakeTorque = currentBrakeTorque;
            }
        }
    }

    private void HandleDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axleType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : rearDriftFactor;

            WheelFrictionCurve sidewaysFriction = wheel.cd.sidewaysFriction;
            sidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.cd.sidewaysFriction = sidewaysFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffness();
        }
    }

    private void HandleWheelAnimation()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.cd.GetWorldPose(out position, out rotation);

            if (wheel.model != null)
            {
                wheel.model.transform.position = position;
                wheel.model.transform.rotation = rotation;
            }
        }
    }


    private void AssignInputEvents()
    {
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            moveInput = input.y;
            turnInput = input.x;
        };

        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0;
            turnInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += ctx => isBraking = false;
    }
}
