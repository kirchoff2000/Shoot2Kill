using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.Combat
{
    public class Weapon : MonoBehaviour
    {

        [Header("Scriptable Objects")]
        [SerializeField] private MuzzleFlash_SO muzzles = null;
        [SerializeField] private ListOfGun_SO listOfGuns = null;
        [SerializeField] private AudioCollection audioCollection = null;
        [SerializeField] private GameObject sniper = null;
        [SerializeField] private GameObject arm = null;
        [Header("")]
        [SerializeField] private List<Transform> muzzlePosition = new List<Transform>();

        //Private
        private Animator armAnimator => GameObject.Find("Arm").GetComponent<Animator>();
        private WeaponSwitcher weaponSwitcher => GameObject.Find("Weapon2").GetComponent<WeaponSwitcher>();
        private Ammo ammo => GameObject.Find("Player").GetComponent<Ammo>();
        private BackFire backFire => GameObject.Find("BackFire").GetComponent<BackFire>();
        private Recoil recoil => GameObject.Find("Recoil").GetComponent<Recoil>();
        private PlayerNetwork player => GameObject.Find("Player").GetComponent<PlayerNetwork>();

        private GameObject generalCrosshair => GameObject.Find("point");
        private GameObject sniperCrosshair => GameObject.Find("SniperCrossHair");
        private GameObject sniperMask => GameObject.Find("SniperMask");

        private Camera mainCamera => GameObject.Find("Main Camera").GetComponent<Camera>();

        private bool isAiming = false;
        private bool isReloading = false;
        private float fireTimer = 0.0f;

        private int hashAiming => Animator.StringToHash("aiming");
        private int hashReload => Animator.StringToHash("reload");
        private int hashKnifeAttackType => Animator.StringToHash("KnifeAttackType");
        private int hashKnifeAttack => Animator.StringToHash("KnifeAttack");

        //Public
        public bool IsAiming { get { return isAiming; } }
        public bool IsReloading { get { return isReloading; } }



        void Update()
        {
            ManageInputs();
        }

        private void ManageInputs()
        {
            fireTimer += Time.deltaTime;

            ProcessAiming();
            ProcessReload();
            ProcessShoot();
            ProcessMelee();
        }

        private void ProcessMelee()
        {
            if (Input.GetKeyDown(KeyCode.Q) && weaponSwitcher.IsChangingWeapon == false)
            {
                armAnimator.SetInteger(hashKnifeAttackType, Random.Range(1, 4));
                armAnimator.SetTrigger(hashKnifeAttack);
            }
        }

        private void ProcessShoot()
        {
            int currentWeapon = weaponSwitcher.CurrentWeapon;
            if (Input.GetMouseButton(0) && fireTimer >= 1 / listOfGuns.gunList[currentWeapon].rateOfFire && isReloading == false && weaponSwitcher.IsChangingWeapon == false)
            {
                if (ammo.GetCurrentClipAmount(listOfGuns.gunList[currentWeapon].ammoType) > 0)
                {
                    Shoot();
                    ammo.DecreaseAmmoClip(listOfGuns.gunList[currentWeapon].ammoType, 1);
                }
                else
                {
                    ProcessAmmunition(currentWeapon);
                }
            }
        }

        private void ProcessReload()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ProcessAmmunition(weaponSwitcher.CurrentWeapon);
            }
        }

        private void ProcessAiming()
        {
            if (Input.GetMouseButtonDown(1) && player.IsGrounded == true)
            {
                isAiming = !isAiming;

                generalCrosshair.GetComponent<Image>().enabled = !isAiming;
                armAnimator.SetBool(hashAiming, isAiming);
                if (isAiming)
                {
                    StartCoroutine(OnScoped());
                }
                else
                {
                    OnUnscoped();
                }
            }
        }

        private void ProcessAmmunition(int currentWeapon)
        {
            int amountToFillClip = listOfGuns.gunList[currentWeapon].clipCapacity - ammo.GetCurrentClipAmount(listOfGuns.gunList[currentWeapon].ammoType);
            if (amountToFillClip == 0) return;

            if (ammo.GetCurrentCarryingAmount(listOfGuns.gunList[currentWeapon].ammoType) > 0)
            {
                StartCoroutine(executeReloadAnim());

                int amountCarrying = ammo.GetCurrentCarryingAmount(listOfGuns.gunList[currentWeapon].ammoType);

                ammo.AddAmmoClip(listOfGuns.gunList[currentWeapon].ammoType, Mathf.Min(amountToFillClip, amountCarrying));
                ammo.DecreaseAmmoCarrying(listOfGuns.gunList[currentWeapon].ammoType, Mathf.Min(amountToFillClip, amountCarrying));
            }
            else
            {
                print("Fuck! No more ammo!");
            }
        }

        private void Shoot()
        {
            backFire.ExecuteBackFire();
            recoil.RecoilFire();
            ProcessFireSound();
            ProcessMuzzleFlash();
            fireTimer = 0.0f;
        }

        private void ProcessFireSound()
        {
            AudioManager.Instance.PlayOneShotSound(AudioManager.Instance.SfxPool, audioCollection.fireSound[weaponSwitcher.CurrentWeapon], transform);
        }

        private void ProcessMuzzleFlash()
        {
            int currentWeapon = weaponSwitcher.CurrentWeapon;
            int rand = Random.Range(0, muzzles.muzzleFlash.Count);
            Instantiate(muzzles.muzzleFlash[rand], muzzlePosition[currentWeapon].position, muzzlePosition[currentWeapon].rotation, muzzlePosition[currentWeapon]);
        }

        private IEnumerator OnScoped()
        {
            int currentWeapon = weaponSwitcher.CurrentWeapon;
            StartCoroutine(lerpFOV(listOfGuns.gunList[currentWeapon].zoomInFOV, 0.5f));
            yield return new WaitForSeconds(0.3f);

            if (currentWeapon == 2)
            {
                sniper.SetActive(false);                
                arm.SetActive(false);
                sniperCrosshair.GetComponent<Image>().enabled = true;
                sniperMask.GetComponent<Image>().enabled = true;
            }
        }

        private void OnUnscoped()
        {
            int currentWeapon = weaponSwitcher.CurrentWeapon;

            if (currentWeapon == 2)
            {
                sniperCrosshair.GetComponent<Image>().enabled = false;
                sniperMask.GetComponent<Image>().enabled = false;
                sniper.SetActive(true);
                arm.SetActive(true);
            }
            StartCoroutine(lerpFOV(listOfGuns.gunList[currentWeapon].zoomOutFOV, 0.3f));
        }

        private IEnumerator executeReloadAnim()
        {
            if (isAiming)
            {
                isAiming = false;
                armAnimator.SetBool(hashAiming, isAiming);
            }
            armAnimator.SetTrigger(hashReload);
            isReloading = true;
            AudioManager.Instance.PlayOneShotSound(AudioManager.Instance.SfxPool, audioCollection.reloadSound[Random.Range(0, audioCollection.reloadSound.Count)], transform);
            yield return new WaitForSeconds(1.5f);
            isReloading = false;
        }

        private IEnumerator lerpFOV(float toFOV, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                float fOVTime = counter / duration;
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, toFOV, fOVTime);
                yield return null;
            }
        }

    }   
}

