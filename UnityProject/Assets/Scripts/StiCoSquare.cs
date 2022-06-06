using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU;
using ABMU.Core;

public class StiCoSquare : AbstractAgent
{
    [SerializeField]
    private int id;
    [SerializeField]
    private float v0;
    [SerializeField]
    private float curDist;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float maxDist;
    [SerializeField]
    private float minDist;
    [SerializeField]
    private float direction;
    [SerializeField]
    private float timeLimit;

    public GameObject markerPrefab;

    [SerializeField]
    private bool _aStiCo;
    [SerializeField]
    private bool _idStiCo;
    private float noCollisionTimer;
    private float timeIncrease;

    private Vector3 lastCorner;

    // Robot values init
    public void Init(int i, float v, float d, float minD, float maxD, float tl, float ti, bool astico, bool idstico)
    {
        base.Init();

        id = i;
        v0 = v;
        distance = d;
        curDist = distance;
        maxDist = maxD;
        minDist = minD;
        direction = (Random.Range(0,2)*2-1);
        timeLimit = tl;

        _aStiCo = astico;
        _idStiCo = idstico;
        timeIncrease = ti;
        noCollisionTimer = 0.0f;

        lastCorner = gameObject.transform.position;

        CreateStepper(DropMarker, 1, Stepper.StepperQueueOrder.NORMAL);
        CreateStepper(Move, 1, Stepper.StepperQueueOrder.NORMAL);
    }

    // Drops a marker at the robot position
    void DropMarker()
    {
        GameObject m = Instantiate(markerPrefab, gameObject.transform.position, gameObject.transform.rotation);
        m.GetComponent<Marker>().Init(id, timeLimit);
    }

    // When there is a collision
    void OnTriggerEnter2D(Collider2D col)
    {
        Vector3 relativePoint = Vector3.zero;
        switch(col.gameObject.tag)
        {
            case "Marker" :
                if(col.gameObject.GetComponent<Marker>().GetId()==id)
                    return;
                else
                    relativePoint = getRelativePosition(gameObject.transform, col.transform.position);
                break;
            case "Robot" :
                if(col.gameObject.GetComponent<StiCo>().GetId()==id)
                    return;
                else
                    relativePoint = getRelativePosition(gameObject.transform, col.transform.position);
                break;
            case "Obstacle" :
                Vector2 colPoint = col.ClosestPoint(transform.position);
                relativePoint = getRelativePosition(gameObject.transform, colPoint);
                break;
        }

        // CW rotation
        if(direction < 0)
        {
            // Extern detection
            if(relativePoint.x < 0)
            {
                if(_idStiCo)
                {
                    distance--;
                    if(distance < minDist)
                        distance = minDist;
                }
                curDist = minDist;
                Quaternion rot =  Quaternion.Euler(0, 0, direction * 90);
                gameObject.transform.rotation *= rot;
            }
            // Intern detection
            else
            {
                if(_idStiCo)
                {
                    distance--;
                    if(distance < minDist)
                        distance = minDist;
                }
                direction = -direction;
                Quaternion rot =  Quaternion.Euler(0, 0, direction * 90);
                gameObject.transform.rotation *= rot;
            }
        }
        // CCW rotation
        else
        {
            // Intern detection
            if(relativePoint.x < 0)
            {
                if(_idStiCo)
                {
                    distance--;
                    if(distance < minDist)
                        distance = minDist;
                }
                direction = -direction;
                Quaternion rot =  Quaternion.Euler(0, 0, direction * 90);
                gameObject.transform.rotation *= rot;
            }
            // Extern detection
            else
            {
                if(_idStiCo)
                {
                    distance--;
                    if(distance < minDist)
                        distance = minDist;
                }
                curDist = minDist;
                Quaternion rot =  Quaternion.Euler(0, 0, direction * 90);
                gameObject.transform.rotation *= rot;
            }
        }
    }

    // While the robot is still colliding with something
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "Marker")
            if(col.gameObject.GetComponent<Marker>().GetId()==id)
                return;
        if(col.gameObject.tag == "Robot")
            if(col.gameObject.GetComponent<StiCo>().GetId()==id)
                return;
        noCollisionTimer = 0.0f;
    }

    // When the robot leaves the collision area
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "Marker")
            if(col.gameObject.GetComponent<Marker>().GetId()==id)
                return;
        if(col.gameObject.tag == "Robot")
            if(col.gameObject.GetComponent<StiCo>().GetId()==id)
                return;

        curDist = distance;
    }

    // Move the robot
    void Move()
    {
        // A-StiCo
        if(_aStiCo)
        {
            noCollisionTimer += Time.deltaTime;
            if(noCollisionTimer >= timeIncrease)
            {
                if(curDist < maxDist)
                {
                    distance += 5;
                    if(distance > maxDist)
                        distance = maxDist;
                    curDist = distance;
                }
                noCollisionTimer = 0.0f;
            }
        }
        
        // Movement
        float dist = Vector3.Distance(lastCorner, gameObject.transform.position);
        if(dist >= distance)
        {
            Quaternion rot =  Quaternion.Euler(0, 0, direction * 90);
            gameObject.transform.rotation *= rot;
            lastCorner = gameObject.transform.position;
        }
        gameObject.transform.Translate(Vector3.up * v0 * Time.deltaTime);
    }

    // Get position relative to the origin transform
    Vector3 getRelativePosition(Transform origin, Vector3 position) {
        Vector3 dist = position - origin.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(origin.right.normalized, dist);
        relativePosition.y = Vector3.Dot(origin.up.normalized, dist);
        relativePosition.z = Vector3.Dot(origin.forward.normalized, dist);
         
        return relativePosition;
    }

    // Returns Id
    public int GetId()
    {
        return id;
    }
}