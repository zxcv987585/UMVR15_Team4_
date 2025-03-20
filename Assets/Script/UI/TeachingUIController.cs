using UnityEngine;

public class TeachingUIController : MonoBehaviour
{
    public GameObject BaseText;
    public GameObject AttackText;

    public void ChangeText()
    {
        BaseText?.SetActive(false);
        AttackText?.SetActive(true);
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}
