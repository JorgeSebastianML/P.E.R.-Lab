using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public int action = 0; 
    public int probabilidad = 10;
    public string animationName = "Asking Question"; 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        else if (Random.Range(0, 1000) < probabilidad)
        {
            this.gameObject.GetComponent<Animator>().Play(animationName);
        }
        else
        {
            this.gameObject.transform.tag = "Ninguna";
        }
    }
}
