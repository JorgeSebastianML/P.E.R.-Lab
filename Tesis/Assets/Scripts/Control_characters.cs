using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_characters : MonoBehaviour
{
    // Variable modificable que determina la probabilidad de activacion de personajes 0-100
    public int probabilityActivation = 20;
    // Variables de control interno
    private List<GameObject> characteres = new List<GameObject>();
    private int nPersonas = 0; 
    // Funcion que se llama en el primer frame
    void Start()
    {
        // Se recorre todos los gameobject hijo y se activa con una probabilidad x 
        for(int i = 0; i < this.transform.childCount; i++)
        {
            if(Random.Range(0, 100) < probabilityActivation)
            {
                GameObject child = this.transform.GetChild(i).gameObject;
                child.SetActive(true); 
                if (child.GetComponent<AnimationController>() != null)
                {
                    AnimationController Animation = child.GetComponent<AnimationController>();
                    if (Animation.action == 0)
                    {
                        nPersonas = nPersonas + 1; 
                    }
                }
                
            }
        }
        // Se imprime el numero total de personajes activados
        print("Numero de personas Preguntando son: " + nPersonas.ToString()); 
    }
}
