using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMove : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;

    // Update is called once per frame
    //void Update()
    //{
    //    transform.position += (Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward) * speed * Time.deltaTime;
    //}

    void Update()
    {
        float step = speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        //Vector3 CurrentPos = transform.position;
        Vector3 CurrentPos = Vector3.MoveTowards(transform.position, target.transform.position, step);
        CurrentPos.y = Terrain.activeTerrain.SampleHeight(transform.position);
        transform.position = CurrentPos;
    }
}
