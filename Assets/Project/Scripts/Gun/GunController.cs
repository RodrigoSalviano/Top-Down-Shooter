using UnityEngine;

public class GunController : MonoBehaviour
{   
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Gun[] allGuns;
    [SerializeField]private float recoilForce;

    public float RecoilForce => recoilForce;

    private Gun _equippedGun;

    public void Reload()
    {
        if(_equippedGun != null)
        {
            _equippedGun.Reload();
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(_equippedGun != null)
        {
            Destroy(_equippedGun.gameObject);
        }

        _equippedGun = Instantiate(gunToEquip, weaponHolder.position, weaponHolder.rotation);
        _equippedGun.transform.parent = weaponHolder;
        recoilForce = _equippedGun.Recoil;
        //_equippedGun.SetBulletHold(weaponHolder.gameObject);
    }

    public void EquipGun(int gunIndex)
    {
        if(gunIndex < allGuns.Length)
        {
            EquipGun(allGuns[gunIndex]);
        }
    }

    public bool OnTriggerHold()
    {
        if(_equippedGun != null)
        {
            return _equippedGun.OnTriggerHold();
        }
        return false;
    }

    public void OnTriggerRealease()
    {
        if(_equippedGun != null)
        {
            _equippedGun.OnTriggerRealease();
        }
    }

    public void SetRotation(Vector3 lookPoint)
    {   
        if(_equippedGun != null)
        {
            _equippedGun.SetRotation(lookPoint);
        } 
    }
}
