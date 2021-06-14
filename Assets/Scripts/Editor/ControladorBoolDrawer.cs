using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ControladorBoolAttribute))]
public class ControladorBoolDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ControladorBoolAttribute boolAtt = (ControladorBoolAttribute)attribute;
        bool enumVal = GetBoolValue(boolAtt, property);

        bool guiState = GUI.enabled;
        GUI.enabled = boolAtt.Valor == enumVal;
        if (GUI.enabled)
            EditorGUI.PropertyField(position, property, label, true);

        GUI.enabled = guiState;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ControladorBoolAttribute boolAtt = (ControladorBoolAttribute)attribute;
        bool boolVal = GetBoolValue(boolAtt, property);

        if (boolAtt.Valor == boolVal)
            return EditorGUI.GetPropertyHeight(property, label);
        else
            return -EditorGUIUtility.standardVerticalSpacing;
    }

    private bool GetBoolValue(ControladorBoolAttribute boolAtt, SerializedProperty property)
    {
        bool boolVal = false;
        SerializedProperty boolProperty = null;
        if (!property.isArray)
        {
            string path = property.propertyPath;
            path = path.Replace(property.name, boolAtt.Campo);
            boolProperty = property.serializedObject.FindProperty(path);
        }

        if (boolProperty != null)
            boolVal = boolProperty.boolValue;

        return boolVal;
    }
}
