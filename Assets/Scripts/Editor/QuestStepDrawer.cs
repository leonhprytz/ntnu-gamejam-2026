using UnityEditor;
using UnityEngine;

// Shows only the field the step's kind actually uses, so a new step is a kind
// dropdown plus either a text box or an object slot, never both.
[CustomPropertyDrawer(typeof(QuestStep))]
public class QuestStepDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty kind = property.FindPropertyRelative("kind");
        SerializedProperty body = BodyOf(property, kind);

        Rect kindRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(kindRect, kind, label);

        Rect bodyRect = new Rect(
            position.x,
            kindRect.yMax + EditorGUIUtility.standardVerticalSpacing,
            position.width,
            EditorGUI.GetPropertyHeight(body, true));

        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(bodyRect, body, true);
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty kind = property.FindPropertyRelative("kind");

        return EditorGUIUtility.singleLineHeight
            + EditorGUIUtility.standardVerticalSpacing
            + EditorGUI.GetPropertyHeight(BodyOf(property, kind), true);
    }

    private static SerializedProperty BodyOf(SerializedProperty property, SerializedProperty kind)
    {
        return kind.enumValueIndex == (int)QuestStepKind.Dialogue
            ? property.FindPropertyRelative("lines")
            : property.FindPropertyRelative("logic");
    }
}
