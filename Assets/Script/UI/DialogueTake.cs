using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class DialogueTake : MonoBehaviour
{
    //ืถโxฎแ`
    public UnityEvent openAction;
    //ืถ่u
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public float TextSpeed;
    public float WaitForNextLine;
    //pาSๆืถๅIint
    private int Index;
    //pาสmlจโxฝoปๆ๎มธIฯh
    public event Action Take1Finish;

    void Start()
    {
        TextComponent.text = string.Empty;

        StartCoroutine(UIAnimation());
        StartCoroutine(DisplayDialogue());
    }

    IEnumerator DisplayDialogue()
    {
        yield return new WaitForSeconds(3.5f);
        for (Index = 0; Index < Lines.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines[Index]));

            if (Index < Lines.Length - 1) 
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
        }

        yield return new WaitForSeconds(7f);
        Take1Finish?.Invoke();
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

    private IEnumerator UIAnimation()
    {
        yield return new WaitForSeconds(2.1f);
        gameObject.GetComponent<DialogueTake>().openAction.Invoke();
    }
}
