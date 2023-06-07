using ABMU.Core;

using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    private bool isPaused = false;
    private AbstractController controller;

    private void Start()
    {
        Button button = GameObject.Find("PauseButton").GetComponent<Button>();
        controller = GameObject.Find("WorldController").GetComponent<AbstractController>();
        button.onClick.AddListener(TogglePause);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            controller.isSimulationPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            controller.isSimulationPaused = false;
        }
    }
}