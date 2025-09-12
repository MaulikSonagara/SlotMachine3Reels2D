# 🎰 Slot Machine Prototype

A **3-reel slot machine game** built in Unity.  
Players can place bets, spin the reels, and win payouts based on a data-driven paytable.  
Features persistence, logs, info pages, and audio feedback.

---

## 🛠️ Features

- **3-Reel Core Loop**
  - Place bet → Spin → Evaluate → Win/Lose → Update balance.
  - 5+ unique symbols with weighted probabilities.
  - Data-driven paytable using ScriptableObjects.

- **Gameplay**
  - Adjustable bet system.
  - Balance persistence across sessions.
  - Single payline win calculation.
  - Auto-Spin mode with toggle.

- **UI/UX**
  - **HUD**: balance, bet amount, spin button, auto-spin toggle.
  - **Settings Page**: SFX/Music toggle, sliders, persistent save.
  - **Log Page**: last 20 spins (bet, time, result, icons).
  - **Info Page**: paytable displayed in table format (icon, base value, 2x, 3x).
  - Feedback messages and claimed win amounts.

- **Audio**
  - Reel spin, stop, win/lose SFX.
  - Toggleable via settings.

- **Persistence**
  - Saves balance, last bet, settings, and spin logs using PlayerPrefs.

---

## 📂 Project Structure

<pre>
Assets/
├── Scripts/
│ ├── BetManager.cs
│ ├── HandleButton.cs
│ ├── PaytableUIManager.cs
│ ├── LogManager.cs
│ ├── InfoPanelManager.cs
│ ├── AudioManager.cs
│ └── SlotSymbol.cs (ScriptableObject)
├── ScriptableObjects/
│ ├── CherrySO
│ ├── LemonSo
│ ├── GrapeSO
│ ├── CloveSO
│ ├── BellSO
│ ├── SevenSO
│ ├── DiamondSO
│ └── PayTableSO
├── Prefabs/
│ ├── LogEntry.prefab
│ ├── InfoRow.prefab
│ └── UI Panels (Settings, Log, Info)
├── Scenes/
│ ├── MainScene.unity
├── Art/
│ ├── gameicons.png
│ 
</pre>
---

## ⚙️ Requirements

- **Unity** 2021.3 LTS or later (tested on 2022.3 LTS).
- PC / WebGL build supported.

---

## ▶️ How to Play

1. **Set Bet** – Adjust bet amount using + / – buttons.
2. **Spin** – Press handle or Spin button to start reels.
3. **Result** – Match 2 or 3 symbols on the payline to win.
4. **Logs** – Check recent spins in Log page.
5. **Info Page** – View paytable multipliers for each symbol.
6. **Settings** – Adjust or mute music/SFX.

---

## 🧮 Paytable Example

| Icon | Base Value | 2x Multiplier | 3x Multiplier |
|------|------------|---------------|---------------|
| 🍒 Cherry | 1 | 0.25x | 0.5x |
| 🍋 Lemon  | 1 | 0.5x  | 0.75x |
| ⭐ Star   | 2 | 1.5x  | 3x   |
| 🍀 Clover | 3 | 2x    | 5x   |
| 💎 Diamond | 5 | 5x    | 10x  |

*(Values fetched live from Paytable ScriptableObject.)*

---

## 📊 Architecture Diagram

<pre>
[ Player ]
↓
[ UI ] <──> [ BetManager ] <──> [ HandleButton ]
↓ ↓
[ PaytableUIManager ] [ AudioManager ]
↓
[ LogManager ] <──> [ InfoPanelManager ][ Player ]
↓
[ UI ] <──> [ BetManager ] <──> [ HandleButton ]
↓ ↓
[ PaytableUIManager ] [ AudioManager ]
↓
[ LogManager ] <──> [ InfoPanelManager ]
</pre>
---

## 📜 Credits

- **Icons**: Google image downloads / Self-made  
- **Music**: [Pixabay](https://pixabay.com/)  
- **Buttons**: [Unity Asset Store – Buttons Set](https://assetstore.unity.com/packages/2d/gui/buttons-set-211824)  
- **Fonts**: [Google Fonts – Luckiest Guy](https://fonts.google.com/specimen/Luckiest+Guy)  

---

## ⚠️ Known Issues

- Buttons clickable during spin unless locked.  
- Minimal win animations (future polish).  
- Reset functionality missing.  

---
