using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConjugateBactery : MonoBehaviour {
	public BacteryColony2D bacteryColony;
	public BacteryProperties newBacteryProps;

	void OnMouseDown() {
		int index = FindClosestBacteryIndex (bacteryColony);
		Vector3 direction = bacteryColony.Bacteries [index].transform.position - transform.position;
		Vector3 scaledDirection = direction.normalized / 10f;
		Vector3 targetPosition = transform.position + scaledDirection;
		bacteryColony.Conjugate (index, targetPosition, newBacteryProps);
	}
	
	int FindClosestBacteryIndex(BacteryColony2D colony) {
		float minDistance = float.MaxValue;
		int index = -1;
		for (int i = 0; i < colony.ActiveCount; i++) {
			Bactery2D b = colony.Bacteries [i].GetComponent<Bactery2D> ();
			float d = Vector3.Distance (colony.GetActiveBacteries() [i].transform.position, transform.position);
			if (d < minDistance && b.props.bacteryState == BacteryState.normal) {
				minDistance = d;
				index = i;
			}
		}
		return index;
	}
}
