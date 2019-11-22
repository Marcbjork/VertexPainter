using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    void Awake()
    {
        transform.position = new Vector3(0.75f, 0.0f, 0.0f);
    }
    void Update()
    {
        if (Input.GetKey("up"))
        {
            //Rotera Upp eller ner
            transform.Rotate(1.0f,0f,0f, Space.World);
        }
        if (Input.GetKey("down"))
         {
            transform.Rotate(-1.0f, 0f, 0f, Space.World);
        }
        if (Input.GetKey("left"))
        {
            //Rotera åt vänster eller höger
            transform.Rotate(0.0f, 0f, 1.0f, Space.World);
        }
        if (Input.GetKey("right"))
        {
            //Rotera åt vänster eller höger
            transform.Rotate(0.0f, 0f, -1.0f, Space.World);
        }
        if (Input.GetKey("p"))
        {   //Reset kuben
            ResetCube();
        }

    }

    void ResetCube()
    {
        //Reseta kuben position här
        transform.rotation = transform.rotation;
    }
}
