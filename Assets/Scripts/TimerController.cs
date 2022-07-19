using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// Include the name space for TextMesh Pro
using TMPro;

public class TimerController : MonoBehaviour
{
    public TMP_Text TimerTextObject;
    public bool running = false;
    private float time;
    private StateHandler State;

    public void startTimer(){
        time = 0;
        running = true;
    }
    void WriteToLogFile(float tau_i, float A_i, float B_i, float lambda_i, bool panic, string time){
        using(System.IO.StreamWriter logFile = new System.IO.StreamWriter(@"C:\Users\Public\panicLog_" + SceneManager.GetActiveScene().name + ".csv", true)){
            logFile.WriteLine(tau_i + ";" + A_i + ";" + B_i + ";" + lambda_i + ";" + panic + ";" + time);
        }
    }

    void logRunthru(float time){
        float tau_i = State.slider1.value;
        float A_i = State.slider2.value;
        float B_i = State.slider3.value;
        float lambda_i = State.slider4.value;
        bool panic = State.toggle1.isOn;
        float hours = Mathf.FloorToInt(time / 3600);
        float minutes = Mathf.FloorToInt(time / 60);  
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = Mathf.FloorToInt((time % 1) * 100);
        string timeString = string.Format("{0:00}:{1:00}:{2:00},{3:00}", hours, minutes, seconds, milliseconds);
        WriteToLogFile(tau_i, A_i, B_i, lambda_i, panic, timeString);
    }

    public void Awake()
	{
		//Find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (running) {
            // check if there are still GameObject on the tag "Human" in the scene and if not stop the timer
            if (GameObject.FindGameObjectsWithTag("Human").Length == 0) {
                running = false;
                logRunthru(time);
            }

            time += Time.deltaTime;
            // display time as hh:mm:ss:ms
            float hours = Mathf.FloorToInt(time / 3600);
            float minutes = Mathf.FloorToInt(time / 60);  
            float seconds = Mathf.FloorToInt(time % 60);
            float milliseconds = Mathf.FloorToInt((time % 1) * 100);
            TimerTextObject.text = string.Format("Timer: {0:00}:{1:00}:{2:00}.{3:00}", hours, minutes, seconds, milliseconds);
        }
        
    }
}
