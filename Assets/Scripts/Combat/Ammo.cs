using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType { AKM, ShotGun, Sniper}
namespace FPS.Combat
{
    public class Ammo : MonoBehaviour
    {

        [SerializeField] private List<AmmoSlot> m_AmmoSloList = new List<AmmoSlot>();

        [System.Serializable]
        private class AmmoSlot
        {
            public AmmoType ammoType;
            public int carryingCapacityAmount;
            public int clipCapacityAmount;
            public int currentCarryingAmount;
            public int currentClipAmount;

        }

        public int GetCurrentCarryingAmount(AmmoType ammoType)
        {
            return GetAmmoSlot(ammoType).currentCarryingAmount;
        }

        public int GetCurrentClipAmount(AmmoType ammoType)
        {
            return GetAmmoSlot(ammoType).currentClipAmount;
        }

        public void DecreaseAmmoClip(AmmoType ammoType, int amount)
        {
            GetAmmoSlot(ammoType).currentClipAmount -= amount;
        }

        public void DecreaseAmmoCarrying(AmmoType ammoType, int amount)
        {
            GetAmmoSlot(ammoType).currentCarryingAmount -= amount;
        }

        public void AddAmmoClip(AmmoType ammoType, int amount)
        {
            GetAmmoSlot(ammoType).currentClipAmount += amount;
        }

        public void AddAmmoCarrying(AmmoType ammoType, int amount)
        {

        }

        private AmmoSlot GetAmmoSlot(AmmoType ammoType)
        {
            foreach (AmmoSlot slot in m_AmmoSloList)
            {
                if (slot.ammoType == ammoType)
                {
                    return slot;
                }
            }
            return null;
        }
    }

}
