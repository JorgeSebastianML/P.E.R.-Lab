using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMove : MonoBehaviour
{
    public float velocidad;
    public float rotacion;
    private GameObject child;
    public GameObject Sensors; 
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(Sensors.transform.position, Sensors.transform.up * -0.5f, Color.red);

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0f, 0f, velocidad * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0f, 0f, -1*velocidad * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(velocidad * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-1*velocidad * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(-1*Vector3.up * rotacion * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up * rotacion * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.R))
        {
            child.transform.Rotate(Vector3.left * rotacion * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.F))
        {
            child.transform.Rotate(-1*Vector3.left * rotacion * Time.deltaTime);
        }
    }
}
