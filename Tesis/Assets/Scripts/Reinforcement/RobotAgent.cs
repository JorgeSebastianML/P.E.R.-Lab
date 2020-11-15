using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RobotAgent : Agent
{
    // Variables
    Rigidbody rBody;
    // Parametros del robot
    public float LinearVelocity = 1;
    public float AngularVelocity = 1;
    public float endDistance = 0.3f; 
    public float SensorDistance = 1; 
    public float CameraDistance = 2;
    public float DetectionDistance = 1f;
    public float DetectionDistanceObject = 1.5f;
    public int Probabilty = 20;
    public float maxTime = 3600;
    // Sensores 
    private GameObject child; 
    public List<GameObject> FrontSensors;
    public List<GameObject> RigthSensors;
    public List<GameObject> LeftSensors;
    public List<GameObject> CameraView; 
    // Variables del entorno
    public GameObject Characteres; 
    public GameObject board; 
    public GameObject Objects;
    // Informacion del entorno
    public List<float> RangoPosicion;
    private List<string> questionPersons; 
    // Informacion de los sensores
    public List<float> CameraDistanceList;
    private List<float> Detections;
    private List<int> CameraDetections_Stop;
    private List<int> CameraDetections_Question;
    private List<int> CameraDetections_None;
    private List<int> CameraDetections_Object;
    // Informacion de la Camara
    private Vector3 RotationCamera; 
    // Condiciones para recompenzas 
    private float RewardTime = 10;
    private float NegativeRewardTime = 10; 
    private Vector3 LastPosition; 
    private float WaithTime = 240; 
    // Informacion del episodio
    private int nEpisode = 0; 

    // Inicializacion del ambiente 
    void Start () {
        // Inicializar la informacion de la pocision, fisicas y sensores del robot
        LastPosition = this.transform.localPosition; 
        rBody = GetComponent<Rigidbody>();
        child = transform.GetChild(0).gameObject;
        Detections = new List<float>();
        CameraDistanceList = new List<float>();
        CameraDetections_Stop = new List<int>(); 
        CameraDetections_Question = new List<int>();
        CameraDetections_None = new List<int>();
        CameraDetections_Object = new List<int>();
        for(int i = 0; i < (FrontSensors.Count + RigthSensors.Count + LeftSensors.Count); i++)
        {
            Detections.Add(SensorDistance);
        }
        for(int i = 0; i < CameraView.Count; i++)
        {
            CameraDetections_Stop.Add(0); 
            CameraDetections_Question.Add(0);
            CameraDetections_None.Add(0);
            CameraDetections_Object.Add(0);
            CameraDistanceList.Add(CameraDistance); 
        }
        RotationCamera = child.transform.localEulerAngles;
    }

    // Inicio del episodio 
    public override void OnEpisodeBegin()
    {
        int nPersonas = 0; 
        questionPersons = new List<string>();
        // Inicializar condiciones de recompenzas e informacion de sensores
        maxTime = 3600;
        WaithTime = 240;
        NegativeRewardTime = 10;
        RewardTime = 10;
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
        nEpisode = nEpisode + 1; 
        // Activar de forma aleatoria las personas del ambiente 
        for(int i = 0; i < Characteres.transform.childCount; i++)
        {
            GameObject temp = Characteres.transform.GetChild(i).gameObject;
            temp.SetActive(false);
            if(Random.Range(0, 100) < Probabilty)
            {  
                temp.SetActive(true); 
                if (temp.GetComponent<AnimationController>() != null)
                {
                    AnimationController Animation = temp.GetComponent<AnimationController>();
                    if (Animation.action == 0)
                    {
                        nPersonas = nPersonas + 1; 
                    }
                }
            }
        }
        print("Numero de personas Preguntando en el Episodio " + nEpisode.ToString() + " son: " + nPersonas.ToString()); 
        // Activar de forma aleatoria los objectos del ambiente 
        for(int i = 0; i < Objects.transform.childCount; i++)
        {
            GameObject temp = Objects.transform.GetChild(i).gameObject;
            temp.SetActive(false);
            if(Random.Range(0, 100) < Probabilty)
            {  
                temp.SetActive(true); 
            }
        }
        // Inicializar aleatoriamente la pocision del robot 
        Vector3 startPosition =  new Vector3(Random.Range(RangoPosicion[0], RangoPosicion[1]), 0.893f, Random.Range(RangoPosicion[2], RangoPosicion[3]));
        this.transform.position = startPosition; 
        this.transform.Rotate(0, Random.Range(0, 360), 0); 
        LastPosition = this.transform.localPosition;
        // Inicializar aleatoriamente la pocision del tablero 
        board.transform.position = new Vector3(Random.Range(RangoPosicion[0], RangoPosicion[1]), 0.1f, Random.Range(RangoPosicion[2], RangoPosicion[3]));
        board.transform.Rotate(0, Random.Range(0, 360), 0); 
    }

    // Recoleccion de Observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        // Rotacion de la camara del robot
        sensor.AddObservation(RotationCamera);
        // Velocidad del robot
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        sensor.AddObservation(rBody.angularVelocity);
        // Informacion de los sensores laseres normalizada
        for(int i = 0; i < Detections.Count; i++)
        {
            sensor.AddObservation(Detections[i] / SensorDistance);
        }
        // Informacion de los sensores de la camara normalizada
        for(int i = 0; i < CameraDetections_Stop.Count; i++)
        {
            sensor.AddObservation(CameraDetections_Stop[i]);
        }

        for(int i = 0; i < CameraDetections_Question.Count; i++)
        {
            sensor.AddObservation(CameraDetections_Question[i]);
        }

        for(int i = 0; i < CameraDetections_None.Count; i++)
        {
            sensor.AddObservation(CameraDetections_None[i]);
        }

        for(int i = 0; i < CameraDetections_Object.Count; i++)
        {
            sensor.AddObservation(CameraDetections_Object[i]);
        }

        for(int i = 0; i < CameraDistanceList.Count; i++)
        {
            sensor.AddObservation(CameraDistanceList[i]/CameraDistance);
        }
    }

    //
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        if(controlSignal.z < 0)
        {
            controlSignal.z = 0;
        }
        //Vector3 controlSignalRotation = Vector3.zero;
        //controlSignalRotation.y = vectorAction[2]; 
        
        //this.transform.Translate(controlSignal * Time.deltaTime * LinearVelocity);
        //Debug.Log(controlSignal * LinearVelocity); 
        rBody.AddRelativeForce(controlSignal * LinearVelocity);
        rBody.velocity = Vector3.ClampMagnitude(rBody.velocity, 5);
        //rBody.AddTorque(controlSignalRotation*AngularVelocity);
        //var torqueY = Mathf.Clamp(vectorAction[2], -1f, 1f) * AngularVelocity; 
        
        if(vectorAction[2] >= 0)
        {
            rBody.maxAngularVelocity = vectorAction[2]*AngularVelocity;
            rBody.AddTorque(new Vector3(0f, 1, 0f), ForceMode.Impulse);
        } 
        else
        {
            rBody.maxAngularVelocity = vectorAction[2]*AngularVelocity*1;
            rBody.AddTorque(new Vector3(0f, -1f, 0f), ForceMode.Impulse);
        }
        
        //this.transform.Rotate(controlSignalRotation * Time.deltaTime * AngularVelocity);
        if(vectorAction[3] > 0)
        {
            Quaternion target = Quaternion.Euler(vectorAction[3]*30, 0, 0);
            child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, target,  Time.deltaTime * 5f);
            //Debug.Log(vectorAction[3]*30);
            
        }
        else if(vectorAction[3] < 0)
        {
            //Debug.Log(360 + vectorAction[3]*30);
            Quaternion target = Quaternion.Euler(360 + vectorAction[3]*30, 0, 0);
            child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, target,  Time.deltaTime * 5f);
        }
        else
        {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, target,  Time.deltaTime * 5f);
        }

        //float HeadRotation = vectorAction[3] * AngularVelocity * Time.deltaTime; 
        bool Object_Predict = false; 

        //child.transform.Rotate(HeadRotation, 0, 0); 
        
        RotationCamera = child.transform.localRotation.eulerAngles / 180.0f - Vector3.one;

        // Laser Sensors
        for(int i = 0; i < FrontSensors.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(FrontSensors[i].transform.position, FrontSensors[i].transform.up * -1f, out hit, SensorDistance))
            {
                if(hit.collider.gameObject.tag == "Choque")
                {
                    Debug.DrawRay(FrontSensors[i].transform.position, FrontSensors[i].transform.up * -1f * hit.distance, Color.green);
                    Detections[i] = hit.distance;
                    if(hit.distance < endDistance)
                    {
                        Debug.Log("choco");
                        SetReward(-1.0f);
                        EndEpisode();
                    }
                }
                else
                {
                    Debug.DrawRay(FrontSensors[i].transform.position, FrontSensors[i].transform.up * -1f * SensorDistance, Color.red);
                    Detections[i] = hit.distance;
                }
                
            }
            else
            {
                Debug.DrawRay(FrontSensors[i].transform.position, FrontSensors[i].transform.up * -1f * SensorDistance, Color.red);
                Detections[i] = SensorDistance;
            }
        }
        for(int i = 0; i < RigthSensors.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(RigthSensors[i].transform.position, RigthSensors[i].transform.up * -1f, out hit, SensorDistance))
            {
                if(hit.collider.gameObject.tag == "Choque")
                {
                    Debug.DrawRay(RigthSensors[i].transform.position, RigthSensors[i].transform.up * -1f * hit.distance, Color.green);
                    Detections[i + FrontSensors.Count] = hit.distance;
                    if(hit.distance < endDistance)
                    {
                        Debug.Log("choco");
                        SetReward(-1.0f);
                        EndEpisode();
                    }
                }
                else
                {
                    Debug.DrawRay(RigthSensors[i].transform.position, RigthSensors[i].transform.up * -1f * SensorDistance, Color.red);
                    Detections[i + FrontSensors.Count] = hit.distance;
                }

            }
            else
            {
                Debug.DrawRay(RigthSensors[i].transform.position, RigthSensors[i].transform.up * -1f * SensorDistance, Color.red);
                Detections[i + FrontSensors.Count] = SensorDistance;
            }
        }
        for(int i = 0; i < LeftSensors.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(LeftSensors[i].transform.position, LeftSensors[i].transform.up * -1f, out hit, SensorDistance))
            {
                if(hit.collider.gameObject.tag == "Choque")
                {
                    Debug.DrawRay(LeftSensors[i].transform.position, LeftSensors[i].transform.up * -1f * hit.distance, Color.green);
                    Detections[i + FrontSensors.Count + LeftSensors.Count] = hit.distance;
                    if(hit.distance < endDistance)
                    {
                        Debug.Log("choco");
                        SetReward(-1.0f);
                        EndEpisode();
                    }
                }
                else
                {
                    Debug.DrawRay(LeftSensors[i].transform.position, LeftSensors[i].transform.up * -1f * SensorDistance, Color.red);
                    Detections[i + FrontSensors.Count + LeftSensors.Count] = hit.distance;
                }

            }
            else
            {
                Debug.DrawRay(LeftSensors[i].transform.position, LeftSensors[i].transform.up * -1f * SensorDistance, Color.red);
                Detections[i + FrontSensors.Count + LeftSensors.Count] = SensorDistance;
            }

        }
        //Camera view
        for(int i = 0; i < CameraView.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(CameraView[i].transform.position, CameraView[i].transform.up * -1f, out hit, CameraDistance))
            {
                if(hit.collider.gameObject.tag == "Ninguna")
                {
                    Debug.DrawRay(CameraView[i].transform.position, CameraView[i].transform.up * -1f * hit.distance, Color.green);
                    //Debug.Log("Encontro a alguien");
                    CameraDetections_None[i] = 1; 
                    CameraDetections_Object[i] = 0;
                    CameraDetections_Stop[i] = 0; 
                    CameraDetections_Question[i] = 0;
                    CameraDistanceList[i] =  hit.distance;
                    NegativeRewardTime -= Time.deltaTime; 
                    if(NegativeRewardTime <= 0)
                    {
                        SetReward(-0.005f);
                    } 
                }
                else if(hit.collider.gameObject.tag == "Pregunta")
                {
                    Debug.DrawRay(CameraView[i].transform.position, CameraView[i].transform.up * -1f * hit.distance, Color.green);
                    CameraDetections_None[i] = 0; 
                    CameraDetections_Object[i] = 0;
                    CameraDetections_Stop[i] = 0; 
                    CameraDetections_Question[i] = 1; 
                    CameraDistanceList[i] =  hit.distance; 
                    SetReward((1 - (hit.distance/CameraDistance)));
                    Debug.Log("Que pregunta tienes?");
                    CameraDistanceList[i] =  hit.distance; 
                    NegativeRewardTime = 10; 
                    if (!questionPersons.Contains(hit.collider.gameObject.name))
                    {
                        questionPersons.Add(hit.collider.gameObject.name);
                        print("En el Episodio " + nEpisode.ToString() + " se ha encontrado " + questionPersons.Count + "personas preguntando"); 
                    }
                    //Debug.Log(hit.distance);
                }
                else if(hit.collider.gameObject.tag == "Alto")
                {
                    Debug.DrawRay(CameraView[i].transform.position, CameraView[i].transform.up * -1f * hit.distance, Color.green);
                    CameraDetections_None[i] = 0; 
                    CameraDetections_Object[i] = 0;
                    CameraDetections_Stop[i] = 1; 
                    CameraDetections_Question[i] = 0; 
                    SetReward(-0.9f*(1-(hit.distance/CameraDistance)));
                    Debug.Log("Alguien No preguntando");
                    CameraDistanceList[i] =  hit.distance; 
                    if(NegativeRewardTime <= 0)
                    {
                        SetReward(-0.005f);
                    } 
                }
                else if(hit.collider.gameObject.tag == "Objecto_P")
                {
                    Debug.DrawRay(CameraView[i].transform.position, CameraView[i].transform.up * -1f * hit.distance, Color.green);
                    CameraDetections_None[i] = 0; 
                    CameraDetections_Object[i] = 1;
                    CameraDetections_Stop[i] = 0; 
                    CameraDetections_Question[i] = 0; 
                    CameraDistanceList[i] =  hit.distance; 
                    NegativeRewardTime = 10;
                    if(vectorAction[4] >= 0)
                    {
                        Debug.Log("Recoja el objecto prohibido");
                        Object_Predict = true;
                        SetReward(1f);
                    }
                }
                else
                {
                    Debug.DrawRay(CameraView[i].transform.position, CameraView[i].transform.up * -1f * hit.distance, Color.green);
                    CameraDetections_None[i] = 0; 
                    CameraDetections_Object[i] = 0;
                    CameraDetections_Stop[i] = 0; 
                    CameraDetections_Question[i] = 0;
                    CameraDistanceList[i] =  CameraDistance;
                    NegativeRewardTime -= Time.deltaTime; 
                    if(NegativeRewardTime <= 0)
                    {
                        SetReward(-0.005f);
                    }
                }
                
            }
            else
            {
                Debug.DrawRay(CameraView[i].transform.position, CameraView[i].transform.up * -1f * CameraDistance, Color.blue);
            }
            
        }
        
        if(vectorAction[4] >= 0 && Object_Predict == false)
        {
            SetReward(-0.2f);
            Debug.Log("Me equivoque en recoja el objecto prohibido");
        }

        maxTime -= Time.deltaTime;
        RewardTime -= Time.deltaTime;
        if(maxTime < 0)
        {
            EndEpisode();
        }
        if(RewardTime < 0)
        {
            SetReward(0.01f);
            RewardTime = 10;
            Debug.Log("Recompenza tiempo");
        }
        WaithTime -= Time.deltaTime;
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, LastPosition);
        if(WaithTime < 0)
        {
            if(distanceToTarget < 0.30)
            {
                Debug.Log("Se quedo quieto mucho tiempo");
                SetReward(-1f);
                EndEpisode();
            }
            else
            {
                SetReward((float)(0.01*distanceToTarget));
                LastPosition = this.transform.localPosition;
                WaithTime = 240;
            }
        }
        

        
        // Rewards

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            //Debug.Log("condicion 2");
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetAxis("Mouse X");
        actionsOut[3] = Input.GetAxis("Mouse Y");
        actionsOut[4] = Input.GetAxis("Fire1");
    }

}