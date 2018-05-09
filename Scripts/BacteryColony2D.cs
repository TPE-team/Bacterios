using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteryColony2D : MonoBehaviour {
	public GameObject bactery_prefab;
	List<GameObject> bacteries;
	List<BacteryProperties> bacteriesPropertiesList;
	Transform baryCenter;
	public int initialCount;
	public int maxBacteryCount = 200;
	public GameObject bacterySpawnPrefab;
	public List<GameObject> Bacteries {
		get {
			return bacteries;
		}
	}
	public Vector3 BaryCenter {
		get {
			return baryCenter.position;
		}
	}
	public int Count {
		get {
			return bacteries.Count;
		}
	}
	public int ActiveCount{
		get {
			return GetActiveBacteries ().Length;
		}
	}

	public float distanceToCam = 1f;

	public void Add(GameObject bactery) {
		bacteries.Add (bactery);
	}

	void Start() {
		bacteries = new List<GameObject>();
		for (int i = 0; i < initialCount; i++) {
			GameObject bactery_object = GameObject.Instantiate (bactery_prefab, transform);
			bacteries.Add (bactery_object);
		}
		baryCenter = GameObject.Find ("Barycenter").transform;
		//bacteries [0].transform.Translate (0.1f, 0.1f, 0.1f);
	}

	void Update() {
		if (Input.GetMouseButtonDown(0) && ActiveCount > 1) {
			Bactery2D[] activeBacteries = GetActiveBacteries ();
			Bactery2D bactery = activeBacteries [Random.Range (0, activeBacteries.Length)];
			bactery.StickToWall ();
		}
	}

	void FixedUpdate() {
		//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 newBaryCenter = new Vector3 ();

		for (int i = 0; i < bacteries.Count; i++) {
			Bactery2D b = bacteries [i].GetComponent<Bactery2D> ();
			if (b.props.bacteryState == BacteryState.normal) {
				newBaryCenter += bacteries [i].transform.position;
				//RaycastHit insideHit;
				//if (Physics.Raycast (ray, out insideHit, 10f, insideMask)) {
				//	Debug.DrawRay (insideHit.point, insideHit.normal, Color.white);
				//	RaycastHit outsideHit;
				//	if (Physics.Raycast (ray, out outsideHit, 10f, outsideMask)) {
				//		Debug.DrawRay (outsideHit.point, outsideHit.normal, Color.yellow);
				//		Vector3 wanted_position = (insideHit.point + outsideHit.point) / 2;
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = distanceToCam;
				mousePos = Camera.main.ScreenToWorldPoint (mousePos);
				Vector3 direction = mousePos - bacteries [i].transform.position;
				Rigidbody2D bacteryRigidBody = bacteries [i].GetComponent<Rigidbody2D> ();
				bacteryRigidBody.AddForce (direction);
				if (ActiveCount < maxBacteryCount) {
					Bactery2D bactery = bacteries [i].GetComponent<Bactery2D> ();
					bactery.TestAndDivide (Random.Range (0f, 1f), this, bacterySpawnPrefab);
				}
			}
		}
		baryCenter.position = newBaryCenter / ActiveCount;
		//Quaternion rotation = baryCenter.rotation;
		//rotation.y += Input.GetAxis ("Vertical");
		//baryCenter.rotation = rotation;
		//int mask = 1 << 10;
		//mask = ~mask;
		//RaycastHit insideHitBary;
		//if (Physics.Raycast (baryCenter.position, Camera.main.transform.forward, out insideHitBary, 100f, mask)) {
			//Debug.DrawRay (insideHit.point, insideHit.normal, Color.white);
		//	RaycastHit outsideHitBary;
		//		if (Physics.Raycast (baryCenter.position, Camera.main.transform.forward, out outsideHitBary, 100f, mask)) {
				//Debug.DrawRay (outsideHit.point, outsideHit.normal, Color.yellow);
		//		Vector3 medianPoint = (insideHitBary.point + outsideHitBary.point) / 2;
		//		Vector3 newBaryCenterPos = new Vector3 (baryCenter.position.x, medianPoint.y, medianPoint.z);
		//		baryCenter.position = newBaryCenterPos;
		//	}
		//}

	}

	public Bactery2D[] GetActiveBacteries() {
		List<Bactery2D> activeBacteries = new List<Bactery2D> ();
		for (int i = 0; i < bacteries.Count; i++) {
			Bactery2D b = bacteries [i].GetComponent<Bactery2D> ();
			if (b.props.bacteryState == BacteryState.normal) {
				activeBacteries.Add (b);
			}
		}
		return activeBacteries.ToArray ();

	}

	public void Conjugate(int index, Vector3 targetPosition, BacteryProperties newBacteryProps) {
		Bactery2D b = bacteries [index].GetComponent<Bactery2D> ();
		b.StartConjugate (targetPosition, newBacteryProps);
	}

	public float GetAverageLengthExcept() {
		float distanceSum = 0;
		for (int i = 0; i < bacteries.Count; i++) {
			Bactery2D b = bacteries [i].GetComponent<Bactery2D> ();
			if (b.props.bacteryState == BacteryState.normal) {
				distanceSum += Vector3.Distance (baryCenter.position, bacteries [i].transform.position);
			}
		}
		return distanceSum / ActiveCount;
	}
}
