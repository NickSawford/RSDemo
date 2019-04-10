using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LoadManager : MonoBehaviour
{
    // Declare the Overlay from the Canvas and the amount at which it will fade
    public CanvasRenderer overlay;
    public int fadeTime;

    void Awake()
    {
        // Fade to clear
        StartCoroutine(FadeToClear());
    }

    // Loads the next Level in the Array set by Unity's Build Settings
    public void LoadNextLevel()
    {
        // If the current Scene is less than the total of scenes in the Build Settings minus 1 than load the next scene and fade to black 
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            StartCoroutine(FadeToBlack(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single)));           
        }
    }

    // Loads a specific scene in the array, identified by the scenes int and fades to black
    public void LoadSpecific(int index)
    {
        StartCoroutine(FadeToBlack(() => SceneManager.LoadScene(index, LoadSceneMode.Single)));        
    }

    // If Unity is in the editor than stop playing otherwise if the game is running quit the application
    public void QuitGame()
    {
#if UNITY_EDITOR
        StartCoroutine(FadeToBlack(() => UnityEditor.EditorApplication.isPlaying = false));
#else
        StartCoroutine(FadeToBlack(() => Application.Quit()));
#endif
    }

    // Fade from black to clear method using an IEnumerator which stops the rest of the program until this method is complete
    private IEnumerator FadeToClear()
    {
        // Sets the overlay GameObjet to active and sets it colour to black
        overlay.gameObject.SetActive(true);
        overlay.SetColor(Color.black);
        
        // Declares and sets the rate and progress of the fade
        float rate = 1.0f / fadeTime;

        float progress = 0.0f;

        // While the progress is less than 1 Lerp the Colour from black to clear (using Alpha) while increasing the progress and executing through each frame
        while (progress < 1.0f)
        {
            overlay.SetColor(Color.Lerp(Color.black, Color.clear, progress));

            progress += rate * Time.deltaTime;

            yield return null;
        }

        // Sets the Overlay to clear and then deactivates it when not seen
        overlay.SetColor(Color.clear);
        overlay.gameObject.SetActive(false);
    }

    // Fade to black method using an IEnumerator which stops the rest of the program until this method is complete
    private IEnumerator FadeToBlack(Action levelMethod)
    {
        // Sets the overlay GameObjet to active and sets it colour to clear (Full Alpha)
        overlay.gameObject.SetActive(true);
        overlay.SetColor(Color.clear);

        // Declares and sets the rate and progress of the fade
        float rate = 1.0f / fadeTime;

        float progress = 0.0f;

        // While the progress is less than 1 Lerp the Colour from clear to black (using Alpha) while increasing the progress and executing through each frame
        while (progress < 1.0f)
        {
            overlay.SetColor(Color.Lerp(Color.clear, Color.black, progress));

            progress += rate * Time.deltaTime;

            yield return null;
        }

        // Sets the Overlay to Black and continues with the Coroutine
        overlay.SetColor(Color.black);
        levelMethod();
    }
}
