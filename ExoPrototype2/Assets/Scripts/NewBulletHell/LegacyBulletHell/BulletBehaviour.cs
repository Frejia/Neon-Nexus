using System.Collections;
using UnityEngine;

namespace BulletHell
{
    
    public class BulletBehaviour : MonoBehaviour
    {
        public float amplitude; // The amplitude of the sinus curve.
        public float frequency; // The frequency of the sinus curve.
        private Transform player;
        [SerializeField] private Vector3 direction;
        
        public enum BulletBehaviours
        {
            None,
            SineCurve,
            Circle,
            Bounce,
            Follow
        }
        
        public BulletBehaviours activebulletBehaviour;

        private void BehaviourSwitch()
        {
            switch (activebulletBehaviour)
            {
                case BulletBehaviours.None:
                    break;
                case BulletBehaviours.SineCurve:
                    MoveInSineCurve();
                    break;
                case BulletBehaviours.Follow:
                    MoveTowardsPlayer();
                    break;
                case BulletBehaviours.Circle:
                    
                    break;
            }
        }

        public void SetBehaviour(BulletBehaviours bulletBehaviour, Vector3 direction)
        {
            activebulletBehaviour = bulletBehaviour;
            this.direction = direction;
        }

        public void Awake()
        {
          player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void Start()
        {
            BehaviourSwitch();
        }

        void Update()
        {
            BehaviourSwitch();
        }

        void MoveTowardsPlayer()
        {
            if (player != null)
            {
                // Move towards the player's position
                transform.position = Vector3.MoveTowards(transform.position, player.position, this.GetComponent<Bullet>().speed * Time.deltaTime);
            }
        }

        public float teiler = 50f;

        public void MoveInSineCurve()
        {
            // Move the bullet in a sine curve pattern
            Vector3 newPos = transform.position;
            newPos += (Vector3)(direction.normalized * Time.deltaTime / teiler);
            newPos.y += amplitude * Mathf.Sin(frequency * Time.time);
            transform.position = newPos;
        }
        
        
    }
}