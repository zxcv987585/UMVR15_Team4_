using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI TextComponent;
    public string[] Lines;
    public float TextSpeed;

    private int Index;

    // Start is called before the first frame update
    void Start()
    {
        TextComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (TextComponent.text == Lines[Index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                TextComponent.text = Lines[Index];
            }
        }
    }

    void StartDialogue()
    {
        Index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach(char c in Lines[Index].ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }

    void NextLine()
    {
        if (Index < Lines.Length - 1)
        {
            Index++;
            TextComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
