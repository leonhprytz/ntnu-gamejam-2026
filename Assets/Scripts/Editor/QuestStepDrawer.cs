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

        Rect rect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );
        EditorGUI.PropertyField(rect, kind, label);

        EditorGUI.indentLevel++;

        foreach (SerializedProperty body in BodiesOf(property, kind))
        {
            rect = new Rect(
                position.x,
                rect.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUI.GetPropertyHeight(body, true)
            );

            EditorGUI.PropertyField(rect, body, true);
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty kind = property.FindPropertyRelative("kind");
        float height = EditorGUIUtility.singleLineHeight;

        foreach (SerializedProperty body in BodiesOf(property, kind))
        {
            height +=
                EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(body, true);
        }

        return height;
    }

    // A dialogue step also offers branching, so it draws two fields; a logic
    // step is still just its object slot.
    private static SerializedProperty[] BodiesOf(
        SerializedProperty property,
        SerializedProperty kind
    )
    {
        if (kind.enumValueIndex == (int)QuestStepKind.Dialogue)
        {
            return new[]
            {
                property.FindPropertyRelative("nextState"),
                property.FindPropertyRelative("auto"),
                property.FindPropertyRelative("lines"),
                property.FindPropertyRelative("branches"),
            };
        }

        return new[]
        {
            property.FindPropertyRelative("nextState"),
            property.FindPropertyRelative("auto"),
            property.FindPropertyRelative("logic"),
        };
    }
}
