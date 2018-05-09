using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BacteryProperties {
	[Range(0f, 1f)]
	public float divisionFrequency;
	public BacteryState bacteryState;
	public Color[] colors;


	public BacteryProperties (Color normal) {
		divisionFrequency = 0.1f;
		bacteryState = BacteryState.normal;
		colors = new Color[4] { normal, normal, normal, normal };
	}

	public BacteryProperties (float df, BacteryState bs, Color[] _colors) {
		divisionFrequency = df;
		bacteryState = bs;
		colors = _colors;
	}


}

public static class BacteryPropertiesExtension {
	public static BacteryProperties Clone(this BacteryProperties props) {
		return new BacteryProperties (props.divisionFrequency, props.bacteryState, props.colors);
	}
}
