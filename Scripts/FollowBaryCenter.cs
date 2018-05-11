using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBaryCenter : MonoBehaviour {
	public Transform baryCenter;
	[Range(0f, 1f)]
	public float bounds = 0.5f;
	public float speed = 1;
	Vector3 center;
	void Start() {
		center =  new Vector3 (Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
	}
	// Use this for initialization
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 pixelBaryCenterPos = Camera.main.WorldToScreenPoint (baryCenter.position);
		if (IsPointOutOfBounds(pixelBaryCenterPos)) {
			Vector3 direction = baryCenter.position - transform.position;
			direction.z = 0f;
			transform.position += direction * (speed / 100f);
		}
	}
	bool IsPointOutOfBounds(Vector3 point) {
		Vector3 length = point - center;
		if (Mathf.Abs(length.x) > bounds*Camera.main.pixelWidth / 2 || Mathf.Abs(length.y) > bounds*Camera.main.pixelHeight / 2) {
			return true;
		}
		return false;
	}

	bool IsPointOutOfUpBounds(Vector3 point) {
		Vector3 dir = point - center;
		if (dir.y > bounds * Camera.main.pixelHeight / 2) {
			return true;
		}
		return false;
	}

	bool IsPointOutOfDownBounds(Vector3 point) {
		Vector3 dir = point - center;
		if (-dir.y > bounds * Camera.main.pixelHeight / 2) {
			return true;
		}
		return false;
	}



			
}
