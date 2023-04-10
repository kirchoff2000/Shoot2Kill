using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Combat
{
    public class WeaponSwitcher : MonoBehaviour
    {

        //private     
        private Animator armAnimator => GameObject.Find("Arm").GetComponent<Animator>();
        private Weapon weapon => GameObject.Find("Arm").GetComponent<Weapon>();
        private PlayerNetwork player => GameObject.Find("Player").GetComponent<PlayerNetwork>();

        private int currentWeapon = 0;

        private bool isChangingWeapon = false;

        //Public
        public bool IsChangingWeapon { get { return isChangingWeapon; } }
        public int CurrentWeapon { get { return currentWeapon; } }



        void Start()
        {
            SetWeaponActive();
        }

        void Update()
        {
            int previousWeapon = currentWeapon;

            ManageKeyInputs();
            ManageScrollWheel();

            if (previousWeapon != currentWeapon)
            {
                SetWeaponActive();
            }
        }

        private void ManageScrollWheel()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && weapon.IsReloading == false && player.IsGrounded && weapon.IsAiming == false)
            {
                if (currentWeapon >= transform.childCount - 1)
                {
                    currentWeapon = 0;
                }
                else
                {
                    currentWeapon++;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && weapon.IsReloading == false && player.IsGrounded && weapon.IsAiming == false)
            {
                if (currentWeapon <= 0)
                {
                    currentWeapon = transform.childCount - 1;
                }
                else
                {
                    currentWeapon--;
                }
            }
        }

        private void ManageKeyInputs()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && weapon.IsReloading == false && player.IsGrounded && weapon.IsAiming == false)
            {
                currentWeapon = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && weapon.IsReloading == false && player.IsGrounded && weapon.IsAiming == false)
            {
                currentWeapon = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && weapon.IsReloading == false && player.IsGrounded && weapon.IsAiming == false)
            {
                currentWeapon = 2;
            }
        }

        private void SetWeaponActive()
        {
            int weaponIndex = 0;

            foreach (Transform weapon in transform)
            {
                if (weaponIndex == currentWeapon)
                {
                    armAnimator.SetTrigger("ChangeWeapon");
                    armAnimator.SetInteger("WeaponType", weaponIndex);
                    StartCoroutine(ExecuteSwitchWeapon(weapon, true, 0.6f));
                }
                else
                {
                    StartCoroutine(ExecuteSwitchWeapon(weapon, false, 0.6f));
                }
                weaponIndex++;
            }
        }

        private IEnumerator ExecuteSwitchWeapon(Transform gun, bool state, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            gun.gameObject.SetActive(state);
        }
    }
}

