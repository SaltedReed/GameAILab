using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Sandbox
{

    public class WeaponUser : MonoBehaviour
    {
        [Header("Weapon")]
        public Transform weaponRoot;
        [Tooltip("The first is the default weapon")]
        public GameObject[] weaponPrefabs;
        public bool createWeaponsOnStart = true;

        public Weapon CurrentWeapon { get; protected set; }
        public Weapon[] Weapons { get; protected set; }


        public void CreateWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            for (int i=0; i<weaponPrefabs.Length; ++i)
            {
                if (weaponPrefabs[i] is null)
                {
                    Debug.LogWarning($"{gameObject.name} has a null weapon prefab");
                    continue;
                }

                GameObject go = Instantiate(weaponPrefabs[i]);
                Weapon w = go.GetComponent<Weapon>();
                if (w is null)
                {
                    Debug.LogWarning($"{gameObject.name} instantiated a weapon game object without Weapon script attached");
                    continue;
                }

                weapons.Add(w);
            }

            Weapons = weapons.ToArray();
            if (Weapons.Length > 0)
            {
                AttachWeapon(Weapons[0]);
            }
        }

        public void AttachWeapon(Weapon weapon)
        {
            if (weapon is null)
            {
                throw new NullReferenceException();
            }

            if (CurrentWeapon != null)
            {
                DetachWeapon();
            }

            weapon.transform.SetParent(weaponRoot, false);
            weapon.Owner = gameObject;
            CurrentWeapon = weapon;
            CurrentWeapon.OnAttach();
            CurrentWeapon.gameObject.SetActive(true);
        }

        public void DetachWeapon()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Owner = null;
                CurrentWeapon.OnDetach();
                CurrentWeapon.gameObject.SetActive(false);
                CurrentWeapon = null;
            }
        }

        public void Shoot(Vector3 dir)
        {
            ShooterWeapon sw = CurrentWeapon as ShooterWeapon;
            if (sw is null)
            {
                Debug.LogError($"{gameObject.name} cannot shoot, because its current weapon is not ShooterWeapon");
                return;
            }

            sw.Shoot(dir, OnDamageLand);
        }

        protected void OnDamageLand(PointDamage dmg)
        {
            // for test
            Debug.Log("damage land");
        }

        protected void Start()
        {
            if (createWeaponsOnStart)
            {
                CreateWeapons();
            }
        }

        // for test
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F");
                Shoot(transform.forward);
            }
        }

    }

}