using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Sandbox
{

    public struct HitResult
    {
        public Collider collider;
        public Vector3 point;
        public Vector3 normal;
    }


    public struct HitScanParams
    {        
        public Vector3 origin;
        public Vector3 direction;
        public float maxDistance;        
        public Action<HitResult> onHit;
    }

    public struct ProjectileParams
    {
        public GameObject projectilePrefab;
        public Vector3 origin;
        public Vector3 direction;
        public float lifetime;
        public Action<HitResult> onHit;
    }

    public struct PointDamageParams
    {
        public GameObject instigator;
        public Weapon weapon;
        public string friendTag;
        public float basicDamage;
        public Action<PointDamage> onDamageLand;
    }

    public struct PointDamage
    {
        public GameObject damagedGo;
        public GameObject instigator;
        public Weapon weapon;
        public Vector3 point;
        public Vector3 normal;
        public float amount;
    }


    public class WeaponUtility
    {
        private static bool HitScan(Vector3 origin, Vector3 dir, float maxDistance, out HitResult result)
        {
            result = new HitResult();

            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, maxDistance))
            {
                result.collider = hit.collider;
                result.point = hit.point;
                result.normal = hit.normal;
                return true;
            }
            return false;
        }

        private static bool FireProjectile(GameObject prefab, Vector3 origin, Vector3 dir, float lifetime, out Projectile proj)
        {
            // todo: pool
            GameObject projGo = GameObject.Instantiate(prefab);
            if (projGo is null)
            {
                proj = null;
                return false;
            }

            proj = projGo.GetComponent<Projectile>();
            if (proj is null)
                return false;

            proj.transform.position = origin;
            proj.transform.up = dir;
            GameObject.Destroy(projGo, lifetime);

            return true;
        }

        public static void HitScan(HitScanParams hitScanParams)
        {
            HitResult result;
            if (HitScan(hitScanParams.origin, hitScanParams.direction, hitScanParams.maxDistance, out result))
            {
                hitScanParams.onHit?.Invoke(result);
            }
        }

        public static void FireProjectile(ProjectileParams projParams)
        {
            Projectile proj;
            if (FireProjectile(projParams.projectilePrefab, projParams.origin, projParams.direction, projParams.lifetime, out proj))
            {
                proj.onHit += projParams.onHit;
            }
        }

        public static void TryHitScanPointDamage(HitScanParams hitScanParams, PointDamageParams pointDmgParams)
        {
            HitResult result;
            if (!HitScan(hitScanParams.origin, hitScanParams.direction, hitScanParams.maxDistance, out result))
            {
                return;
            }

            hitScanParams.onHit?.Invoke(result);
            TryPointDamage(result, pointDmgParams);
        }

        public static void TryProjectilePointDamage(ProjectileParams projParams, PointDamageParams pointDmgParams)
        {
            Projectile proj;
            if (FireProjectile(projParams.projectilePrefab, projParams.origin, projParams.direction, projParams.lifetime, out proj))
            {
                proj.onHit += projParams.onHit;
                proj.onHit += (HitResult result) => 
                    { 
                        TryPointDamage(result, pointDmgParams); 
                        GameObject.Destroy(proj.gameObject, 0.1f); // todo: param
                    };
            }
        }

        private static void TryPointDamage(HitResult result, PointDamageParams pointDmgParams)
        {
            if (!string.IsNullOrEmpty(pointDmgParams.friendTag)
                && pointDmgParams.friendTag != "Untagged"
                && result.collider.gameObject.tag == pointDmgParams.friendTag)
            {
                Debug.Log($"{pointDmgParams.instigator.name} hit a friend");
                return;
            }

            PointDamage damage = new PointDamage
            {
                damagedGo = result.collider.gameObject,
                instigator = pointDmgParams.instigator,
                weapon = pointDmgParams.weapon,
                point = result.point,
                normal = result.normal,
                amount = pointDmgParams.basicDamage
            };

            pointDmgParams.onDamageLand?.Invoke(damage);

        }
    }

}