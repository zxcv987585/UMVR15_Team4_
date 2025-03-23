using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class DialogueTake : MonoBehaviour
{
    //µøµ¡°Êµe
    public UnityEvent openAction;
    //Àx¦s¤å¦r¬ÛÃö¤º®e
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public string[] Lines2;
    public string[] Lines3;
    public string[] Lines4;
    public float TextSpeed;
    public float WaitForNextLine;
    //°O¿ı¤å¦r¶i«×©Ò»İIndex
    private int Index;
    //»İ­nÃö³¬ªº²Ä¤@¹D«Ì»ÙÀğ¾À
    public GameObject LockWall;
    //ª±®a²Ä¤@¦¸¤É¯Å»İ­nÄ²µoªº¼@±¡¨Æ¥E
    public LevelSystem levelSystem;
    //²Ä¤@°Ï³Ì«á¤@¬q¤å¦r©Ò»İ¶Ç°eªºdelegate¡]¥D­n¥Î©ó±±¨û’nityÂE
    public event Action LastTakeAction;
    //¼@±¡µ²§ô«á»İ­n¶Ç°eªºdelegate
    public event Action TakeFinish;
    //BattleScene³Ì«á¤@°Ï¹EÜ¥Îdelegate
    public event Action LastAreaTakeFinish;

    private void Awake()
    {
        levelSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelSystem>();
    }

    void Start()
    {
        levelSystem.PlayerFirstLevelup += playerlevelUp;

        TextComponent.text = string.Empty;

        LockWall = GameObject.Find("ForcefieldRed (4)");

        StartCoroutine(UIAnimation());
        StartCoroutine(DisplayDialogue());
    }

    public void AreaTwoTakes()
    {
        TextComponent.text = string.Empty;
        gameObject.SetActive(true);
        StartCoroutine(OpenUIAnimation());
        StartCoroutine(DisplayDialogue2());
    }

    public void playerlevelUp()
    {
        TextComponent.text = string.Empty;
        gameObject.SetActive(true);
        StartCoroutine(OpenUIAnimation());
        StartCoroutine(DisplayDialogue3());
    }

    public void AreaTreeTakes()
    {
        TextComponent.text = string.Empty;
        gameObject.SetActive(true);
        StartCoroutine(OpenUIAnimation());
        StartCoroutine(DisplayDialogue4());
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
                AudioManager.Instance.PlaySound("Bye", transform.position);
                LastTakeAction?.Invoke();
            }
        }

        yield return new WaitForSeconds(4f);
        TakeFinish?.Invoke();
        LockWall?.SetActive(false);
        StartCoroutine(CloseUIAnimation());
    }

    IEnumerator DisplayDialogue2()
    {
        yield return new WaitForSeconds(0.8f);
        AudioManager.Instance.PlaySound("Uwa", transform.position);
        for (Index = 0; Index < Lines2.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines2[Index]));

            if (Index < Lines2.Length - 1)
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
            if (Index == Lines2.Length - 1)
            {
                AudioManager.Instance.PlaySound("Bye", transform.position);
                LastTakeAction?.Invoke();
            }
        }

        yield return new WaitForSeconds(4f);
        TakeFinish?.Invoke();
        StartCoroutine(CloseUIAnimation());
    }

    IEnumerator DisplayDialogue3()
    {
        yield return new WaitForSeconds(0.8f);
        AudioManager.Instance.PlaySound("Surprise", transform.position);
        for (Index = 0; Index < Lines3.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines3[Index]));

            if (Index < Lines3.Length - 1)
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
            if (Index == Lines3.Length - 1)
            {
                AudioManager.Instance.PlaySound("NiceFight", transform.position);
                LastTakeAction?.Invoke();
            }
        }

        yield return new WaitForSeconds(4f);
        TakeFinish?.Invoke();
        levelSystem.PlayerFirstLevelup -= playerlevelUp;
        StartCoroutine(CloseUIAnimation());
    }

    IEnumerator DisplayDialogue4()
    {
        yield return new WaitForSeconds(0.8f);
        AudioManager.Instance.PlaySound("Uwa", transform.position);
        for (Index = 0; Index < Lines4.Length; Index++)
        {
            yield return StartCoroutine(TypeLine(Lines4[Index]));

            if (Index < Lines4.Length - 1)
            {
                yield return new WaitForSeconds(WaitForNextLine);
                TextComponent.text = string.Empty;
            }
            if (Index == Lines4.Length - 1)
            {
                AudioManager.Instance.PlaySound("Uwwwaaaa", transform.position);
                LastAreaTakeFinish?.Invoke();
            }
        }

        yield return new WaitForSeconds(4f);
        TakeFinish?.Invoke();
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
    private IEnumerator OpenUIAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<DialogueTake>().openAction.Invoke();
    }
}
