using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRotate : MonoBehaviour
{
    // Manually set the rotation amoun on the X, Y or Z axis
    public float rotAmountX;
    public float rotAmountY;
    public float rotAmountZ;
	
	// Update is called once per frame
	void Update ()
    {
        // Rotates this GameObject on the amount inputed to the float
        this.gameObject.transform.Rotate(rotAmountX, rotAmountY, rotAmountZ);
	}
}
