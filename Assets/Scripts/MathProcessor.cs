using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathProcessor : MonoBehaviour
{
	public LayerMask HumanMask;
	public LayerMask DoorMask;
	public LayerMask WallMask;
	public LayerMask SammelplatzMask;

	private StateHandler State;

	public void Awake()
	{
		//Find StateHandler in the scene
		State = GameObject.FindObjectOfType<StateHandler>();
	}
	public Vector2 getMainForce(GameObject entity)
	{
		List<Vector2> forces = new List<Vector2>();

			Vector2 desiredDirection = Vector2.zero;

		// Observe Sourroundings
		// get movement controller from entity
		if (entity.GetComponent<movementController>().doorReached == false)
		{
			// get vector to nearest door
			desiredDirection = getVecToNearestCollider(entity, Physics2D.OverlapCircleAll(entity.transform.position, 100f, DoorMask));
		} else {
			// get vector to nearest sammelplatz
			desiredDirection = getVecToNearestCollider(entity, Physics2D.OverlapCircleAll(entity.transform.position, 100f, SammelplatzMask));
		}
		
		Vector2 e_i0 = desiredDirection.normalized;
		float v_i0 = entity.GetComponent<movementController>().desiredSpeed;
		Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
		float tau_i = State.slider1.value;
		float m = entity.GetComponent<Rigidbody2D>().mass;

		Vector2 xi_i = 1.5f * getRndDir(desiredDirection.normalized, 90.0f);
		// xi_i = Vector2.zero;
		Vector2 res = (((e_i0 * v_i0 + xi_i) - v_i) / tau_i) * m;

		return res;
	}

	public Vector2 getWallForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get radius of human
		float r_i = entity.transform.localScale.x;

		// find nearest colliders in the wall layer
		Collider2D[] waendeInRange = Physics2D.OverlapCircleAll(entity.transform.position, r_i * 5, WallMask);

		// if no colliders are found return zero
		if (waendeInRange.Length == 0) return Vector2.zero;

		// iterate thru colliders
		foreach (Collider2D wand in waendeInRange) {
			Vector2 entityPosition2D = entity.transform.position;
			Vector2 nearestPointOnColldier = wand.ClosestPoint(entityPosition2D);

			float d_ib = Vector2.Distance(entityPosition2D, nearestPointOnColldier);
			Vector2 n_ib = (entityPosition2D - nearestPointOnColldier).normalized;
			Vector2 t_ib = new Vector2(-n_ib.y, n_ib.x);
			Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
			float A_i = State.slider2.value;
			float B_i = State.slider3.value;

			Vector2 res = (Mathf.Pow(A_i, (r_i - d_ib) / B_i) + theta(r_i - d_ib)) * n_ib - theta(r_i - d_ib) * (v_i * t_ib) * t_ib;
			// Debug.Log(res);
			forces.Add(res * 35);
		}

		// sum all forces
		Vector2 sum = Vector2.zero;
		foreach (Vector2 force in forces) {
			sum += force;
		}

		return sum;
	}

	public Vector2 getSozForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get radius of human
		float r_ij = entity.transform.localScale.x * 2;

		// find nearest colliders in the wall layer
		Collider2D[] humansInRange = Physics2D.OverlapCircleAll(entity.transform.position, r_ij * 2, HumanMask);

		// if no colliders are found return zero
		if (humansInRange.Length == 0) return Vector2.zero;

		// iterate thru humans
		foreach (Collider2D human in humansInRange) {
			Vector2 entityPosition2D = entity.transform.position;
			Vector2 humanPosition2D = human.transform.position;

			float d_ij = Vector2.Distance(entityPosition2D, humanPosition2D);
			Vector2 n_ij = (entityPosition2D - humanPosition2D).normalized;
			Vector2 v_i = entity.GetComponent<Rigidbody2D>().velocity;
			Vector2 e_i = v_i.normalized;
			float lamda_i = State.slider4.value;
			float A_i = State.slider2.value;
			float B_i = State.slider3.value;

			Vector2 res = (Mathf.Pow(A_i, (r_ij - d_ij) / B_i)) * n_ij * (lamda_i + (1 - lamda_i) * ( (1 + Mathf.Cos( Vector2.Angle(e_i, -n_ij) ) ) / 2));
			forces.Add(res);
		}

		// sum all forces
		Vector2 sum = Vector2.zero;
		foreach (Vector2 force in forces) {
			sum += force;
		}

		return sum;
	}

	public Vector2 getPanicForces(GameObject entity) {
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get radius of human
		float r_ij = entity.transform.localScale.x * 2;

		// find nearest colliders in the wall layer
		Collider2D[] humansInRange = Physics2D.OverlapCircleAll(entity.transform.position, r_ij, HumanMask);

		// if no colliders are found return zero
		if (humansInRange.Length == 0) return Vector2.zero;

		// iterate thru humans
		foreach (Collider2D human in humansInRange) {
			Vector2 entityPosition2D = entity.transform.position;
			Vector2 humanPosition2D = human.transform.position;

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

			Vector2 res = theta(r_ij - d_ij) * n_ij + theta(r_ij - d_ij) * delta_vt * t_ij;
			forces.Add(res);
		}

		// sum all forces
		Vector2 sum = Vector2.zero;
		foreach (Vector2 force in forces) {
			sum += force;
		}

		return sum;
	}

	public Vector2 getRndDir(Vector2 dir, float angle)
	{
		Vector2 res = dir;
		float rnd = Random.Range(-angle, angle);
		res = Quaternion.AngleAxis(rnd, Vector3.forward) * res;
		return res;
	}

	public float theta(float x) {
		if (x >= 0) return x;
		return 0.0f;
	}

	public Vector2 getVecToNearestCollider(GameObject entity, Collider2D[] colliders)
	{
		Shuffle(colliders);
		Collider2D nearestCollider = colliders[0];
		float nearestDistance = (nearestCollider.transform.position - entity.transform.position).magnitude;

		foreach (Collider2D collider in colliders)
		{
			float distance = (collider.transform.position - entity.transform.position).magnitude;
			if (distance < nearestDistance)
			{
				nearestCollider = collider;
				nearestDistance = distance;
			}
		}

		return nearestCollider.transform.position - entity.transform.position;
	}

	private void Shuffle<T>(T[] inputList)
    {
        for (int i = 0; i < inputList.Length - 1; i++)
        {
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Length);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }
	public List<Vector2> getForces(GameObject entity)
	{
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// get doorForce and add them to the list
		forces.Add(getMainForce(entity));

		// get wallForce and add them to the list
		forces.Add(getWallForces(entity));

		// get sozForce and add them to the list
		forces.Add(getSozForces(entity));
		
		if (State.toggle1.isOn == true) {
			// get panicForces and add them to the list
			forces.Add(getPanicForces(entity));
		} 

		return forces;
	}

	public List<Vector2> gravityTest(GameObject entity)
	{
		// create list of Vector2s to store all the forces
		List<Vector2> forces = new List<Vector2>();

		// Observe Sourroundings
		// get all the colliders in the radius
		Collider2D[] colliders = Physics2D.OverlapCircleAll(entity.transform.position, 2.0f, HumanMask);

		// Calc Forces
		foreach (var collider in colliders)
		{
			// ignore self
			if (collider.gameObject == gameObject)
				continue;

			// add new force to the list of forces resembling the direction of the collider
			// multiply it by the size of the collider
			forces.Add((collider.transform.position - entity.transform.position).normalized * collider.bounds.size.magnitude * State.slider1.value);

			// get gameobject of colllider and log name
			// Debug.Log(collider.gameObject.name);
		}
		return forces;
	}
}

