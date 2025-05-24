using System;
using UnityEngine;

public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    public Car_SFX carSounds { get; private set; }
    public Rigidbody rb { get; private set; }
    private PlayerControls controls;

    public bool carActive { get; private set; }
    [SerializeField] private LayerMask whatIsGround;

    private float moveInput;
    private float turnInput;

    // Tốc độ thực tế của Rigidbody
    public float actualSpeedMPS { get; private set; } // Mét trên giây
    public float actualSpeedKPH { get; private set; } // Kilômét trên giờ

    private WheelCollider frontLeftWheel;

    [Range(30, 60)][SerializeField] private float turnSensitivity = 30;

    [Header("Car Settings")]
    [SerializeField] private DriveType driveType;
    [SerializeField] private Transform centerOfMass;

    [Range(350, 1000)]
    [SerializeField] private float carMass;

    [Range(20, 80)]
    [SerializeField] private float wheelsMass;

    [Range(0.5f, 2)]
    [SerializeField] private float frontWheelTraction = 1;

    [Range(0.5f, 2)]
    [SerializeField] private float rearWheelTraction = 1;

    [Header("Engine Settings")]
    private float currentSpeed; // Biến này dùng cho motorTorque, không phải tốc độ hiển thị

    // These 2 parameters are mile, not kilometer (Lưu ý: maxSpeed này đang được dùng như m/s trong HandleSpeedLimit)
    [Range(4, 20)]
    [SerializeField] private float maxSpeed = 7;

    [Range(0.5f, 10)]
    [SerializeField] private float accelerationRate = 2;

    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f;

    [Header("Brake Settings")]
    public bool isBraking { get; private set; }

    [Range(0, 10)]
    public float frontBrakeSensitivity = 5;

    [Range(0, 10)]
    public float backBrakeSensitivity = 5;

    [Range(4000, 6000)]
    public float brakeForce = 5000;

    [Header("Drift Settings")]
    [Range(0, 1)]
    [SerializeField] private float frontDriftFactor = 0.5f;

    [Range(0, 1)]
    [SerializeField] private float rearDriftFactor = 0.5f;

    [SerializeField] private float driftDuration = 1;
    private float driftTimer;
    public bool isDrifting { get; private set; }
    private bool canEmitTrails = true;

    [Header("Drift Effects")]
    [SerializeField] private ParticleSystem RLWParticleSystem;
    [SerializeField] private ParticleSystem RRWParticleSystem;

    private Car_Wheel[] wheels;
    private UI ui;

    #region Unity Methods

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();
        carSounds = GetComponent<Car_SFX>();
        ui = UI.instance;

        controls = ControlsManager.instance.controls;

        ActivateCar(false);
        AssignInputEvents();
        SetupDefaultValues();
    }

    private void FixedUpdate()
    {

        if (!carActive)
        {
            DecelerateCar();
            return;
        }

        HandleWheelAnimation();
        ApplyTrailOnTheGround();
        HandleDriving();
        HandleSteering();
        HandleBraking();
        HandleSpeedLimit();

        if (isDrifting)
            HandleDrift();
        else
            StopDrift();
    }

    public void DecelerateCar()
    {
        StopDrift();
        isDrifting = false;

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 7f);
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 7f);

        HandleWheelAnimation();
    }


    private void Update()
    {
        if (!carActive)
        {
            return;
        }

        actualSpeedMPS = rb.linearVelocity.magnitude;
        actualSpeedKPH = actualSpeedMPS * 3.6f;

        UI.instance.inGameUI.UpdateSpeedText(Mathf.RoundToInt(actualSpeedKPH) + " km/h");

        driftTimer -= Time.deltaTime;
        if (driftTimer < 0)
            isDrifting = false;
    }

    #endregion

    #region Setup Methods

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass;

            if (wheel.axleType == AxelType.Front)
                wheel.SetDefaltStiffness(frontWheelTraction);

            if (wheel.axleType == AxelType.Rear)
                wheel.SetDefaltStiffness(rearWheelTraction);
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
            isDrifting = true;
            driftTimer = driftDuration;
        };

        controls.Car.Brake.canceled += ctx => isBraking = false;
        controls.Car.EnterExit.performed += ctx => GetComponent<Car_Interaction>().ExitCar();
        controls.Car.TogglePauseUI.performed += ctx => UI.instance.TogglePauseUI();
        controls.Car.ToggleMinimap.performed += ctx =>
        {
            bool isMinimapActive = UI.instance.inGameUI.minimap.activeSelf;
            UI.instance.ToggleMinimap(!isMinimapActive);
        };
    }

    #endregion

    #region Driving Methods

    private void HandleDriving()
    {
        currentSpeed = moveInput * accelerationRate * Time.deltaTime;
        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axleType == AxelType.Front)
                    wheel.cd.motorTorque = motorTorqueValue;
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axleType == AxelType.Rear)
                    wheel.cd.motorTorque = motorTorqueValue;
            }
            else if (driveType == DriveType.AllWheelDrive)
                wheel.cd.motorTorque = motorTorqueValue;
        }
    }

    private void HandleSpeedLimit()
    {
        // Giả sử maxSpeed là đơn vị m/s (Unity units/second)
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
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

    private void ApplyTrailOnTheGround()
    {
        if (!canEmitTrails)
            return;

        foreach (var wheel in wheels)
        {
            if (isDrifting || isBraking)
            {
                WheelHit hit;
                if (wheel.cd.GetGroundHit(out hit) &&
                    (whatIsGround == (whatIsGround | (1 << hit.collider.gameObject.layer))))
                {
                    if (wheel.trailRenderer != null)
                        wheel.trailRenderer.emitting = true;
                }
                else
                {
                    if (wheel.trailRenderer != null)
                        wheel.trailRenderer.emitting = false;
                }
            }
            else
            {
                if (wheel.trailRenderer != null)
                    wheel.trailRenderer.emitting = false;
            }
        }
    }

    #endregion

    #region Drift Methods

    private void HandleDrift()
    {
        foreach (var wheel in wheels)
        {
            bool backWheel = wheel.axleType == AxelType.Rear;
            float driftFactor = backWheel ? rearDriftFactor : frontDriftFactor;

            WheelFrictionCurve sidewaysFriction = wheel.cd.sidewaysFriction;
            sidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.cd.sidewaysFriction = sidewaysFriction;
        }

        DriftCarPS();
        carSounds.HandleTireSqueal(true);
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
            wheel.RestoreDefaultStiffness();

        DriftCarPS(false);
        carSounds.HandleTireSqueal(false);

        foreach (var wheel in wheels)
        {
            if (wheel.trailRenderer != null) 
                wheel.trailRenderer.emitting = false;
        }
    }

    private void DriftCarPS(bool isDrifting = true)
    {
        try
        {
            if (isDrifting)
            {
                RLWParticleSystem?.Play();
                RRWParticleSystem?.Play();
            }
            else
            {
                RLWParticleSystem?.Stop();
                RRWParticleSystem?.Stop();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    #endregion

    #region Animation Methods

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

    #endregion

    #region Public Methods

    public void ActivateCar(bool active)
    {
        carActive = active;
        if (carSounds != null)
            carSounds.ActivateCarSFX(active);
    }

    public void BreakCar()
    {
        canEmitTrails = false;

        foreach (var wheel in wheels)
        {
            if (wheel.trailRenderer != null) // Thêm kiểm tra null
                wheel.trailRenderer.emitting = false;
        }

        carSounds.ActivateCarSFX(false);

        motorForce = 0;
        rb.linearDamping = 1;
        frontDriftFactor = 0.9f;
        rearDriftFactor = 0.9f;
    }

    #endregion
}