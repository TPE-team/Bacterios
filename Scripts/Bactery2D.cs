using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bactery2D : MonoBehaviour {
	public float divisionFrequency = 0.1f;
	public BacteryState bacteryState = BacteryState.normal;

	public GameObject Divide(Transform colony, GameObject animation) {
		GameObject newBactery = GameObject.Instantiate (gameObject, colony);
		animation = GameObject.Instantiate (animation, transform.position, new Quaternion(), colony);
		//Add forces for popping
		float x = Random.Range (0f, 0.001f);
		float y = Random.Range (0f, 0.001f);
		float z = Random.Range (0f, 0.001f);
		Vector3 direction = new Vector3 (x, y, z);
		Rigidbody2D newBacteryRigidBody = newBactery.GetComponent<Rigidbody2D> ();
		Rigidbody2D rigidBody = GetComponent<Rigidbody2D> ();
		rigidBody.AddForce (direction);
		newBacteryRigidBody.AddForce (-direction);


		return newBactery;
	}

	public void TestAndDivide(float number, BacteryColony2D colony, GameObject animation) {
		Transform colonyTransform = colony.GetComponent<Transform> ();
		if (number <= divisionFrequency / 60f) {
			GameObject newBactery = Divide (colonyTransform, animation);
			colony.Add (newBactery);
			UIController ui = FindObjectOfType<UIController> ();
			ui.UpdateBacteryCount ();
		}
	}

	public void StickToWall() {
		bacteryState = BacteryState.sticking;
		UIController ui = FindObjectOfType<UIController> ();
		ui.UpdateBacteryCount ();
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 10f, 1)) {
			StartCoroutine (SendTo (hit.point));
		}

	}

	public IEnumerator SendTo(Vector3 destination) {
		Rigidbody2D rigidBody = GetComponent<Rigidbody2D> ();
		while (Vector3.Distance (transform.position, destination) >= 0.1f) {
			Vector3 direction = destination - transform.position;
			rigidBody.AddForce (direction);
			yield return null;
		}
		rigidBody.isKinematic = true;
		bacteryState = BacteryState.sticked;
	}
	
}
