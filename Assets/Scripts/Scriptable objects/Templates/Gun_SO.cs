using UnityEngine;


[CreateAssetMenu(menuName = "Guns_SO")]
public class Gun_SO : ScriptableObject
{
    [Header("Backfire parametres")]
    public float returnSpeed = 15.0f;
    public float snapiness = 10.0f;
    public float backFire = 0.35f;

    public float aimReturnSpeed = 15.0f;
    public float aimSnapiness = 10.0f;
    public float aimBackFire =  0.05f;

    [Header("Recoil parametres")]
    public float recoilX = -3.0f;
    public float recoilY = 3.0f;
    public float recoilZ = 0.35f;
    public float RecoilSnapiness = 6.0f;
    public float RecoilReturnSpeed = 2.0f;

    public float aimRecoilX = -3.0f;
    public float aimRecoilY = 3.0f;
    public float aimRecoilZ = 0.35f;
    public float aimRecoilSnapiness = 6.0f;
    public float aimRecoilReturnSpeed = 2.0f;

    [Header("Zoom parametres")]
    public float zoomInFOV = 15.0f;  
    public float zoomOutFOV = 60.0f;

    [Header("Misc parametres")]
    public float rateOfFire = 5.0f;
    public float damage = 15.0f;
    public float range = 10.0f;

    [Header("Ammo parametres")]
    public int carryingCapacity = 20;
    public int clipCapacity = 10;
    public AmmoType ammoType = AmmoType.Sniper;
}
