using UnityEngine;

namespace FPS.Combat
{
    public class Recoil : MonoBehaviour
    {

        [SerializeField] private ListOfGun_SO listOfGuns = null;

        private Vector3 currentRotation = Vector3.zero;
        private Vector3 targetRotation = Vector3.zero;

        private Weapon weapon => GameObject.Find("Arm").GetComponent<Weapon>();
        private WeaponSwitcher weaponSwitcher => GameObject.Find("Weapon2").GetComponent<WeaponSwitcher>();


        void Update()
        {
            int currentWeapon = weaponSwitcher.CurrentWeapon;
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, (weapon.IsAiming ? listOfGuns.gunList[currentWeapon].aimRecoilReturnSpeed : listOfGuns.gunList[currentWeapon].RecoilReturnSpeed) * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, (weapon.IsAiming ? listOfGuns.gunList[currentWeapon].aimRecoilSnapiness : listOfGuns.gunList[currentWeapon].RecoilSnapiness) * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }

        public void RecoilFire()
        {
            int currentWeapon = weaponSwitcher.CurrentWeapon;
            if (weapon.IsAiming)
            {
                targetRotation += new Vector3(listOfGuns.gunList[currentWeapon].aimRecoilX,
                Random.Range(-listOfGuns.gunList[currentWeapon].aimRecoilY, listOfGuns.gunList[currentWeapon].aimRecoilY),
                Random.Range(-listOfGuns.gunList[currentWeapon].aimRecoilZ, listOfGuns.gunList[currentWeapon].aimRecoilZ));
            }

            else
            {
                targetRotation += new Vector3(listOfGuns.gunList[currentWeapon].recoilX,
               Random.Range(-listOfGuns.gunList[currentWeapon].recoilY, listOfGuns.gunList[currentWeapon].recoilY),
               Random.Range(-listOfGuns.gunList[currentWeapon].recoilZ, listOfGuns.gunList[currentWeapon].recoilZ));
            }
        }
    }
}

