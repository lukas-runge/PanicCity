using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerController : MonoBehaviour {
    public bool running = false; // "global" running state
    public float passedTime; // current time

    public TMP_Text TimerTextObject; // reference to timer text
    private StateHandler State; // reference to Canvas via State

    void Awake() {
		// find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();
	}

    public void startTimer() {
        passedTime = 0; // reset time
        running = true; // start the timer
    }

    void Update() {
        if (running) {
            int humanCountInScene = countGameObjectsWithTag("Human");
            bool noHumansInScene = humanCountInScene == 0;

            if (noHumansInScene) { // humans have successfully escaped
                running = false; // stop the timer
                logRunthru(passedTime); // log time to *.csv file
            }

            passedTime += Time.deltaTime; // add passed time
            updateTimer(passedTime); // update timer text
        }
    }

    private void updateTimer(float time) {
        float hours = Mathf.FloorToInt(time / 3600);
        float minutes = Mathf.FloorToInt(time / 60);  
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = Mathf.FloorToInt((time % 1) * 100);
        TimerTextObject.text = string.Format("Timer: {0:00}:{1:00}:{2:00}.{3:00}", hours, minutes, seconds, milliseconds);
    }

    private void logRunthru(float time){
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

    private void WriteToLogFile(float tau_i, float A_i, float B_i, float lambda_i, bool panic, string time) {
        using(System.IO.StreamWriter logFile = new System.IO.StreamWriter(@"C:\Users\Public\panicLog_" + SceneManager.GetActiveScene().name + ".csv", true)){
            logFile.WriteLine(tau_i + ";" + A_i + ";" + B_i + ";" + lambda_i + ";" + panic + ";" + time);
        }
    }

    private int countGameObjectsWithTag(string tag) {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }
}