using UnityEngine;
using TMPro;

public class Projectile : Weapon
{
    public Transform shootingPoint;
    public GameObject directorComponent;
    public int quantity;
    public TMP_Text weaponWheelQuantityTMP;

    public void SpawnProjectile()
    {
        // TODO: When projectile cycling is implemented, set canHit to false
        canHit = true;
        directorComponent.SetActive(true);

        quantity--;
        if (quantity < 1) notAvailable = true;
        if (weaponWheelQuantityTMP != null) weaponWheelQuantityTMP.text = quantity.ToString();

        GameObject projectile = Instantiate(gameObject, shootingPoint.position, shootingPoint.rotation);
        directorComponent.SetActive(false);
        gameObject.SetActive(false);
    }
}
