using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Sandbox
{

    public enum WeaponShootType
    {
        Manual,
        //Automatic,
    }


    public class ShooterWeapon : Weapon
    {
        [Header("Internal References")]
        [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
        public GameObject weaponRoot;
        [Tooltip("Tip of the weapon, where the projectiles are shot")]
        public Transform weaponMuzzle;

        [Header("Shoot Parameters")]
        //[Tooltip("The type of weapon wil affect how it shoots")]
        //public WeaponShootType shootType;
        public bool generateProjectile;
        [Tooltip("The projectile prefab")]
        public Projectile projectilePrefab;
        //[Tooltip("In degree")]
        //public float bulletSpreadAngle = 0f;
        //public float verticalRecoil = 0f;
        public float maxRange = 30;

        [Header("Ammo Parameters")]
        [Tooltip("Amount of ammo reloaded each time")]
        public int ammoPerReload = 4;
        [Tooltip("Maximum amount of ammo in the gun")]
        public int maxAmmo = 8;

        [Header("Damage")]
        [Tooltip("Basic damage per bullet")]
        public float damage = 10;

        [Header("Audio & Visual")]
        [Tooltip("Optional weapon animator for OnShoot animations")]
        public string k_animShoot;
        public string k_animReload;
        [Tooltip("Prefab of the muzzle flash")]
        public GameObject muzzleFlashPrefab;
        [Tooltip("Prefab of the hit effect")]
        public GameObject hitEffectPrefab;
        [Tooltip("sound played when shooting")]
        public AudioClip shootSFX;

        protected AudioSource m_shootAudioSource;
        protected Animator m_animator;


        public int Ammo
        {
            get => m_ammo;
            set
            {
                int old = m_ammo;
                m_ammo = Mathf.Clamp(value, 0, maxAmmo);
                onAmmoChange?.Invoke(this, old, m_ammo);
            }
        }
        protected int m_ammo;

        public Action<ShooterWeapon, int, int> onAmmoChange;
        public Action<ShooterWeapon> onShoot;


        public bool CanShoot()
        {
            return Ammo > 0;
        }

        public void Shoot(Vector3 dir, Action<PointDamage> onPointDmgLand = null)
        {
            if (!CanShoot())
            {
                Debug.LogWarning(gameObject.name + " cannot shoot");
                return;
            }

            SimShoot();

            --Ammo;

            onShoot?.Invoke(this);

            Vector3 correctDir = dir.normalized;
            PointDamageParams pointDmgParams = new PointDamageParams
            {
                instigator = Owner,
                weapon = this,
                friendTag = Owner.tag,
                basicDamage = damage,
                onDamageLand = onPointDmgLand
            };
            if (!generateProjectile)
            {
                WeaponUtility.TryHitScanPointDamage(new HitScanParams
                {
                    origin = transform.position,
                    direction = correctDir,
                    maxDistance = maxRange
                }, pointDmgParams);
            }
            else
            {
                WeaponUtility.TryProjectilePointDamage(new ProjectileParams
                {
                    projectilePrefab = projectilePrefab.gameObject,
                    origin = weaponMuzzle.position,
                    direction = correctDir,
                    lifetime = projectilePrefab.lifetime
                }, pointDmgParams);
            }
        }

        protected void SimShoot()
        {
            if (m_animator != null)
            {
                m_animator.SetTrigger(k_animShoot);
            }

            if (muzzleFlashPrefab != null)
            {
                VfxManager.SpawnParticles(muzzleFlashPrefab, weaponMuzzle.position, weaponMuzzle.rotation, 2.0f);
            }

            if (shootSFX != null && m_shootAudioSource != null)
            {
                m_shootAudioSource.PlayOneShot(shootSFX);
            }
        }

        public void Reload()
        {
            if (Ammo < maxAmmo)
            {
                SimReload();
                Ammo += ammoPerReload;
            }
        }

        protected void SimReload()
        {
            m_animator?.SetTrigger(k_animReload);
        }

        protected virtual void Start()
        {
            m_animator = GetComponentInChildren<Animator>();
            m_shootAudioSource = GetComponentInChildren<AudioSource>();

            Ammo = maxAmmo;
        }

    }

}
