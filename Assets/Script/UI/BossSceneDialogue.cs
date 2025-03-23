using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class BossSceneDialogue : MonoBehaviour
{
    //µ¯µ°∞ µe
    public UnityEvent openAction;
    //•Œ®”Ω’æ„Value≠»
    public CanvasGroup canvasGroup;
    //¿x¶s§Â¶r¨€√ˆ§∫Æe
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public string[] Lines2;
    public string[] Lines3;
    public float TextSpeed;
    public float WaitForNextLine;
    //∞Oø˝§Â¶r∂i´◊©“ª›Index
    private int Index;
    //∫Xº–¿À¨d¨Oß_•ø¶bπÅE‹
    public bool IsTalk = false;

    // Start is called before the first frame update
    void Start()
    {

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void FirstTalkToPlayer()
    {
        if (IsTalk) return;

        IsTalk = true;
        TextComponent.text = string.Empty;
        StartCoroutine(OpenUIAnimation());
        StartCoroutine(TalkDialogue());
    }

    public void TalkToPlayer()
    {
        if (IsTalk) return;

        IsTalk = true;
        TextComponent.text = string.Empty;
        StartCoroutine(OpenUIAnimation());
        StartCoroutine(TalkDialogueTwo());
    }

    public void LastTalk()
    {
        if (IsTalk) return;

        TextComponent.text = string.Empty;
        StartCoroutine(OpenUIAnimation());
        StartCoroutine(LastTalkDialogue());
    }

    IEnumerator TalkDialogue()
    {
        yield return new WaitForSeconds(1f);
        for (Index = 0; Index < Lines.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines[Index]));

            if (Index < Lines.Length - 1)
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
        }

        yield return new WaitForSeconds(4f);
        StartCoroutine(CloseUIAnimation());
    }

    IEnumerator TalkDialogueTwo()
    {
        yield return new WaitForSeconds(1f);
        for (Index = 0; Index < Lines2.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines2[Index]));

            if (Index < Lines2.Length - 1)
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
        }

        yield return new WaitForSeconds(4f);
        StartCoroutine(CloseUIAnimation());
    }

    IEnumerator LastTalkDialogue()
    {
        yield return new WaitForSeconds(2f);

        for (Index = 0; Index < Lines3.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines3[Index]));

            if (Index < Lines3.Length - 1)
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
        }

        yield return new WaitForSeconds(4f);
        StartCoroutine(CloseUIAnimation());
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line.ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }

    private IEnumerator OpenUIAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.blocksRaycasts = true;
        gameObject.GetComponent<BossSceneDialogue>().openAction.Invoke();
    }
    private IEnumerator CloseUIAnimation()
    {
        gameObject.GetComponent<BossSceneDialogue>().openAction.Invoke();
        yield return new WaitForSeconds(0.5f);
        float duration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.blocksRaycasts = false;
        IsTalk = false;
    }
}
