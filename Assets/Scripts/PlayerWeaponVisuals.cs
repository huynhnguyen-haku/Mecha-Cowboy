using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator animator;
    private bool isGrabbingWeapon;

    #region Weapon Transform
    [SerializeField] private Transform[] gunTransforms;
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    private Transform currentGun;
    #endregion


    [Header("Left Hand IK Settings")]
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private float leftHandIK_WeightIncrease;
    private bool shouldIncrease_LeftHandIK_Weight;

    [Header("Rig Settings")]
    [SerializeField] private float rig_WeightIncrease;
    private bool shouldIncrease_Rig_Weight;
    private Rig rig;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();

        SwitchOnGun(pistol);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && isGrabbingWeapon == false)
        {
            animator.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();
        UpdateLeftHandWeight();
    }

    private void UpdateLeftHandWeight()
    {
        if (shouldIncrease_LeftHandIK_Weight)
        {
            leftHandIK.weight += leftHandIK_WeightIncrease * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                shouldIncrease_LeftHandIK_Weight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_Rig_Weight)
        {
            rig.weight += rig_WeightIncrease * Time.deltaTime;
            if (rig.weight >= 1)
            {
                shouldIncrease_Rig_Weight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0;
        ReduceRigWeight();
        animator.SetFloat("WeaponGrabType", (float)grabType);
        animator.SetTrigger("WeaponGrab");

        SetBusyGrabbingWeapon(true);
    }

    public void SetBusyGrabbingWeapon(bool busy)
    {
        isGrabbingWeapon = busy;
        animator.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }

    public void MaximizeRigWeight() => shouldIncrease_Rig_Weight = true;

    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandIK_Weight = true;

    private void SwitchOnGun(Transform gun)
    {
        SwitchOffGuns();
        gun.gameObject.SetActive(true);
        currentGun = gun;

        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTranform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        leftHandIK_Target.localPosition = targetTranform.localPosition;
        leftHandIK_Target.localRotation = targetTranform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        // Turn off all layers
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
        // Turn on the layer we want
        animator.SetLayerWeight(layerIndex, 1);
    }
    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOnGun(pistol);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOnGun(revolver);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOnGun(rifle);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOnGun(shotgun);
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOnGun(sniper);
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}

public enum GrabType { SideGrab, BackGrab };