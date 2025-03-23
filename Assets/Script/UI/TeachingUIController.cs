using System.Buffers.Text;
using UnityEngine;

public class TeachingUIController : MonoBehaviour
{
    public GameObject BaseText;
    public GameObject AttackText;
    public GameObject BaseBG;
    public GameObject AttackBG;

    public void ChangeText()
    {
        BaseText?.SetActive(false);
        BaseBG?.SetActive(false);
        AttackText?.SetActive(true);
        AttackBG?.SetActive(true);
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}
