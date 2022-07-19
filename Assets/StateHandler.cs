using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// Include the name space for TextMesh Pro
using TMPro;

public class StateHandler : MonoBehaviour
{
    public bool runningState = false;
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;
    public Toggle toggle1;
    public TimerController Timer;
    public TMP_Text ButtonTextObject;

    public void toggleRunning()
    {
        if (!runningState)
        {
            Debug.Log("Starting");
            runningState = true;
            ButtonTextObject.text = "RESET";
            Debug.Log("Running");
            Timer.startTimer();
        } else {
            Debug.Log("Stopping");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }    
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
