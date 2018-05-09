using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(BacteryProperties))]
[CanEditMultipleObjects]
public class BacteryPropertyDrawer : Editor {
	public override void OnInspectorGUI ()
	{
		serializedObject.ApplyModifiedProperties();
		base.OnInspectorGUI();
	}
}


[CustomPropertyDrawer(typeof(BacteryProperties))]
[CanEditMultipleObjects]
public class BacteryPropertiesDrawer : PropertyDrawer{
	const int colorWidth = 30;
	const int colorHeight = 18;
	const int sliderHeight = 20;
	const int checkboxHeight = 20;

	private bool isPrefabchild = false;

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		if (isPrefabchild) {
			return base.GetPropertyHeight (property, label) + 2 * sliderHeight + checkboxHeight;
		} else {
			return base.GetPropertyHeight (property, label) + 2 * sliderHeight + checkboxHeight + (property.FindPropertyRelative ("colors").arraySize+2) * colorHeight;
		}
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		//make it work on prefabs
		EditorGUI.BeginProperty (position, label, property);

		//draw the property label
		EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

		//don't make the child's fiels be indented
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;
		EditorGUI.PropertyField (new Rect(position.x, position.y + sliderHeight, position.width, sliderHeight), property.FindPropertyRelative ("divisionFrequency"), new GUIContent("Division Frequency"));
		EditorGUI.PropertyField (new Rect(position.x, position.y + 2 * sliderHeight, position.width, sliderHeight), property.FindPropertyRelative ("bacteryState"), new GUIContent("State"));
		isPrefabchild = EditorGUI.Toggle (new Rect(position.x, position.y + 3 * sliderHeight, position.width, checkboxHeight), new GUIContent("Is Prefab Child", "Only check if you need to reset the normalColor"), isPrefabchild);
		if (!isPrefabchild) {
			//EditorGUI.ColorField (new Rect (position.x + 50, position.y + sliderHeight + colorHeight + checkboxHeight, colorWidth, colorHeight), new GUIContent("Normal color", "Color for standart bactery (may change if conjugating)"), normalColor, true, true, false, new ColorPickerHDRConfig(0f, 10f, 0f, 10f));
			EditorGUI.PropertyField (
				new Rect (position.x, position.y + 3 * sliderHeight + checkboxHeight, position.width, colorHeight),
				property.FindPropertyRelative ("colors"),
				new GUIContent ("Array of colors"), true
			);
			//normalColor = EditorGUI.ColorField (
			//	new Rect (position.x, position.y + 3 * sliderHeight + checkboxHeight, position.width, colorHeight),
			//	normalColor
			//);

		}
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty ();
	}
}
