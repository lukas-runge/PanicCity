using System.Collections;
using UnityEngine;

public class TestController : MonoBehaviour {
	private bool testRunning = false; // current running state
	private int repetitions = 0; // successful repetitions count

	// inspector configured variables 
	public bool enableTestSeries = false;
	public int testRepetitions = 10;
	public float tau_i = 2.5f;
	public float A_i = 0.5f;
	public float B_i = 0.5f;
	public float lambda_i = 0.5f;
	public bool panic = false;

	private StateHandler State; // reference to StateHandler
	private TimerController Timer; // reference to TimerController

	public void Awake() {
		int numTestControllers = FindObjectsOfType<TestController>().Length;
		bool TestControllerAlreadyExists = numTestControllers != 1;

		if (TestControllerAlreadyExists) {
			Destroy(this.gameObject); // destroy self
		} else {
			DontDestroyOnLoad(gameObject); // make self persistent
		}
	}

	void Start() {
		if (enableTestSeries && repetitions < testRepetitions) {
			StartCoroutine(startSimWithDelay());
		}
	}

	void Update() {
		if (testRunning == true) {
			if (Timer.running == false) { // successfull run
				State.toggleRunning();
				testRunning = false;

				Start(); // conduct new test if necessary
			}

			// abort test if more than 60 seconds have passed
			if (Timer.passedTime > 60) { // failed simulation run
				State.toggleRunning();
				testRunning = false;
				repetitions--; // dont count this test as it failed

				Start(); // retry test
			}
		}
	}

	private IEnumerator startSimWithDelay() {
		// wait 1 sec for scene to load
		yield return new WaitForSeconds(1);

		// find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();

		// find TimerController in scene
		Timer = GameObject.FindObjectOfType<TimerController>();

		// set parameters to the inspector configured values
		State.slider1.value = tau_i;
		State.slider2.value = A_i;
		State.slider3.value = B_i;
		State.slider4.value = lambda_i;
		State.toggle1.isOn = panic;

		// start the simulation
		State.toggleRunning();
		testRunning = true;
		repetitions++;
	}
}