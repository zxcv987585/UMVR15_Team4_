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
    //pาสmlจY๔Y๕ฎ์
    public event Action LastTakeAction;

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
            if (Index == Lines.Length - 1)
            {
                LastTakeAction?.Invoke();
            }
        }

        yield return new WaitForSeconds(4f);
        Take1Finish?.Invoke();
        StartCoroutine(CloseUIAnimation());
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
    private IEnumerator CloseUIAnimation()
    {
        gameObject.GetComponent<DialogueTake>().openAction.Invoke();
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
