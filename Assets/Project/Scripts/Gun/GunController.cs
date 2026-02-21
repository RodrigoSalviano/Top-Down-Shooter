using UnityEngine;

public class GunController : MonoBehaviour
{   
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Gun startingGun;


    public float recoilForce;

    private Gun _equippedGun;

    void Start()
    {
        if(startingGun != null)
        {
            EquipGun(startingGun);
        }
        /*
        if(!Shoot())
        {
            SetBulletHold(weaponHolder.gameObject);
        }
        */
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(_equippedGun != null)
        {
            Destroy(_equippedGun.gameObject);
        }

        _equippedGun = Instantiate(gunToEquip, weaponHolder.position, weaponHolder.rotation);
        _equippedGun.transform.parent = weaponHolder;
        recoilForce = _equippedGun.recoil;
        _equippedGun.SetBulletHold(weaponHolder.gameObject);
    }

    public bool Shoot()
    {
        if(_equippedGun != null)
        {
            return _equippedGun.Shoot();
        }

        return false;
    }
}
