using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject UIManager;

    private void OnCollisionEnter(Collision collision)
    {
        // If this collider collides with the Player
        if (collision.gameObject.tag == "Player")
        {
            // Add 25 Score to the UI Manager
            UIManager.GetComponent<UIManager>().AddScore(25);

            // Destroy the Collectable
            Destroy(this.gameObject);
        }
    }
}
