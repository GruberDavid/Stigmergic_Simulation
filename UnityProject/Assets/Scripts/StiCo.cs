using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU;
using ABMU.Core;

public class StiCo : AbstractAgent
{
    [SerializeField]
    private int id;
    [SerializeField]
    private float v0;
    [SerializeField]
    private float curW;
    [SerializeField]
    private float w0;
    [SerializeField]
    private float maxCurve;
    [SerializeField]
    private float minCurve;
    [SerializeField]
    private float timeLimit;

    public GameObject markerPrefab;

    [SerializeField]
    private bool _aStiCo;
    [SerializeField]
    private bool _idStiCo;
    private float noCollisionTimer;
    private float timeIncrease;

    // Robot values init
    public void Init(int i, float v, float w, float minC, float maxC, float tl, float ti, bool astico, bool idstico)
    {
        base.Init();

        id = i;
        v0 = v;
        w0 = w;
        curW = (Random.Range(0,2)*2-1) * w0;
        maxCurve = maxC;
        minCurve = minC;
        timeLimit = tl;

        _aStiCo = astico;
        _idStiCo = idstico;
        timeIncrease = ti;
        noCollisionTimer = 0.0f;

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
        if(curW < 0)
        {
            // Extern detection
            if(relativePoint.x < 0)
            {
                if(curW > -maxCurve)
                {
                    if(_idStiCo)
                    {
                        w0 += 5;
                        if(w0 > maxCurve)
                            w0 = maxCurve;
                    }
                    curW = -maxCurve;
                }
            }
            // Intern detection
            else
            {
                if(_idStiCo)
                {
                    w0 += 5;
                    if(w0 > maxCurve)
                        w0 = maxCurve;
                }
                curW = -curW;
            }
        }
        // CCW rotation
        else
        {
            // Intern detection
            if(relativePoint.x <= 0)
            {
                if(_idStiCo)
                {
                    w0 += 5;
                    if(w0 > maxCurve)
                        w0 = maxCurve;
                }
                curW = -curW;
            }
            // Extern detection
            else
            {
                if(curW < maxCurve)
                {
                    if(_idStiCo)
                    {
                        w0 += 5;
                        if(w0 > maxCurve)
                            w0 = maxCurve;
                    }
                    curW = maxCurve;
                }
            }
        }
    }

    // While the robot is still colliding with something
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "Marker")
        {
            if(col.gameObject.GetComponent<Marker>().GetId()==id)
                return;
        }
        else if(col.gameObject.tag == "Robot")
        {
            if(col.gameObject.GetComponent<StiCo>().GetId()==id)
                return;
        }
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

        if(curW < 0)
            curW = -w0;
        else
            curW = w0;
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
                if((curW < 0) && (curW < -minCurve))
                {
                    w0 -= 5;
                    if(w0 < minCurve)
                        w0 = minCurve;
                    curW = -w0;
                }
                if((curW > 0) && (curW > minCurve))
                {
                    w0 -= 5;
                    if(w0 < minCurve)
                        w0 = minCurve;
                    curW = w0;
                }
                noCollisionTimer = 0.0f;
            }
        }
        
        // Movement
        Quaternion rot =  Quaternion.Euler(0, 0, curW * Time.deltaTime);
        gameObject.transform.rotation *= rot;
        gameObject.transform.Translate(Vector3.up * v0 * Time.deltaTime);
    }

    // Get position relative to the origin transform
    Vector3 getRelativePosition(Transform origin, Vector3 position) {
        Vector3 distance = position - origin.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(origin.right.normalized, distance);
        relativePosition.y = Vector3.Dot(origin.up.normalized, distance);
        relativePosition.z = Vector3.Dot(origin.forward.normalized, distance);
         
        return relativePosition;
    }

    // Returns Id
    public int GetId()
    {
        return id;
    }
}