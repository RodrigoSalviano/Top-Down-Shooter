
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{   
    public enum FireMode
    {
        Auto,
        Single,
        Burst
    }
 
    [Header("General Settings")]
    [SerializeField] private FireMode fireMode;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float msBetweenSpawn;
    [SerializeField] private GameObject _bulletHold;
    [SerializeField] private int burstCount;
    [SerializeField]private float recoil;

    [Header("Reload Settings")]
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime;
    [SerializeField] private float maxReloadAngle;

    [Header("Muzzle Settings")]
    [SerializeField] private Transform[] muzzles;
    [SerializeField] private float muzzleVelocity;
    [SerializeField] private MuzzleFlash muzzleFlash;

    [Header("Shell Settings")]
    [SerializeField] private Transform shell;
    [SerializeField] private Transform ejectionPoint;

    [Header("Recoil Settings")]
    [SerializeField] private Vector2 kickMinMax = new Vector2(.5f, .2f);
    [SerializeField] private float recoilMoveSettleTime = .1f;
    [SerializeField] private Vector2 recoilAnleMinMax = new Vector2(3f, 5f);
    [SerializeField] private float recoilRotateSettleTime = .1f;

    #region Properties
    public float Recoil => recoil;
    #endregion

    #region Control Fields
    private float _nextSpawnTime;
    private int _shootsReamainingInBurst;
    private bool _triggerRealeasedSincetLastShot;
    private Vector3 _recoilSmoothDampVelocity;
    private float _recoilAngle;
    private float _recoilAngleSmoothDampVelocity;
    private int _shotsRemainingInMagazine;
    private bool _isReloading;
    #endregion

    public void Start()
    {  
        _shootsReamainingInBurst = burstCount;
        _shotsRemainingInMagazine = magazineSize;

    }

    public void SetBulletHold(GameObject bulletHold)
    {
        _bulletHold = bulletHold;
    }

    private void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity, recoilMoveSettleTime);
        _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilAngleSmoothDampVelocity, recoilRotateSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;
    }

    private bool Shoot()
    {
        if(_shotsRemainingInMagazine > 0 && Time.time > _nextSpawnTime && !_isReloading)
        {
            if(fireMode == FireMode.Burst)
            {
                if(_shootsReamainingInBurst == 0) return false;

                _shootsReamainingInBurst--;
            }
            else if(fireMode == FireMode.Single)
            {
                if(!_triggerRealeasedSincetLastShot) return false;
            }

            for(int i = 0; i < muzzles.Length; i++)
            {   
                if(_shotsRemainingInMagazine <= 0) break;

                _shotsRemainingInMagazine--;

                Projectile newProject = Instantiate(projectile, muzzles[i].position, muzzles[i].rotation);
                newProject.SetSpeed(muzzleVelocity);
            }

            _nextSpawnTime = Time.time + msBetweenSpawn/1000;

            Instantiate(shell, ejectionPoint.position, ejectionPoint.rotation);
            muzzleFlash.Activate();

            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            _recoilAngle += Random.Range(recoilAnleMinMax.x, recoilAnleMinMax.y);
            _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 30f);

            return true;
        }
        return false;
    }

    public void Reload()
    {
        if(_isReloading || _shotsRemainingInMagazine == magazineSize) return;

        StartCoroutine(AnimateReload());

    }

    IEnumerator AnimateReload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f/ reloadTime;
        float percent = 0;
        Vector3 initialRotation = transform.localEulerAngles;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        transform.localEulerAngles = initialRotation;
        _shotsRemainingInMagazine = magazineSize;
        _isReloading = false;
    }

    public void SetRotation(Vector3 lookPoint)
    {
      transform.LookAt(lookPoint);  
    }

    public bool OnTriggerHold()
    {
        bool shootResult = Shoot();
        _triggerRealeasedSincetLastShot = false;
        return shootResult;
    }

    public void OnTriggerRealease()
    {
        _triggerRealeasedSincetLastShot = true;
        _shootsReamainingInBurst = burstCount;
    }
}
