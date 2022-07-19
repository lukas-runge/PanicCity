using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
	private StateHandler State;
	private TimerController Timer;

	private bool testRunning = false;
	public bool enableTestSeries = false;
	public int testRepetitions = 10;
    public float tau_i = 2.5f;
    public float A_i = 0.5f;
    public float B_i = 0.5f;
    public float lambda_i = 0.5f;
    public bool panic = false;
	private int repetitions = 0;
	private float startTime;
	private static TestController TestControllerInstance;
	public void Awake()
	{
		int numTestControllers = FindObjectsOfType<TestController>().Length;
		if (numTestControllers != 1)
		{
			Destroy(this.gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
			if (enableTestSeries)
			{
				Debug.Log("Starting test series");

			}
		}
	}

	private IEnumerator startSimWithDelay()
	{
		//yield on a new YieldInstruction that waits for 5 seconds.
		yield return new WaitForSeconds(1);

        //Find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();

		//Find TimerController in scene
		Timer = GameObject.FindObjectOfType<TimerController>();
        
        // set Parameters to the current values of the sliders
        State.slider1.value = tau_i;
        State.slider2.value = A_i;
        State.slider3.value = B_i;
        State.slider4.value = lambda_i;
        State.toggle1.isOn = panic;

		State.toggleRunning();

        // remember current start time
        startTime = Time.time;

        testRunning = true;
        repetitions++;
        Debug.Log("Starting test " + repetitions);
	}

	// Start is called before the first frame update
	void Start()
	{
		if (enableTestSeries && repetitions < testRepetitions)
		{
			StartCoroutine(startSimWithDelay());
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (testRunning == true)
		{
			if (Timer.running == false) {
			    Debug.Log("Test " + repetitions + " finished");
			    testRunning = false;
			    State.toggleRunning();
			    Start();
			}

			// if more than one minute elaplsed since start, abort test
			if (Time.time - startTime > 60)
			{
				Debug.Log("Test " + repetitions + " aborted");
				testRunning = false;
				State.toggleRunning();
				repetitions--;
				Start();
			}
		}
	}
}
