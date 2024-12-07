using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PressEcollision : MonoBehaviour
{
    public TextMeshProUGUI pressE;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag == "Player"){
            pressE.text = "Press E to pick up food";
        pressE.enabled = true;

        }
    }
     private void OnTriggerExit(Collider other)
    {


        if (other.gameObject.tag == "Player"){
            pressE.text = "";
        pressE.enabled = false;

        }
    }
}
