using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public Text _text;

    public void SetText(string Text)
    {
        _text.text = Text;
    }
}
