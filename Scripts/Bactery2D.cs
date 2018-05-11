using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bactery2D : MonoBehaviour {
	public Color spawnColor;
	public BacteryProperties props;
	float zVelocity = 0f;

	private Renderer _renderer;
	private MaterialPropertyBlock _propBlock;

	void Awake() {
		_propBlock = new MaterialPropertyBlock ();
		_renderer = GetComponent<Renderer> ();
		float duration = 0.5f;
		StartCoroutine (FadeFromToColor (spawnColor, props.colors[0], duration));
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
		if (number <= props.divisionFrequency / 60f) {
			GameObject newBactery = Divide (colonyTransform, animation);
			Bactery2D newB = newBactery.GetComponent<Bactery2D> ();
			newB.props = props.Clone();
			newB.UpdateProps ();
			colony.Add (newBactery);
			UpdateBacteryCount ();
		}
	}

	public void StickToWall() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 10f, (1 << 8) | (1 << 12))) {
			if (hit.collider.gameObject.layer == 8) {
				props.bacteryState = BacteryState.moving;
				gameObject.layer = 11;
				UpdateBacteryCount ();
				Debug.DrawRay (hit.point, hit.normal, Color.black);
				BacteryProperties targetProperties = props.Clone ();
				targetProperties.bacteryState = BacteryState.sticked;
				StartCoroutine (SendTo (hit.point, targetProperties));
			}
		}

	}

	IEnumerator SendTo(Vector3 destination, BacteryProperties targetProps) {
		Rigidbody2D rigidBody = GetComponent<Rigidbody2D> ();
		rigidBody.drag *= 2f;
		Color final = props.colors [(int)targetProps.bacteryState];
		Color initial = props.colors [(int)props.bacteryState];
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
			_propBlock.SetColor ("_EmissionColor", Color.Lerp (final, initial, d / d1));
			_renderer.SetPropertyBlock (_propBlock);
			t += Time.deltaTime;
			yield return null;
		}
		rigidBody.bodyType = RigidbodyType2D.Static;
		rigidBody.isKinematic = true;
		props = targetProps.Clone();
	}

	public void StartConjugate(Vector3 position, BacteryProperties targetProperties) {
		gameObject.layer = 11;
		BacteryProperties newProps = targetProperties.Clone();
		props.bacteryState = BacteryState.moving;
		StartCoroutine (SendTo (position, targetProperties));
		props.bacteryState = BacteryState.conjugating;
		Invoke("ConjugateBack", 3f);
	}

	void ConjugateBack() {
		BacteryColony2D bC = GetComponentInParent<BacteryColony2D> ();
		float averageLength = bC.GetAverageLengthExcept ();
		// times 2 because in the case of a perfect circle, 
		//the average distance to center is half of the radius (at least i think)
		StartCoroutine (SendToDistance (averageLength * 2, BacteryState.normal));
	}

	public IEnumerator SendToDistance(float distance, BacteryState bacteryState) {
		StopCoroutine ("SendTo");
		Rigidbody2D rigidBody = GetComponent<Rigidbody2D> ();
		rigidBody.bodyType = RigidbodyType2D.Dynamic;
		rigidBody.isKinematic = false;
		Color initial = props.colors [(int)bacteryState];
		Color final = props.colors [(int)props.bacteryState];
		zVelocity = 0f;
		BacteryColony2D bC = GetComponentInParent<BacteryColony2D> ();
		Vector3 destination = distance * Vector3.Normalize(bC.BaryCenter - transform.position) + bC.BaryCenter;
		float d1 = Vector3.Distance (transform.position, destination);
		float t = 0;
		while (Vector3.Distance (bC.BaryCenter, transform.position) >= distance) {
			t += Time.deltaTime;
			destination = distance * Vector3.Normalize(bC.BaryCenter - transform.position) + bC.BaryCenter;
			Vector3 direction = destination - transform.position;
			rigidBody.AddForce (direction);
			zVelocity += direction.z;
			zVelocity /= 30f;
			Vector3 newPos = transform.position + new Vector3 (0f, 0f, zVelocity);
			transform.position = newPos;
			float d = Vector3.Distance (transform.position, destination);
			_renderer.GetPropertyBlock (_propBlock);
			_propBlock.SetColor ("_EmissionColor", Color.Lerp (initial, final, d / d1));
			_renderer.SetPropertyBlock (_propBlock);
			t += Time.deltaTime;
			yield return null;
		}
		rigidBody.drag /= 2f;
		rigidBody.isKinematic = false;
		gameObject.layer = 10;
		props.bacteryState = bacteryState;
		UpdateProps ();
	}

	void UpdateBacteryCount() {
		UIController ui = FindObjectOfType<UIController> ();
		ui.UpdateBacteryCount ();
	}

	public void UpdateProps() {
		//setting color;
		_renderer.GetPropertyBlock (_propBlock);
		_propBlock.SetColor ("_EmissionColor", props.colors [(int)props.bacteryState]);
		_renderer.SetPropertyBlock (_propBlock);
	}

}
