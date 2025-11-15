using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float muzzleVelocity;
    [SerializeField] private float msBetweenSpawn;
    public float recoil;

    private float nextSpawnTime;

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
