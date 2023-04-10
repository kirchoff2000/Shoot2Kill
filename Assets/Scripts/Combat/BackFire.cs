using UnityEngine;

namespace FPS.Combat
{
    public class BackFire : MonoBehaviour
    {
        [SerializeField] private ListOfGun_SO listOfGuns = null;

        private Vector3 targetPosition = Vector3.zero;
        private Vector3 currentPosition = Vector3.zero;
        private Vector3 initialPosition = Vector3.zero;

        private Weapon weapon => GameObject.Find("Arm").GetComponent<Weapon>();
        private WeaponSwitcher weaponSwitcher => GameObject.Find("Weapon2").GetComponent<WeaponSwitcher>();

        void Start()
        {
            initialPosition = transform.localPosition;
        }

        void Update()
        {
            targetPosition = Vector3.Lerp(targetPosition, initialPosition, listOfGuns.gunList[weaponSwitcher.CurrentWeapon].returnSpeed * Time.deltaTime);
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, listOfGuns.gunList[weaponSwitcher.CurrentWeapon].snapiness * Time.fixedDeltaTime);
            transform.localPosition = currentPosition;
        }

        public void ExecuteBackFire()
        {
            targetPosition += new Vector3(0, 0, weapon.IsAiming ? -listOfGuns.gunList[weaponSwitcher.CurrentWeapon].aimBackFire : -listOfGuns.gunList[weaponSwitcher.CurrentWeapon].backFire);
        }
    }
}


