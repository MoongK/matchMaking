using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : NetworkBehaviour {
    public GameObject weapon;
    public GameObject shellPrefab;
    public Transform shellEjection;
    public float fireRate = 10;
    public Light muzzleFlash;
    public GameObject impactFX;
    public GameObject bulletHolePrefab;

    Camera fpsCamera;
    float nextTimeToFire = 0f;
    Vector3 originPos;

    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public float recoilMoveSettleTime = 0.1f;
    Vector3 recoilSmoothVelocity;


    enum Hitpart
    {
        Head = 0, Body
    }


    private void Awake()
    {
        fpsCamera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        originPos = weapon.transform.localPosition;
    }

    void Update () {
        if (!isLocalPlayer)
            return;

	    if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            CmdShoot();
        }
	}

    [Command]
    void CmdShoot()
    {
        RpcFireEffects();
        CheckHit();
    }

    [ClientRpc]
    void RpcFireEffects()
    {
        MakeMuzzleFlash();
        MakeShell();
        PlayGunSound();
        Kick();
    }

    void MakeMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        Invoke("OffFlashLight", 0.05f);
    }
    void OffFlashLight()
    {
        muzzleFlash.enabled = false;
    }
    
    void MakeShell()
    {
        GameObject clone = Instantiate(shellPrefab, shellEjection);
        clone.transform.parent = null;
    }

    void PlayGunSound()
    {
        weapon.GetComponent<AudioSource>().Play();
    }

    void Kick()
    {
        weapon.transform.localPosition -= Vector3.forward * UnityEngine.Random.Range(0.07f, 0.3f);
    }

    void CheckHit()
    {
        RaycastHit hit;
        if(Physics.Raycast(fpsCamera.transform.position, 
                           fpsCamera.transform.forward, 
                           out hit, 
                           200f,
                           1 << LayerMask.NameToLayer("Floor") | 
                           1 << LayerMask.NameToLayer("Player") |
                           1 << LayerMask.NameToLayer("Mob")))
        {
            //print(hit.transform.name);
            NetworkIdentity ni = hit.transform.GetComponentInParent<NetworkIdentity>();
            if (ni != null)
            {
                if (hit.collider.CompareTag("Head"))
                    RpcHitReaction(ni.netId, hit.point, hit.normal, GetComponent<NetworkIdentity>().netId, Hitpart.Head);
                else
                    RpcHitReaction(ni.netId, hit.point, hit.normal, GetComponent<NetworkIdentity>().netId, Hitpart.Body);
            }
            Health health = hit.transform.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(10);
        }
    }

    [ClientRpc]
    void RpcHitReaction(NetworkInstanceId netId, Vector3 point, Vector3 normal, NetworkInstanceId attackerid, Hitpart part)
    {
        GameObject hit = ClientScene.FindLocalObject(netId);
        GameObject attacker = ClientScene.FindLocalObject(attackerid);

        if (hit != null && attacker != null)
        {
            if (hit.GetComponent<Rigidbody>() != null)
            {
                hit.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * 200f);
            }
            BulletSound bs = hit.GetComponent<BulletSound>();
            if (bs != null)
                bs.Play();
            BulletRandomSound brs = hit.GetComponent<BulletRandomSound>();
            if (brs != null)
                brs.Play();

            Hitreaction hr = hit.GetComponent<Hitreaction>();
            if(hr != null)
            {
                MobController mc = hit.GetComponent<MobController>();
                if(mc != null)
                {
                    mc.attacker = attacker;
                }
                hr.React(part.ToString());
            }

            MakeImpactFX(point, normal);
            MakeBulletHole(point, normal, hit.transform);
        }
    }

    void MakeImpactFX(Vector3 point, Vector3 normal)
    {
        GameObject fx = Instantiate(impactFX, point, Quaternion.LookRotation(normal));
        Destroy(fx, 0.3f);
    }

    void MakeBulletHole(Vector3 point, Vector3 normal, Transform parent)
    {
        GameObject clone = Instantiate(bulletHolePrefab, point+normal*0.1f, 
                                       Quaternion.FromToRotation(-Vector3.forward, normal));
        clone.transform.SetParent(parent, true);
        Destroy(clone, 3f);
    }

    private void LateUpdate()
    {
        weapon.transform.localPosition = Vector3.SmoothDamp(weapon.transform.localPosition,
                                                            originPos,
                                                            ref recoilSmoothVelocity,
                                                            recoilMoveSettleTime);
    }
}
