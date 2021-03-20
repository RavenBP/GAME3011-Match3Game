using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer 
{
	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label)
	{
		EditorGUI.PrefixLabel(position,label);
		Rect newposition = position;
		newposition.y += 20f;
		SerializedProperty data = property.FindPropertyRelative("rows");

        if (data.arraySize != 20)
        {
            data.arraySize = 20;
        }

		for(int i = 0; i < 20; i++)
		{
			SerializedProperty row = data.GetArrayElementAtIndex(i).FindPropertyRelative("row");
			newposition.height = 20f;

			if (row.arraySize != 10)
            {
				row.arraySize = 10;
            }

			newposition.width = position.width/10;

			for (int j = 0; j < 10; j++)
			{
				EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(j),GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += 20f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label)
	{
		return 20f * 21;
	}
}
