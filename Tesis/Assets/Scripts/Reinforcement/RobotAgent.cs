using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RobotAgent : Agent
{
    // Variables
    Rigidbody rBody;
    // Tipo de entrenamiento: 
    public bool learningOptions = false;
    public float timeOptions = 2;  
    private List<bool> timeFlag;
    private float controlTime; 
    private bool isInAction = false; 
    private float rotationDir = 0;
    // Parametros del robot
    public float LinearVelocity = 1;
    public float AngularVelocity = 1;
    public float endDistance = 0.3f; 
    public float SensorDistance = 1; 
    public float CameraDistance = 2;
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
    private List<string> prohibiteObjtects; 
    private List<string> detectedObjects;
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
    private float WaithTime = 120; 
    // Informacion del episodio
    private int nEpisode = 0; 

    // Inicializacion del ambiente 
    void Start () {
        // Inicializar la informacion de la pocision, fisicas y sensores del robot
        controlTime = timeOptions; 
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
        // Se inicializarn las banderas 
        timeFlag = new List<bool>(); 
        for(int i = 0; i < 4; i++)
        {
            timeFlag.Add(false); 
        }
        isInAction = false;
        rotationDir = 0;
    }

    // Inicio del episodio 
    public override void OnEpisodeBegin()
    {
        int nPersonas = 0; 
        int nObjects = 0; 
        questionPersons = new List<string>();
        prohibiteObjtects = new List<string>();
        detectedObjects = new List<string>(); 
        rotationDir = 0;
        // Inicializar condiciones de recompenzas e informacion de sensores
        maxTime = 3600;
        WaithTime = 120;
        NegativeRewardTime = 10;
        RewardTime = 10;
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
        nEpisode = nEpisode + 1; 
        controlTime = timeOptions; 
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
                nObjects = nObjects + 1;
                detectedObjects.Add(temp.name);  
            }
        }
        print("Numero de objectos prohibidos en el Episodio " + nEpisode.ToString() + " son: " + nObjects.ToString()); 
        // Inicializar aleatoriamente la pocision del robot 
        Vector3 startPosition =  new Vector3(Random.Range(RangoPosicion[0], RangoPosicion[1]), 0.893f, Random.Range(RangoPosicion[2], RangoPosicion[3]));
        //print("Posicion de inicio en el episodio " + nEpisode.ToString() + " es: " + startPosition.ToString()); 
        transform.localPosition = startPosition; 
        transform.Rotate(0, Random.Range(0, 360), 0); 
        LastPosition = transform.localPosition;
        //print("Posicion registrada de inicio en el episodio " + nEpisode.ToString() + " es: " + LastPosition.ToString()); 
        // Inicializar aleatoriamente la pocision del tablero 
        board.transform.localPosition = new Vector3(Random.Range(RangoPosicion[0], RangoPosicion[1]), 0.1f, Random.Range(RangoPosicion[2], RangoPosicion[3]));
        board.transform.Rotate(0, Random.Range(0, 360), 0); 

        for(int i = 0; i < 4; i++)
        {
            timeFlag[i] = false; 
        }
        isInAction = false;
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
        // Acciones
        if(learningOptions == true) // Acciones de Learning Option
        {
            // Verificacion de que solo una accion se ejecute a la vez
            if((vectorAction[1] > 0.5) && (isInAction == false))
            {
                timeFlag[0] = true;
            }
            else if((vectorAction[1] < -0.5) && (isInAction == false))
            {
                timeFlag[1] = true;
            }
            else if((vectorAction[0] > 0.5) && (isInAction == false))
            {
                timeFlag[2] = true;
            }
            else if((vectorAction[0] < -0.5) && (isInAction == false))
            {
                timeFlag[3] = true;
            }
            else
            {
                if(vectorAction[2] >= 0.5)
                {
                    rBody.maxAngularVelocity = vectorAction[2]*AngularVelocity;
                    rBody.AddTorque(new Vector3(0f, 1f, 0f), ForceMode.Impulse);
                    rotationDir = -1f;
                } 
                else if(vectorAction[2] <= -0.5)
                {
                    rBody.maxAngularVelocity = vectorAction[2]*AngularVelocity*1;
                    rBody.AddTorque(new Vector3(0f, -1f, 0f), ForceMode.Impulse);
                    rotationDir = 1f;
                }
                else
                {
                    rBody.AddTorque(new Vector3(0f, rotationDir, 0f), ForceMode.Impulse);
                    rBody.AddTorque(new Vector3(0f, 0f, 0f), ForceMode.Impulse);
                    rBody.maxAngularVelocity = 0;
                }
            }
            // Mantener la direccion x tiempo
            if(timeFlag[0] == true)
            {
                isInAction = true;
                Vector3 controlSignal = Vector3.zero;
                rBody.AddTorque(new Vector3(0f, rotationDir, 0f), ForceMode.Impulse);
                rBody.maxAngularVelocity = 0;
                controlSignal.z = 1f;
                rBody.AddRelativeForce(controlSignal * LinearVelocity);
                controlTime -= Time.deltaTime;
                if(controlTime < 0)
                {
                    timeFlag[0] = false;
                    controlTime = timeOptions; 
                    isInAction = false; 
                    rBody.velocity = Vector3.zero;
                }
            }
            else if(timeFlag[1] == true)
            {
                isInAction = true;
                Vector3 controlSignal = Vector3.zero;
                rBody.AddTorque(new Vector3(0f, rotationDir, 0f), ForceMode.Impulse);
                rBody.maxAngularVelocity = 0;
                controlSignal.z = 0f;
                rBody.AddRelativeForce(controlSignal * LinearVelocity);
                controlTime -= Time.deltaTime;
                if(controlTime < 0)
                {
                    timeFlag[1] = false;
                    controlTime = timeOptions; 
                    isInAction = false; 
                    rBody.velocity = Vector3.zero;
                }
            }
            else if(timeFlag[2] == true)
            {
                isInAction = true;
                Vector3 controlSignal = Vector3.zero;
                rBody.AddTorque(new Vector3(0f, rotationDir, 0f), ForceMode.Impulse);
                rBody.maxAngularVelocity = 0;
                controlSignal.x = 1f;
                rBody.AddRelativeForce(controlSignal * LinearVelocity);
                controlTime -= Time.deltaTime;
                if(controlTime < 0)
                {
                    timeFlag[2] = false;
                    controlTime = timeOptions; 
                    isInAction = false; 
                    rBody.velocity = Vector3.zero;
                }
            }
            else if(timeFlag[3] == true)
            {
                isInAction = true;
                Vector3 controlSignal = Vector3.zero;
                rBody.AddTorque(new Vector3(0f, rotationDir, 0f), ForceMode.Impulse);
                rBody.maxAngularVelocity = 0;
                controlSignal.x = -1f;
                rBody.AddRelativeForce(controlSignal * LinearVelocity);
                controlTime -= Time.deltaTime;
                if(controlTime < 0)
                {
                    timeFlag[3] = false;
                    controlTime = timeOptions; 
                    isInAction = false; 
                    rBody.velocity = Vector3.zero;
                }
            }           
        }
        else // Acciones continuas
        {
            Vector3 controlSignal = Vector3.zero;
            controlSignal.x = vectorAction[0];
            controlSignal.z = vectorAction[1];
            if(controlSignal.z < 0)
            {
                controlSignal.z = 0;
            } 
            rBody.AddRelativeForce(controlSignal * LinearVelocity);
            rBody.velocity = Vector3.ClampMagnitude(rBody.velocity, 5);
            
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
        }
        // Accion: Rotacion sobre el mismo eje
        if(vectorAction[3] > 0)
        {
            Quaternion target = Quaternion.Euler(vectorAction[3]*30, 0, 0);
            child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, target,  Time.deltaTime * 5f);     
        }
        else if(vectorAction[3] < 0)
        {
            Quaternion target = Quaternion.Euler(360 + vectorAction[3]*30, 0, 0);
            child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, target,  Time.deltaTime * 5f);
        }
        else
        {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, target,  Time.deltaTime * 5f);
        }

        bool Object_Predict = false; 
        
        RotationCamera = child.transform.localRotation.eulerAngles / 180.0f - Vector3.one;

        // Lectura de sensores laseres
        for(int i = 0; i < FrontSensors.Count; i++) //Frontales
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
                        ListOfObjects(); 
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
        for(int i = 0; i < RigthSensors.Count; i++) // Los de la derecha
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
                        ListOfObjects();
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
        for(int i = 0; i < LeftSensors.Count; i++) // Los de la izquierda
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
                        ListOfObjects(); 
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
        //Lectura de camara de profundidad
        for(int i = 0; i < CameraView.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(CameraView[i].transform.position, CameraView[i].transform.up * -1f, out hit, CameraDistance))
            {
                if(hit.collider.gameObject.tag == "Ninguna") // Se determina si la deteccion es de alguien que no quiere que se acerque
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
                else if(hit.collider.gameObject.tag == "Pregunta") // Se determina si la deteccion es de alguien que pregunta
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
                        SetReward(0.5f);
                        questionPersons.Add(hit.collider.gameObject.name);
                        print("En el Episodio " + nEpisode.ToString() + " se ha encontrado " + questionPersons.Count + " personas preguntando"); 
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
                    if (!prohibiteObjtects.Contains(hit.collider.gameObject.name))
                    {
                        prohibiteObjtects.Add(hit.collider.gameObject.name);
                        SetReward(0.5f);
                        print("En el Episodio " + nEpisode.ToString() + " se ha encontrado " + prohibiteObjtects.Count + " objectos prohibidos"); 
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
        // Verificacion señal de advertencia
        if(vectorAction[4] >= 0 && Object_Predict == false)
        {
            SetReward(-0.2f);
            Debug.Log("Me equivoque en recoja el objecto prohibido");
        }
        // Calculo del teimpo de ejecucion
        maxTime -= Time.deltaTime;
        RewardTime -= Time.deltaTime;
        if(maxTime < 0) // Condicion de terminacion del episodeo, tiempo maximo 
        {
            ListOfObjects(); 
            EndEpisode();
        }
        // Condicion de recompenza cuando no se choca en x tiempo
        if(RewardTime < 0)
        {
            SetReward(0.01f);
            RewardTime = 10;
            Debug.Log("Recompenza tiempo");
        }
        // Calculo de distancia recorrida y tiempo trancurrido
        WaithTime -= Time.deltaTime;
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, LastPosition);
        // Condicion de recompenza negativa de mquedarse quieto mucho tiempo
        if(WaithTime < 0)
        {
            if(distanceToTarget < 0.5)
            {
                Debug.Log("Se quedo quieto mucho tiempo");
                SetReward(-1f);
                ListOfObjects(); 
                EndEpisode();
            }
            else
            {
                SetReward((float)(0.02*distanceToTarget));
                LastPosition = this.transform.localPosition;
                WaithTime = 120;
            }
        }
        // Condicion de caerse de la plataforma
        if (this.transform.localPosition.y < 0)
        {
            ListOfObjects(); 
            EndEpisode();
        }
    }

    // Controles manuales para mover el robot por medio de periferico
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetAxis("Mouse X");
        actionsOut[3] = Input.GetAxis("Mouse Y");
        actionsOut[4] = Input.GetAxis("Fire1");
    }

    // Funcion que permite idenificar cuantos objetos no son encontrados durante un episodeo 
    private void ListOfObjects()
    {
        List<string> list = new List<string>(); 
        for(int i = 0; i < detectedObjects.Count; i++)
        {
            if (!prohibiteObjtects.Contains(detectedObjects[i]))
            {
                list.Add(detectedObjects[i]); 
            }
        }
        
        print("Objetos no reconocidos en el episodio " + nEpisode.ToString() + " son: " + string.Join(" ", list)); 
    }

}