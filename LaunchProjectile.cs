using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class LaunchProjectile : MonoBehaviour
    {
        //public AudioClip audioClip;
        //public float frequency;
        float lastShotTime;
        bool reload = false;
        //public BallTrackerHelper ballTrackerHelper;
        //public TrialSequence trialCount;
        ////public Manager shots;
        
        [SerializeField]
        [Tooltip("The projectile that's created")]
        GameObject m_ProjectilePrefab = null;

        [SerializeField]
        [Tooltip("The point that the project is created")]
        Transform m_StartPoint = null;

        [SerializeField]
        [Tooltip("The speed at which the projectile is launched")]
        float m_LaunchSpeed = 1.0f; //supposed to be 1.0f
        [SerializeField]
        GameObject target;

        // public void Fire()
        // {
        //     if(Time.time > lastShotTime + frequency)
        //     {
        //     GameObject newObject = Instantiate(m_ProjectilePrefab, m_StartPoint.position, m_StartPoint.rotation, null);
            
        //     if (newObject.TryGetComponent(out Rigidbody rigidBody))
        //     {
        //         ApplyForce(rigidBody);
        //         AudioSource.PlayClipAtPoint(audioClip, transform.position);
        //     }
        //     lastShotTime = Time.time;
        //     }
        // }

        private void Start()
        {
            //trialCount = FindObjectOfType<TrialSequence>();
        }

        public void Fire()
        {
            if(reload == false)
            {
            GameObject newObject = Instantiate(m_ProjectilePrefab, m_StartPoint.position, m_StartPoint.rotation, null);
            
            if (newObject.TryGetComponent(out Rigidbody rigidBody))
            {
                ApplyForce(rigidBody);
                //AudioSource.PlayClipAtPoint(audioClip, transform.position);
                reload = true;
                //ballTrackerHelper.newShotTaken = true;
                //trialCount.AddTrial(1);
                ////shots.AddShots(1);
            }
            lastShotTime = Time.time;
            }
        }
        private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target)
        {
            ScoreCount.instance.AddPoint();
            //Destroy(collision.gameObject); // Destroy the target when hit (optional)
            Destroy(gameObject); // Destroy the projectile when it hits the target (optional)
        }
    }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Reload"))
            {
                reload = false;
            }
        }

        void ApplyForce(Rigidbody rigidBody)
        {
            Vector3 force = m_StartPoint.forward * m_LaunchSpeed;
            rigidBody.AddForce(force);
        }
    }
