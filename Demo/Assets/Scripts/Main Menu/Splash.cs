using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour
{

    // Declare the Load Counter and the Total
    public float loadCount;
    public int loadTotal;

    // Set the Load Manager GameObject
    public GameObject loadManagerOb;

	// Use this for initialization
	void Start ()
    {
        // Set the Load Count to 0 so it can count up
        loadCount = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Increase the Load Count by seconds
        loadCount += 1 * Time.deltaTime;

        // If the Counter reaches the total, load the next level
        if (loadCount >= loadTotal)
        {
            loadManagerOb.GetComponent<LoadManager>().LoadNextLevel();
        }
	}
}
