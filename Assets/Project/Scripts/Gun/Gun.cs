using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float muzzleVelocity;
    [SerializeField] private float msBetweenSpawn;
    [SerializeField] private GameObject _bulletHold;
    public float recoil;

    private float nextSpawnTime;

    public void Start()
    {   /*
        if(_bulletHold == null)
        {
            _bulletHold = GameObject.Find("BulletHold");
        }*/




    }

    public void SetBulletHold(GameObject bulletHold)
    {
        _bulletHold = bulletHold;
    }

    public bool Shoot()
    {
        if(Time.time > nextSpawnTime)
        {
        nextSpawnTime = Time.time + msBetweenSpawn/1000;
        Projectile newProject = Instantiate(projectile, muzzle.position, muzzle.rotation);
        newProject.SetSpeed(muzzleVelocity);
        return true;
        }
        return false;
    }
}
