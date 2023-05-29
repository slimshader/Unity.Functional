using Bravasoft.Functional;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(Option<string>), true)]
public class OptionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var isSomeProperty = property.FindPropertyRelative("IsSome");
        var valueProperty = property.FindPropertyRelative("_value");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var checkRect = new Rect(position.x, position.y, 20, position.height);
        var valueRect = new Rect(position.x + 25, position.y, position.width - 25, position.height);

        isSomeProperty.boolValue = EditorGUI.Toggle(checkRect, isSomeProperty.boolValue);
        if (isSomeProperty.boolValue)
        {
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}
