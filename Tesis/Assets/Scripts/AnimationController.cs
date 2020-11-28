using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public int action = 0; 
    public int probabilidad = 40;
    public string animationName = "Asking Question"; 
    public float animationTime = 60; 
    private float MaxTime;
    // Start is called before the first frame update
    void Start()
    {
        MaxTime = animationTime + Random.Range(-30, 30); 
    }

    // Update is called once per frame
    void Update()
    {
        MaxTime -= Time.deltaTime;
        if (MaxTime <= 0)
        {
            MaxTime = animationTime + Random.Range(-30, 30);
            
            if (Random.Range(0, 100) <= probabilidad)
            {
                this.gameObject.GetComponent<Animator>().Play(animationName);
            }
            
        }

        if (this.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            if(action == 0)
            {
                this.gameObject.transform.tag = "Pregunta";
            }
            else
            {
                this.gameObject.transform.tag = "Alto";
            }
            
            return; 
        }
        else
        {
            this.gameObject.transform.tag = "Ninguna";
        }
        
    }
}
