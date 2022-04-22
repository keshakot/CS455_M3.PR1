using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;
    public float restartDelay = 1f;
    public GameObject completeLevelUI;
    public Text goalText;
    public GOBMovement gobm;

    int qsize = 10;
    Queue myLogQueue = new Queue();

    public void CompleteLevel()
    {
    }

    public void EndGame()
    {
    }

    void Start(){
        
    }

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height / 2 + 200, 400, Screen.height / 2));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }

    void Update(){
    }
    
    void Restart ()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}
