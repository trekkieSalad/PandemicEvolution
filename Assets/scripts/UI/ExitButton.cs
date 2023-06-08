using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    private void Start()
    {
        Button button = GameObject.Find("ExitButton").GetComponent<Button>();
        button.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}

