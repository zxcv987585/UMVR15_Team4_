## UMVR15 專題作品《幻影之星》

本專案為三人團隊協作開發的 Unity 動作冒險遊戲
專題期間由我負責大部分 UI 系統邏輯與動畫效果
並參與部分 Shader 編寫與道具技能系統整合

### 我的主要貢獻－UI 系統設計與動畫整合

- 主選單與選項介面控制
  - `CheckToReturnUI`, `ExitUI`, `SettingUI`, `StopUI`, `PauseUI`
  - 使用 Coroutine 搭配 Easing 曲線製作 UI 動畫
  - 旗標 isOpen 判斷目前是否已有視窗開啟

- 主視覺動畫設計
  - `TitleAnimation.cs`：親自撰寫標題開場動畫邏輯（大量 Coroutine 與時間節點控制）
  - `titleEle.shader`：Shader 掃描光效果（使用 AI 協助撰寫與參數調整）

- 戰鬥 UI 與技能提示
  - `BossUI.cs`：由組員完成主要架構，並由我補上動畫細節
  - `GetSkillText.cs`, `UnLockSystem.cs`：升級與技能解鎖提示動畫（Coroutine 實作）

- 命名輸入介面
  - `InputNameUI.cs`：UI 出現動畫控制 + 與組員合作完成資料傳遞（PlayerPrefs）

---
### 道具與技能系統開發（主導）

此部分幾乎全由我負責，使用大量 ScriptableObject 管理資料，並建立拖曳邏輯。

- `InventoryManager.cs`, `ItemDragger.cs`, `HotbarManager.cs`, `ItemData.cs` 等
- Dragger & Dropper 系統，支援道具 / 技能拖曳至快捷欄並使用
- 顯示圖示、數量與資料讀取，與其他戰鬥與背包模組整合

---
### 場景互動功能開發

- `TeleportToBossArena.cs`, `TeleportToLoadScene.cs`
  - 傳送裝置互動與切場景特效（Coroutine 控制過場白光與黑幕）

- `RecoverFullHpPp.cs`
  - 恢復裝置功能與互動提示控制，補滿血量與能量值

---

## 備註說明

- 專案為三人協作完成，本頁面說明為我負責與主導的部分
- 部分功能架構由組員搭建，我參與了動畫整合與優化
- 初期以 AI 為輔協助架構撰寫，後期根據實作需求進行調整與整合

若有任何程式細節或整合方式想進一步了解，歡迎聯繫我進行補充說明！
