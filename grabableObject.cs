using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabableObject : MonoBehaviour
{
    
    public Transform attachmentPoint;
    private LaunchProjectile launchScript;
    private FireBulletOnActivate fireBulletOnActivate;
    HandInteraction handInteraction;

    private bool isFiring = false;

    private void Start()
    {
        launchScript = GetComponent<LaunchProjectile>();
        fireBulletOnActivate = GetComponent<FireBulletOnActivate>();
    }

    public void FirePistol(bool value)
    {
        if (launchScript != null)
        {
            if (value && !isFiring)
            {
                Debug.Log("Success");
                launchScript.Fire();
                isFiring = true;
            }
            else if (!value && isFiring)
            {
                isFiring = false;
            }
        }
        
        // if (fireBulletOnActivate != null)
        // {
        //     if (value && !isFiring)
        //     {
        //         fireBulletOnActivate.FireBullet();
        //         isFiring = true;
        //     }
        //     else if (!value && isFiring)
        //     {
        //         isFiring = false;
        //     }
        // }
    }

    public Transform GetAttachmentPoint()
    {
        return attachmentPoint;
    }

}
