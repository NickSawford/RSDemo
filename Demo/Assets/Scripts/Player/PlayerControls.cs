using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // Declares the Player Gameobject and the Player Data script
    private GameObject playerGO;
    private PlayerData playerData;

    // Declares the UI Gameobject and the UI Manager script
    private GameObject UiGO;
    private UIManager UIManager;

	// Use this for initialization
	void Start ()
    {
        // Finds the GameObject with the tag of Player and sets script of the Player to Player Data
        playerGO = GameObject.FindWithTag("Player");
        playerData = playerGO.GetComponent<PlayerData>();

        // Finds the GameObject with the tag of UIManager and sets script of the UI to UIManager
        UiGO = GameObject.FindWithTag("UI Manager");
        UIManager = UiGO.GetComponent<UIManager>();
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        // Calls each Movement method from the Player Data script and sends through an Input for each set in the Input in the Unity Project Settings
        playerData.Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerData.RotateLookMouse(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        playerData.RotateLookGamePad(Input.GetAxis("Right Joystick X"), Input.GetAxis("Right Joystick Y"));
        playerData.Jump(Input.GetButtonDown("Jump"));
        playerData.Crouch(Input.GetButton("Crouch"));
        playerData.Teleport(Input.GetButtonDown("Mark"), Input.GetButtonDown("Teleport"));
        UIManager.Pause(Input.GetButtonDown("Pause"));
    }
}
