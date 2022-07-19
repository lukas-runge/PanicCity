using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class movementController : MonoBehaviour
{
    private StateHandler State;
    private MathProcessor Math;
    private List<Vector2> forces;
    private Rigidbody2D rb;
    public bool doorReached = false;
    public float desiredSpeed;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();

        //Find StateHandler in the scene
        State = GameObject.FindObjectOfType<StateHandler>();

        //Find MathProcessor in the scene
        Math = GameObject.FindObjectOfType<MathProcessor>();

        // get Mass of rb
        rb.mass = rb.mass + Random.Range(-20f, 20f);

        // randomize desired speed
        desiredSpeed = Random.Range(0.9f, 1.7f);
        // desiredSpeed = 1.3f;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Door") {
            doorReached = true;
        }

        if (col.gameObject.tag == "Sammelplatz") {
            // despawn entity
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        if (State.runningState == false) {
            return;
        }        
        
        //get new list of forces from MathProcessor
        forces = Math.getForces(gameObject);

        // // Clear Velocity
        // rb.velocity = Vector2.zero;

        // Apply Forces
        foreach (var force in forces)
        {
            // add the force to the rigidbody
            rb.AddForce(force);
        }
    }
}
