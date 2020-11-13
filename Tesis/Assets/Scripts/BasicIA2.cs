using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class BasicIA2 : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for 
        private float timeToChangeDirection;
        public float Speed = 0.5f; 
        private Vector3 newUp; 
        public List<float> RangoPosicion;

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updateRotation = false;
            agent.updatePosition = true;
        }


        private void Update()
        {
            timeToChangeDirection -= Time.deltaTime;
            if (target != null)
                agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
            {
                if (timeToChangeDirection <= 0)
                {
                    ChangeDirection();
                    //UnityEngine.Debug.Log(newUp);
                }
                agent.speed = Speed;
                //character.Move(newUp, false, false);
                agent.SetDestination(newUp);
            }
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        private void ChangeDirection()
        {
            //float x = UnityEngine.Random.Range(-0.5f, 0.5f);
            //float z = UnityEngine.Random.Range(-0.5f, 0.5f);
            //newUp = new Vector3(x, 0, z);
            float x = UnityEngine.Random.Range(RangoPosicion[0], RangoPosicion[1]);
            float z = UnityEngine.Random.Range(RangoPosicion[2], RangoPosicion[3]);
            newUp = new Vector3(x, 0, z);
            //newUp.y = 0;
            //newUp.Normalize();
            //UnityEngine.Debug.Log(newUp);
            timeToChangeDirection = 1.5f;
        }


        void OnCollisionEnter(Collision coll)
        {
            float des = UnityEngine.Random.Range(0f, 1f);
            Vector3 pos = coll.transform.position;
            //Debug.Log(newUp.ToString()); 
            if (des < 0.7)
            {
                agent.SetDestination(-1 * newUp);
                new WaitForSeconds(3f); // Wait for new path
                //Debug.Log("Stay Away");
            }
            else
            {
                agent.SetDestination(newUp);
                new WaitForSeconds(3f); // Wait for new path
                //Debug.Log("GO!");
            }
        }
    }

}

