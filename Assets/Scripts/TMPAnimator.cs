using UnityEngine;
using TMPro;

public class TextFromAnimator : MonoBehaviour
{
    public TMP_Text tmpText;

    public void SetText(string message)
    {
        message = message.Replace("\\n", "\n");
        tmpText.text = message;
    }
}
