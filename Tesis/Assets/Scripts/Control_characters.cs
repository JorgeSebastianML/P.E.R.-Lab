using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_characters : MonoBehaviour
{
    public int probabilityActivation = 20;
    private List<GameObject> characteres = new List<GameObject>();
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
