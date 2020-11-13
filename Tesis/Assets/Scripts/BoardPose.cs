using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(Random.Range(-4, 4), -0.526f, Random.Range(-1f, 17f));
        this.transform.Rotate(0, Random.Range(0, 360), 0); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
