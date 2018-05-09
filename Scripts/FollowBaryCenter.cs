using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBaryCenter : MonoBehaviour {
	public Transform baryCenter;
	[Range(0f, 1f)]
	public float bounds = 0.5f;
	Camera camera;
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
	}
	// Update is called once per frame
	void FixedUpdate () {
		if (IsPointOutOfBounds(camera.WorldToScreenPoint (baryCenter.position))) {
			Vector3 direction = baryCenter.position - transform.position;
			direction.z = 0f;
			transform.position += direction / 100f;
		}
	}
	bool IsPointOutOfBounds(Vector3 point) {
		Vector3 center = new Vector3 (camera.pixelWidth / 2, camera.pixelHeight / 2);
		Vector3 length = point - center;
		if (Mathf.Abs(length.x) > bounds*camera.pixelWidth / 2 || Mathf.Abs(length.y) > bounds*camera.pixelHeight / 2) {
			return true;
		}
		return false;
	}
			
}
