using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    //public Rigidbody rb;
    //public GameObject sun;
    
    public float orbitSpeed = 30f;
    public float rotationSpeed = 10f;
    public float planetDistance;
    //public Vector3 planet_tilt = new Vector3(0, 1, 0);
    //private GameObject sun;
    //private GameObject orbitLine;
    // Start is called before the first frame update
    /* void Start()
    {
        //sun = GameObject.FindWithTag("Sun");
        orbitLine = new GameObject { name = "circle_"+name };
    } */

    // Update is called once per frame
    /* void Update()
    {
        planetDistance = (transform.position - new Vector3(0,0,0)).magnitude;
        orbitLine.DrawCircle(planetDistance, .5f);
        transform.RotateAround(new Vector3(0,0,0), Vector3.up, orbitSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    } */
}
