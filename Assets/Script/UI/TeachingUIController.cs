using System.Buffers.Text;
using TMPro;
using UnityEngine;

public class TeachingUIController : MonoBehaviour
{
    public GameObject BaseText;
    public GameObject AttackText;
    public GameObject BaseBG;
    public GameObject AttackBG;

    private void Start()
    {
        GameInput gameInput = GameInput.Instance;
    
        BaseText.GetComponent<TextMeshProUGUI>().text = string.Format(
            "※ 移動滑鼠來觀察周圍\n" +
            "※ 按下 {0}{1}{2}{3} 來移動\n" +
            "※ 按下 {4} 來攻擊\n" +
            "※ 移動時按下 {5} 來衝刺\n" +
            "※ 按下 {6} 來打開背包\n" +
            "※ 將獲得的道具移動到快捷鍵使用",
            gameInput.GetBindText(GameInput.Bind.MoveUp),
            gameInput.GetBindText(GameInput.Bind.MoveDown),
            gameInput.GetBindText(GameInput.Bind.MoveLeft),
            gameInput.GetBindText(GameInput.Bind.MoveRight),
            gameInput.GetBindText(GameInput.Bind.Attack),
            gameInput.GetBindText(GameInput.Bind.Dash),
            gameInput.GetBindText(GameInput.Bind.ItemMenu)
        );
        
        AttackText.GetComponent<TextMeshProUGUI>().text = string.Format(
            "※ 按下 {0} 來鎖定敵人\n" +
            "※ 按下 {1} DASH\n" +
            "※ 按下 {2} 切換遠程武器\n" +
            "※ 擊殺敵人可獲得道具與經驗值\n",
            gameInput.GetBindText(GameInput.Bind.LockOn),
            gameInput.GetBindText(GameInput.Bind.Dash),
            gameInput.GetBindText(GameInput.Bind.Aim)
        );
    }

    public void ChangeText()
    {
        GameInput gameInput = GameInput.Instance;
    
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
