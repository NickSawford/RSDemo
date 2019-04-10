using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Declare the Score amount and the UI Score Text
    public int score;
    public Text scoreText;

    // Declares the canvas for access to the Pause Menu
    public Canvas UI; 

    // Start is called before the first frame update
    void Start()
    {
        // Set the Score to start at 0
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the Score Text to display the actual Score
        scoreText.text = "" + score;
    }

    // Add Score method to handle the increases in the score
    public virtual void AddScore(int amount)
    {
        // Add the amount of score that comes in to the total of the Score 
        score += amount;
    }

    // Pause Method
    public virtual void Pause(bool pause)
    {
        // If pause input is given, set the menu screen to active and unlock the cursor
        if (pause)
        {
            UI.transform.GetChild(0).gameObject.SetActive(true);
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }       
    }

    // Unpause method that handles restoring the time and reverting the mouse to being locked and invisible
    public virtual void UnpauseTime()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;             
    }
}
