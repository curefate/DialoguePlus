using UnityEngine;
using TMPro;
using DialoguePlus.Core;
public class DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI ChatText;
    public TextMeshProUGUI NameText;

    private List<char> _upcomingChars = new();

    public void OnDialogue(Runtime runtime, SIR_Dialogue dialogue)
    {
        NameText.text = dialogue.Speaker;
        ChatText.text = "";
        _upcomingChars.Clear();
        foreach (var c in (string)dialogue.Text.Evaluate(runtime))
        {
            _upcomingChars.Add(c);
        }
        runtime.StartCoroutine(TypeText());
    }
}
