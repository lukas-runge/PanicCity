using System.Collections.Generic;
using UnityEngine;

public class MathProcessor : MonoBehaviour {
	public LayerMask HumanMask;
	public LayerMask DoorMask;
	public LayerMask WallMask;
	public LayerMask SammelplatzMask;

	private StateHandler State; // reference to StateHandler
	private bool panic; // reference to panic state

	void Awake() {
		// find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();
		panic = State.toggle1.isOn;
	}

	public List<Vector2> getForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get mainForce and add them to the list
		forces.Add(getMainForce(entity));

		// get wallForce and add them to the list
		forces.Add(getWallForces(entity));

		// get sozForce and add them to the list
		forces.Add(getSozForces(entity));

		if (panic == true) {
			// get panicForces and add them to the list
			forces.Add(getPanicForces(entity));
		}

		return forces;
	}

	private Vector2 getMainForce(GameObject entity) {
		Vector2 desiredDirection = Vector2.zero; // init desired direction

		// observe sourroundings and determine desired direction
		if (entity.GetComponent<movementController>().doorReached == false) {
			// get vector to nearest door
			Collider2D[] doorColliders = findColliders(entity, 100f, DoorMask);
			desiredDirection = getVecToNearestCollider(entity, doorColliders);
		} else {
			// get vector to nearest sammelplatz
			Collider2D[] sammelplatzColliders = findColliders(entity, 100f, SammelplatzMask);
			desiredDirection = getVecToNearestCollider(entity, sammelplatzColliders);
		}

		// define static calculation variables
		Vector2 e_i0 = desiredDirection.normalized;
		float v_i0 = entity.GetComponent<movementController>().desiredSpeed;
		Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
		float tau_i = State.slider1.value;
		float m = entity.GetComponent<Rigidbody2D>().mass;

		// define dynamic calculation variables (randomness)
		float rndStrength = 1.5f;
		float rndWidth = 90.0f;
		Vector2 xi_i = rndStrength * getRndDir(desiredDirection.normalized, rndWidth);

		// calculate force
		Vector2 result = ( ( ( e_i0 * v_i0 + xi_i ) - v_i ) / tau_i ) * m;

		return result;
	}

	private Vector2 getWallForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get radius of human
		float r_i = entity.transform.localScale.x;

		// find nearest colliders in the wall layer
		Collider2D[] waendeInRange = findColliders(entity, r_i * 5, WallMask);

		// if no colliders are found return zero
		if (waendeInRange.Length == 0) return Vector2.zero;

		// iterate thru colliders
		foreach (Collider2D wand in waendeInRange) {
			Vector2 entityPosition2D = entity.transform.position;
			Vector2 nearestPointOnColldier = wand.ClosestPoint(entityPosition2D);

			// define static calculation variables
			float d_ib = Vector2.Distance(entityPosition2D, nearestPointOnColldier);
			Vector2 n_ib = (entityPosition2D - nearestPointOnColldier).normalized;
			Vector2 t_ib = new Vector2(-n_ib.y, n_ib.x);
			Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
			float A_i = State.slider2.value;
			float B_i = State.slider3.value;

			// calculate force
			Vector2 res = ( Mathf.Pow( A_i, ( r_i - d_ib ) / B_i ) + theta( r_i - d_ib ) ) * n_ib - theta( r_i - d_ib ) * ( v_i * t_ib ) * t_ib;

			// amplify force to have an impact on pushing entity away from the wall
			int factor = 35;

			// add force to forces list
			forces.Add(res * factor);
		}

		// sum all forces
		Vector2 sum = Vector2.zero;
		foreach (Vector2 force in forces) {
			sum += force;
		}

		return sum;
	}

	private Vector2 getSozForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get radius of human
		float r_ij = entity.transform.localScale.x * 2;

		// find nearest colliders in the wall layer
		Collider2D[] humansInRange = findColliders(entity, r_ij * 2, HumanMask);

		// if no colliders are found return zero
		if (humansInRange.Length == 0) return Vector2.zero;

		// iterate thru humans
		foreach (Collider2D human in humansInRange) {
			Vector2 entityPosition2D = entity.transform.position;
			Vector2 humanPosition2D = human.transform.position;

			// define static calculation variables
			float d_ij = Vector2.Distance(entityPosition2D, humanPosition2D);
			Vector2 n_ij = (entityPosition2D - humanPosition2D).normalized;
			Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
			Vector2 e_i = v_i.normalized;
			float lamda_i = State.slider4.value;
			float A_i = State.slider2.value;
			float B_i = State.slider3.value;

			// calculate force
			Vector2 res = ( Mathf.Pow( A_i, ( r_ij - d_ij ) / B_i ) ) * n_ij * ( lamda_i + ( 1 - lamda_i ) * ( ( 1 + Mathf.Cos( Vector2.Angle( e_i, -n_ij ) ) ) / 2 ) );
			
			// add force to forces list
			forces.Add(res);
		}

		// sum all forces
		Vector2 sum = Vector2.zero;
		foreach (Vector2 force in forces) {
			sum += force;
		}

		return sum;
	}

	private Vector2 getPanicForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get radius of human
		float r_ij = entity.transform.localScale.x * 2;

		// find nearest colliders in the wall layer
		Collider2D[] humansInRange = findColliders(entity, r_ij, HumanMask);

		// if no colliders are found return zero
		if (humansInRange.Length == 0) return Vector2.zero;

		// iterate thru humans
		foreach (Collider2D human in humansInRange) {
			Vector2 entityPosition2D = entity.transform.position;
			Vector2 humanPosition2D = human.transform.position;

			// define static calculation variables
			float d_ij = Vector2.Distance(entityPosition2D, humanPosition2D);
			Vector2 n_ij = (entityPosition2D - humanPosition2D).normalized;
			Vector2 t_ij = new Vector2(-n_ij.y, n_ij.x);
			Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
			Vector2 v_j = human.GetComponent<Rigidbody2D>().velocity;
			Vector2 e_i = v_i.normalized;
			float lamda_i = State.slider4.value;
			float A_i = State.slider2.value;
			float B_i = State.slider3.value;
			Vector2 delta_vt = (v_j - v_i) * t_ij;

			// calculate force
			Vector2 res = theta( r_ij - d_ij ) * n_ij + theta( r_ij - d_ij ) * delta_vt * t_ij;

			// add force to forces list
			forces.Add(res);
		}

		// sum all forces
		Vector2 sum = Vector2.zero;
		foreach (Vector2 force in forces) {
			sum += force;
		}

		return sum;
	}

	private float theta(float x) {
		if (x >= 0) return x;
		return 0.0f;
	}

	private Vector2 getRndDir(Vector2 dir, float angle) {
		Vector2 res = dir;
		float rnd = Random.Range(-angle, angle);
		res = Quaternion.AngleAxis(rnd, Vector3.forward) * res;
		return res;
	}

	private Vector2 getVecToNearestCollider(GameObject entity, Collider2D[] colliders) {
		Shuffle(colliders);
		Collider2D nearestCollider = colliders[0];
		float nearestDistance = (nearestCollider.transform.position - entity.transform.position).magnitude;

		foreach (Collider2D collider in colliders) {
			float distance = (collider.transform.position - entity.transform.position).magnitude;
			if (distance < nearestDistance) {
				nearestCollider = collider;
				nearestDistance = distance;
			}
		}

		return nearestCollider.transform.position - entity.transform.position;
	}

	private Collider2D[] findColliders(GameObject entity, float range, LayerMask layer) {
		return Physics2D.OverlapCircleAll(entity.transform.position, range, layer);;
	}

	private void Shuffle<T>(T[] inputList) {
		for (int i = 0; i < inputList.Length - 1; i++) {
			T temp = inputList[i];
			int rand = Random.Range(i, inputList.Length);
			inputList[i] = inputList[rand];
			inputList[rand] = temp;
		}
	}
}

