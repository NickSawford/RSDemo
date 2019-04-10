using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Declare the Position Array and the next position
    public Transform[] positionArray;
    private Vector3 nextPos;

    // Declare for Speed and if the Platforms Loop or not
    public float speed;
    public bool loopAround;

    // Declare the pointer for the Array Position and if its positive or negative
    private int pointer;
    private bool positive;

    // Use this for initialization
    void Start()
    {

        // Set Pointer to the second in the Array and Positive to true to increase the Pointer
        pointer = 2;
        positive = true;

        // Collects all the childs transform positions into the Array
        positionArray = new Transform[transform.childCount];        

        for (int i = 0; i < transform.childCount; i++)
        {
            positionArray[i] = transform.GetChild(i);
        }

        // Sets the next position to the second in the Array
        nextPos = positionArray[2].position;
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the first position in the Array as point A and the Next Position as B and moves toward it at the speed of what is set
        positionArray[0].transform.position = Vector3.MoveTowards(positionArray[0].transform.position, nextPos, speed * Time.deltaTime);

        // If point A equals point B then change position
        if (positionArray[0].transform.position == nextPos)
        {
            ChangePosition();
        }
    }

    // Change Position Method
    private void ChangePosition()
    {
        // Set the next position to be that of the pointer in the Array
        nextPos = positionArray[pointer].position;

        // If Loop has been set to true and the pointer is less than 1 of the length of the Array then set the pointer to the first position of the Array
        if (loopAround)
        {
            if (pointer >= positionArray.Length - 1)
            {
                pointer = 0;
            }

            pointer++;
        }
        else
        {   
            // If Loop has not been set, check if the order is positive or negative          
            if (positive)
            {   
                // Increase the pointer and if the pointer reaches the end of the array set Positive to false and set the Pointer to the second from last position in the Array          
                pointer++;

                if (pointer >= positionArray.Length - 1)
                {
                    positive = false;
                    pointer = positionArray.Length - 1;
                }
            }
            else
            {   // If the pointer is set to negative, decrease the pointer until it reaches 1 and set positive to true                
                pointer--;

                if (pointer <= 1)
                {
                    positive = true;
                    pointer = 1;
                }
            }
        }  
    }

    // OnDrawGizmoSelected for Debug to see the route of the Platforms
    void OnDrawGizmosSelected()
    {
        // Declares the Position Array to be the length of the amount of children
        positionArray = new Transform[transform.childCount];

        // Inputs the children into the Array
        for (int i = 0; i < transform.childCount; i++)
        {
            positionArray[i] = transform.GetChild(i);
        }

        // Loops through each child in the array and connects a line to each platform (A line from A to B, B to C and so on)
        for (int i = 0; i < transform.childCount - 2; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(positionArray[i + 1].position, positionArray[i + 2].position);
        }

        // If Loop is set, draw a line between the last position in the Array to the first and sets the line to blue
        if (loopAround)
        {
            for (int i = 0; i < 1; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(positionArray[i + transform.childCount - 1].position, positionArray[i + 1].position);
            }
        }
    }
}
