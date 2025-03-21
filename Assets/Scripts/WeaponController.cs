using UnityEngine;

public class WeaponControls : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += _ => Shot();
    }

    private void Shot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
