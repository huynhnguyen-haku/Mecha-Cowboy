using System;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.XR;

public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    private bool carActive;
    private PlayerControls controls;
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    public float speed;

    [Range(30, 60)][SerializeField] private float turnSensitivity = 30;

    [Header("Car Settings")]
    [SerializeField] private DriveType driveType;
    [SerializeField] private Transform centerOfMass;

    [Range(350, 1000)][SerializeField] private float carMass;
    [Range(20, 80)][SerializeField] private float wheelsMass;

    [Range(0.5f, 2)][SerializeField] private float frontWheelTraction = 1;
    [Range(0.5f, 2)][SerializeField] private float rearWheelTraction = 1;


    [Header("Engine Settings")]
    public float currentSpeed;

    [Range(7, 12)][SerializeField] private float maxSpeed = 7;
    [Range(0.5f, 10)][SerializeField] private float accelerationRate = 2;
    [Range(1500, 5000)][SerializeField] private float motorForce = 1500f;

    [Header("Brake Settings")]
    public bool isBraking;
    [Range(0, 10)] public float frontBrakeSensitivity = 5;
    [Range(0, 10)] public float backBrakeSensitivity = 5;
    [Range(4000, 6000)] public float brakeForce = 5000;

    [Header("Drift Settings")]
    [Range(0, 1)][SerializeField] private float frontDriftFactor = 0.5f;
    [Range(0, 1)][SerializeField] private float rearDriftFactor = 0.5f;
    [SerializeField] private float driftDuration = 1;
    private float driftTimer;
    private bool isDrifting;

    private Car_Wheel[] wheels;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();

        controls = ControlsManager.instance.controls;
        ControlsManager.instance.SwitchToCarControls();

        AssignInputEvents();
        SetupDefaultValues();
    }

    private void FixedUpdate()
    {
        if (!carActive) return;

        HandleWheelAnimation();
        HandleDriving();
        HandleSteering();
        HandleBraking();
        HandleSpeedLimit();

        if (isDrifting)
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
        if (!carActive)
            return;

        speed = rb.linearVelocity.magnitude;
        driftTimer -= Time.deltaTime;
        if (driftTimer < 0)
        {
            isDrifting = false;
        }

    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass;

            if (wheel.axleType == AxelType.Front)
            {
                wheel.SetDefaltStiffness(frontWheelTraction);
            }
            if (wheel.axleType == AxelType.Rear)
            {
                wheel.SetDefaltStiffness(rearWheelTraction);
            }
        }
    }

    private void HandleDriving()
    {
        currentSpeed = moveInput * accelerationRate * Time.deltaTime;
        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            // Use for FWD Car
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axleType == AxelType.Front)
                {
                    wheel.cd.motorTorque = motorTorqueValue;
                }
            }

            // Use for RWD Car
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axleType == AxelType.Rear)
                {
                    wheel.cd.motorTorque = motorTorqueValue;
                }
            }

            // Use for AWD Car
            else
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
        foreach (var wheel in wheels)
        {
            bool backBrakes = wheel.axleType == AxelType.Rear;
            float brakeSensitivity = backBrakes ? backBrakeSensitivity : frontBrakeSensitivity;

            float newBrakeForce = brakeForce * brakeSensitivity * Time.deltaTime;
            float currentBrakeTorque = isBraking ? newBrakeForce : 0;

            wheel.cd.brakeTorque = currentBrakeTorque;
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

    public void ActivateCar(bool active)
    {
        carActive = active;
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
            isDrifting = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += ctx => isBraking = false;
    }

    [ContextMenu("Focus Camera On Car")]
    public void TestThiosCar()
    {
        ActivateCar(true);
        CameraManager.instance.ChangeCameraTarget(transform, 12);
    }
}
