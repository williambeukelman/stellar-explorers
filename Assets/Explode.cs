using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, exp.main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
