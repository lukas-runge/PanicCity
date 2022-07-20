using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StateHandler : MonoBehaviour {
    // "global" state boolean
    public bool runningState = false;

    // reference to canvas objects
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;
    public Toggle toggle1;
    public TMP_Text ButtonTextObject;

    // reference to TimerController
    public TimerController Timer;

    public void toggleRunning() { // state mutation function
        if (!runningState) { // start simulation
            runningState = true;
            Timer.startTimer();
            ButtonTextObject.text = "RESET";
        } else { // stop simulation
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }    
    }
}