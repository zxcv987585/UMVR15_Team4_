using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharactersUIcontroller : MonoBehaviour
{
    //儲存視窗動畫
    public UnityEvent openAction;
    //抓取劇情UI介面
    public DialogueTake dialogue;
    //抓取unity醬的動畫
    public Animator animator;
    //抓取玩家的等級系統，播放初次升級事件
    public LevelSystem levelSystem;

    private void Awake()
    {
        levelSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UIAnimation());

        dialogue.TakeFinish += DisableUI;
        dialogue.LastTakeAction += playAnimation;
        dialogue.LastAreaTakeFinish += ScaredAnimation;
        levelSystem.PlayerFirstLevelup += EnableUI;

        animator = GameObject.FindGameObjectWithTag("UnityChan").GetComponent<Animator>();
    }

    public void EnableUI()
    {
        StartCoroutine(OpenUIAnimation());
    }

    private void DisableUI()
    {
        StartCoroutine(CloseUIAnimation());
    }

    public void LastArea()
    {
        StartCoroutine (LastAreaUIAnimation());
    }

    private IEnumerator UIAnimation()
    {
        yield return new WaitForSeconds(2.1f);
        AudioManager.Instance.PlaySound("Radio", transform.position);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
        yield return new WaitForSeconds(1.3f);
        animator.Play("WAIT03");

        AudioManager.Instance.PlaySound("Yaho", transform.position);
    }

    private IEnumerator OpenUIAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound("Radio", transform.position);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
    }

    private IEnumerator CloseUIAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound("Radio", transform.position);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
    }

    private IEnumerator LastAreaUIAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound("Radio", transform.position);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
        animator.Play("LOSE00");

    }

    private void playAnimation()
    {
        animator.Play("WIN00"); 
    }

    private void ScaredAnimation()
    {
        animator.Play("REFLESH00");
    }
}
