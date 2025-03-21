using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchOnGun(pistol);
        
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchOnGun(revolver);
      
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchOnGun(rifle);

        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchOnGun(shotgun);

        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchOnGun(sniper);
        
    }

    private void SwitchOnGun(Transform gun)
    {
        SwitchOffGuns();
        gun.gameObject.SetActive(true);
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }
}
