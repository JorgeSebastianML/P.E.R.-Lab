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
        public UnityEngine.AI.NavMeshAgent agent { get; private set; } // Variable del navmesh para el calculo de una ruta
        public ThirdPersonCharacter character { get; private set; } // Para el control del personaje
        public Transform target; // Destino
        private float timeToChangeDirection; // Variable de tiempo para el cambio de destino
        public float Speed = 0.5f; // Velocidad de caminata
        private Vector3 newUp; 
        public List<float> RangoPosicion; // Area en el que puede movilizarse 

        // Funcion que se ejecuta en el primer frame
        private void Start()
        {
            // Se cargan los componentes de navegacion y de tercera persona del personaje 
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            // Se ajustan los parametros de actualizacion de movimiento
            agent.updateRotation = false;
            agent.updatePosition = true;
        }

        // Funcion que se llama cada frame
        private void Update()
        {
            // Se calcula el tiempo transcurrido
            timeToChangeDirection -= Time.deltaTime;
            // Se verifica que exista un destino, para poder configurarlo en el sistema de navegacion 
            if (target != null)
                agent.SetDestination(target.position);
            // Se verifica si el personaje aun no ha llegado a su destino, para reconfirmar su destino
            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
            {
                // Se verifica si se acabo el tiempo de cambio de direccion para cambiar la direccion  
                if (timeToChangeDirection <= 0)
                {
                    ChangeDirection();
                }
                // Se le ajusta la velocidad al personaje
                agent.speed = Speed;
                // Se confirma el destino previmente seleccionado 
                agent.SetDestination(newUp);
            }
        }

        // Funcion para seleccionar un destino
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
            timeToChangeDirection = 2f;
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

