using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializedOption<>))]
public class SerializedOptionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var hasValueProperty = property.FindPropertyRelative("_hasValue");
        var valueProperty = property.FindPropertyRelative("_value");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var checkRect = new Rect(position.x, position.y, 20, position.height);
        var valueRect = new Rect(position.x + 25, position.y, position.width - 25, position.height);

        hasValueProperty.boolValue = EditorGUI.Toggle(checkRect, hasValueProperty.boolValue);
        if (hasValueProperty.boolValue)
        {
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}
