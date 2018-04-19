using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bactery2D : MonoBehaviour {
	public Color normalColor;
	public Color stickedColor;
	public Color spawnColor;
	public float divisionFrequency = 0.1f;
	public BacteryState bacteryState = BacteryState.normal;
	float zVelocity = 0f;

	private Renderer _renderer;
	private MaterialPropertyBlock _propBlock;

	void Awake() {
		_propBlock = new MaterialPropertyBlock ();
		_renderer = GetComponent<Renderer> ();
		float duration = 0.5f;
		StartCoroutine (FadeFromToColor (spawnColor, normalColor, duration));
	}

	IEnumerator FadeToColor(Color color, float duration) {
		_renderer.GetPropertyBlock (_propBlock);
		Color begin = _propBlock.GetColor ("_EmissionColor");
		float t = 0;
		while (ColorDifference(_propBlock.GetColor("_EmissionColor"), color) >= 0.01f) {
			t += Time.deltaTime;
			_propBlock.SetColor ("_EmissionColor", Color.Lerp (begin, color, t * duration));
			_renderer.SetPropertyBlock (_propBlock);
			yield return null;
		}
	}

	IEnumerator FadeFromToColor(Color begin, Color end, float duration) {
		_renderer.GetPropertyBlock(_propBlock);
		_propBlock.SetColor("_EmissionColor", begin);
		_renderer.SetPropertyBlock(_propBlock);
		StartCoroutine (FadeToColor (end, duration));
		yield return null;
	}

	float ColorDifference(Color color1, Color color2) {
		float h1, h2, s1, s2, v1, v2;
		Color.RGBToHSV (color1, out h1, out s1, out v1);
		Color.RGBToHSV (color2, out h2, out s2, out v2);
		float distanceSquared = Mathf.Pow((h1 - h2), 2f) + Mathf.Pow((v1 - v2), 2f) + Mathf.Pow((s1 - s2), 2f);
		return Mathf.Sqrt (distanceSquared);
	}

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
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 10f, 1)) {
			bacteryState = BacteryState.sticking;
			gameObject.layer = 11;
			UIController ui = FindObjectOfType<UIController> ();
			ui.UpdateBacteryCount ();
			Debug.DrawRay (hit.point, hit.normal, Color.black);
			StartCoroutine (SendTo (hit.point));
		}

	}

	IEnumerator SendTo(Vector3 destination) {
		Rigidbody2D rigidBody = GetComponent<Rigidbody2D> ();
		rigidBody.drag *= 2f;
		float d1 = Vector3.Distance (transform.position, destination);
		float t = 0f;
		while ((Vector3.Distance (transform.position, destination) >= 0.05f) && t < 20f) {
			Vector3 direction = destination - transform.position;
			rigidBody.AddForce (direction);
			zVelocity += direction.z;
			zVelocity /= 30f;
			Vector3 newPos = transform.position + new Vector3 (0f, 0f, zVelocity);
			transform.position = newPos;
			float d = Vector3.Distance (transform.position, destination);
			_renderer.GetPropertyBlock (_propBlock);
			_propBlock.SetColor ("_EmissionColor", Color.Lerp (stickedColor, normalColor, d / d1));
			_renderer.SetPropertyBlock (_propBlock);
			t += Time.deltaTime;
			yield return null;
		}
		rigidBody.bodyType = RigidbodyType2D.Static;
		rigidBody.isKinematic = true;
		//transform.position = destination;
		bacteryState = BacteryState.sticked;
	}
	
}
