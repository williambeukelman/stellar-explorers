using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceCamera : MonoBehaviour
{
    public Vector3 angle = new Vector3(0, 1, 0);
    public GameObject tracks;
    // Start is called before the first frame update
    void Start()
    {
        tracks = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = gamecamera.transform.rotation;
        transform.LookAt(tracks.transform.position);
        //Debug.Log(tracks.transform.rotation.normalized);
        //transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,0);
    }
}
