using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU;
using ABMU.Core;

public class SimController : AbstractController
{
    public enum shape{
        Circle,
        Square,
        Triangle
    };
    public shape s;

    [Header("Global")]
    public int population;
    public float v0;
    [Header("Circle")]
    public GameObject robotCirclePrefab;
    public float w0;
    public float maxCurve;
    public float minCurve;

    [Header("Square / Triangle")]
    public GameObject robotSquarePrefab;
    public GameObject robotTrianglePrefab;
    public float dist;
    public float maxDist;
    public float minDist;

    [Header("Extensions")]
    public bool AStiCo;
    public float timeIncrease;
    public bool IdStiCo;

    [Header("Marker")]
    public float timeLimit = 1.5f;

    public void Init(shape simShape, int pop, float vel, float param1, float param2, float param3, bool a, float ti, bool id)
    {
        base.Init();

        s = simShape;
        population = pop;
        v0 = vel;
        if(s == shape.Circle)
        {
            w0 = param1;
            maxCurve = param2;
            minCurve = param3;
        }
        else
        {
            dist = param1;
            maxDist = param2;
            minDist = param3;
        }

        AStiCo = a;
        timeIncrease = ti;
        IdStiCo = id;

        Vector2 pos = Vector2.zero;
        for(int i=0; i<population; i++)
        {
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
            GameObject robot;
            switch(s)
            {
                case shape.Circle :
                    robot = Instantiate(robotCirclePrefab, pos, rot);
                    robot.GetComponent<StiCo>().Init(i, v0, w0, minCurve, maxCurve, timeLimit, timeIncrease, AStiCo, IdStiCo);
                    break;
                case shape.Square :
                    robot = Instantiate(robotSquarePrefab, pos, rot);
                    robot.GetComponent<StiCoSquare>().Init(i, v0, dist, minDist, maxDist, timeLimit, timeIncrease, AStiCo, IdStiCo);
                    break;
                case shape.Triangle :
                    robot = Instantiate(robotTrianglePrefab, pos, rot);
                    robot.GetComponent<StiCoTriangle>().Init(i, v0, dist, minDist, maxDist, timeLimit, timeIncrease, AStiCo, IdStiCo);
                    break;
            }
        }
    }
}
