using UnityEngine;
using TMPro;

// Put this on a TextMeshPro UI text and it shows itself whenever something is
// in interact range.
public class InteractionPrompt : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string message = "Interact (Space)";

    void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        textComponent.text = message;
    }

    void Update()
    {
        textComponent.enabled = InteractionManager.instance.availableInteractions.Count > 0;
    }
}
