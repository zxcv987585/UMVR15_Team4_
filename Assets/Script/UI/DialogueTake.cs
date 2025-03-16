using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueTake : MonoBehaviour
{
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public float TextSpeed;
    public float WaitForNextLine;

    private int Index;

    void Start()
    {
        TextComponent.text = string.Empty;

        StartCoroutine(DisplayDialogue());
    }

    IEnumerator DisplayDialogue()
    {
        for(Index = 0; Index < Lines.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines[Index]));

            if (Index < Lines.Length - 1) 
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
        }

        yield return new WaitForSeconds(7f);
        gameObject.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        foreach(char c in line.ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }
}
