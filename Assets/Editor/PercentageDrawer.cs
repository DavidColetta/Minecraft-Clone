using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Percentage))]
public class PercentageDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty max = property.FindPropertyRelative("_max");
        SerializedProperty val = property.FindPropertyRelative("val");

        // Draw label
        Rect barPos = position;
        barPos.Set(barPos.x-5,barPos.y,75,barPos.height);
        float percent;
        if (max.intValue == 0) percent = 1; else percent = (float)val.intValue/max.intValue;
        EditorGUI.ProgressBar(barPos,percent,"");
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        position.Set(position.x-48,position.y,30,position.height);
        EditorGUI.LabelField(position,"Max: ");
        position.Set(position.x+30,position.y,50,position.height);
        max.intValue = Mathf.Max(EditorGUI.DelayedIntField(position,max.intValue), 1);

        position.Set(position.x+50,position.y,30,position.height);
        EditorGUI.LabelField(position,"Val: ");
        position.Set(position.x+25,position.y,EditorGUIUtility.currentViewWidth-position.x-44f,position.height);
        val.intValue = EditorGUI.DelayedIntField(position,val.intValue);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
