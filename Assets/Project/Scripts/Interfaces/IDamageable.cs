using UnityEngine;

public interface IDamageable
{
    public void TakeHit(float damage, RaycastHit hit);
    //public void TakeHit(RaycastHit hit);
    //public void TakeHit(float damage);
}