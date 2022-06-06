using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public SpriteRenderer sr;

    private int id;
    private float lifeTime;
    private float timeLimit;

    public void Init(int i, float tl)
    {
        id = i;
        lifeTime = 0.0f;
        timeLimit = tl;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        // Destroy the Marker if time limit is over
        if(lifeTime >= timeLimit)
            Destroy(gameObject);

        // Set the alpha based on life time
        Color c = sr.color;
        c.a = 1-(lifeTime/timeLimit);
        sr.color = c;
    }

    public int GetId()
    {
        return id;
    }
}
