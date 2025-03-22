using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    [Header("Left Hand IK Settings")]
    [SerializeField] private Transform leftHand;

    private Transform currentGun;

    private void Start()
    {
        SwitchOnGun(pistol);
        
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOnGun(pistol);
            SwitchAnimationLayer(1);
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOnGun(revolver);
            SwitchAnimationLayer(1);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOnGun(rifle);
            SwitchAnimationLayer(1);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOnGun(shotgun);
            SwitchAnimationLayer(2);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOnGun(sniper);
            SwitchAnimationLayer(3);
        }       
    }

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
        leftHand.localPosition = targetTranform.localPosition;
        leftHand.localRotation = targetTranform.localRotation;
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
}
