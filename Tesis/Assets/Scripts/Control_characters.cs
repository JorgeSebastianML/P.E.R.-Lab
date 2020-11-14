using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_characters : MonoBehaviour
{
    public int probabilityActivation = 20;
    private List<GameObject> characteres = new List<GameObject>();
    private int nPersonas = 0; 
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this.transform.childCount); 
        for(int i = 0; i < this.transform.childCount; i++)
        {
            if(Random.Range(0, 100) < 20)
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
        print("Numero de personas Preguntando son: " + nPersonas.ToString()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
