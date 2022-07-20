using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class movementController : MonoBehaviour {
	public bool doorReached = false;
	public float desiredSpeed = 1.3f; // average speed of humans

	private Rigidbody2D rb; // reference to rigidbody
	private StateHandler State; // reference to StateHandler
	private MathProcessor Math; // reference to MathProcessor

	void Awake() {
		// find Rigidbody2D on the entity
		rb = GetComponent<Rigidbody2D>();

		// find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();

		// find MathProcessor in the scene
		Math = GameObject.FindObjectOfType<MathProcessor>();

		// get mass from rigidbody and randomize it
		rb.mass = rb.mass + Random.Range(-20f, 20f);

		// randomize desired speed
		desiredSpeed = desiredSpeed + Random.Range(-0.4f, 0.4f);
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Door") {
			doorReached = true;
		}

		if (col.gameObject.tag == "Sammelplatz") {
			Destroy(gameObject); // despawn human
		}
	}

	void FixedUpdate() {
		if (State.runningState == false) { // simulation stopped
			return; // do nothing
		}

		//get new list of forces from MathProcessor and apply them to rb
		foreach (var force in Math.getForces(gameObject)) {
			rb.AddForce(force);
		}
	}
}