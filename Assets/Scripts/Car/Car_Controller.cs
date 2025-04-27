using System;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    public float speed;

    public float turnSensitivity;

    [Header("Car Settings")]
    [SerializeField] private Transform centerOfMass;

    [Header("Engine Settings")]
    public float currentSpeed;

    [Range(7, 12)] public float maxSpeed;
    [Range(0.5f, 5)] public float accelerationRate;
    [Range(1500, 3000)] public float motorForce = 1500f;

    [Header("Brake Settings")]
    private bool isBraking;
    [Range(4, 10)] public float brakeSensitivity =5;
    [Range(4000, 6000)] public float brakeForce = 5000;

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
    }

    private void Update()
    {
        speed = rb.linearVelocity.magnitude; 
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
            if (wheel.axleType == AxelType.Rear) // Apply brake force to rear wheels, may be adjusted to front wheel
            {
                wheel.cd.brakeTorque = currentBrakeTorque;
            }
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

        controls.Car.Brake.performed += ctx => isBraking = true;
        controls.Car.Brake.canceled += ctx => isBraking = false;
    }
}
