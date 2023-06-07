using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionBlocked : MonoBehaviour
{
    public BuildManager builder;
    public UserControls Controls;
    // Start is called before the first frame update
    void Start()
    {
        builder = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildManager>();
    }
    void OnTriggerEnter(Collider collision){
        if(collision.gameObject.CompareTag("Building")){
            for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
            {
                GetComponent<Renderer>().materials[i].SetColor("_Color", Color.red);
            }
            builder.canBuild = false;
        }
    }

    void OnTriggerExit(){
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            GetComponent<Renderer>().materials[i].SetColor("_Color", Color.green);
        }
        builder.canBuild = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
