using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class DialogueTake : MonoBehaviour
{
    //視窗動畫
    public UnityEvent openAction;
    //儲存文字相關內容
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public string[] Lines2;
    public string[] Lines3;
    public string[] Lines4;
    public float TextSpeed;
    public float WaitForNextLine;
    //記錄文字進度所需Index
    private int Index;
    //需要關閉的第一道屏障牆壁
    public GameObject LockWall;
    //玩家第一次升級需要觸發的劇情事件
    public LevelSystem levelSystem;
    //第一區最後一段文字所需傳送的delegate（主要用於控制unity醬)
    public event Action LastTakeAction;
    //劇情結束後需要傳送的delegate
    public event Action TakeFinish;
    //BattleScene最後一區對話用delegate
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

        // 最後一區域, 解鎖傳送門用
        FindObjectOfType<UnlockPortal>().OpenIsTakeFinish();
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
