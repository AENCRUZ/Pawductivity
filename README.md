<div align="center">

# 🐾 Pawductivity

**A Digital Pet Productivity System**

*CS 222 · Advanced Object-Oriented Programming · Batangas State University*

![Team](https://img.shields.io/badge/Team-LAVA-ff69b4?style=for-the-badge)
![Section](https://img.shields.io/badge/Section-CS--2202-c084fc?style=for-the-badge)

![Platform](https://img.shields.io/badge/Windows-0078D4?style=flat-square&logo=windows&logoColor=white)
![Framework](https://img.shields.io/badge/.NET_8_WinForms-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![IDE](https://img.shields.io/badge/Visual_Studio-5C2D91?style=flat-square&logo=visualstudio&logoColor=white)
![Theme](https://img.shields.io/badge/Theme-Pink_Kawaii_🌸-ff69b4?style=flat-square)

> *Stay productive. Keep your pet happy. Don't let your tasks go overdue.*

</div>

---

## 📖 Overview

**Pawductivity** is a gamified productivity desktop app built with **.NET 8 WinForms**. You adopt a virtual pet — a cat 🐱 or a dog 🐶 — and your tasks directly affect its health, mood, level, coins, and evolution.

Complete tasks and your pet gains XP, mood, health, and coins. Let tasks become overdue and your pet loses health and mood. The app now includes animated pet reactions, floating stat-change animations, coin gain effects, and shop item animations so the pet feels more alive while you manage tasks.

It's a productivity tool with stakes — and a little companion watching your progress.

---

## 🚀 Getting Started

### Prerequisites

| # | Requirement | Details |
|---|---|---|
| 1 | [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) | Windows only — WinForms requires Windows |
| 2 | **.NET Desktop Development** workload | Select this during Visual Studio installation |
| 3 | **.NET 8 SDK** | Required by `net8.0-windows` |

### Running the App

1. Open **Visual Studio Community**
2. Click **Open a project or solution**
3. Navigate to the `Pawductivity/` folder
4. Open `Pawductivity.slnx`
5. Press **F5** to build and run

> 💡 **Tip:** Use `Ctrl + F5` to run without the debugger for a faster startup.

---

## 📁 Project Structure

```text
Pawductivity/
├── Pawductivity.slnx              ← Solution file
├── Pawductivity.csproj            ← Project file
├── Program.cs                     ← Entry point
├── PawTheme.cs                    ← Centralized theme: colors, fonts, button styles
│
├── Animations/
│   ├── PetAnimationState.cs       ← Pet animation state enum
│   └── PetRenderer.cs             ← Cat, dog, speech bubble, and drawing helpers
│
├── Controls/
│   └── PetAnimationControl.cs     ← Animated pet canvas and visual effects
│
├── Models/
│   ├── Pet.cs                     ← Abstract base class: shared pet state and evolution
│   ├── PetTypes.cs                ← CatPet and DogPet behavior
│   ├── PetChangeResult.cs         ← Stat-change result used for UI animations
│   ├── TaskItem.cs                ← Task data model and overdue penalty tracking
│   ├── ShopItem.cs                ← Shop item model and default shop list
│   └── SaveData.cs                ← Serializable snapshot models
│
├── Managers/
│   ├── GameManager.cs             ← Core game logic and stat-change calculations
│   └── SaveManager.cs             ← File I/O: save, load, list, delete profiles
│
└── Forms/
    ├── LoginForm.cs               ← Profile selector and new profile creation
    ├── DashboardForm.cs           ← Main screen: task list, stats, and pet control host
    ├── TaskEditForm.cs            ← Add and edit task dialog
    ├── ShopForm.cs                ← Coin shop and purchase flow
    └── StatsForm.cs               ← Productivity analytics
```

---

## 🔄 Gameplay Loop

```text
Login → Add Task → Complete Task → Pet Reacts → Earn Coins → Buy Items
           ↑                                                       |
           └───────────────────── loop ────────────────────────────┘
```

Every task you complete rewards you and your pet. Every overdue task applies a health and mood penalty once, then remembers that penalty so the same task does not drain the pet repeatedly every minute.

Progress is **automatically saved** when the app closes and restored when you reopen it. Profiles, pet stats, tasks, streaks, coins, and overdue penalty state are all persisted.

---

## 🐾 Pet Animations

The pet animation system is separated from the dashboard UI. `DashboardForm` hosts `PetAnimationControl`, while animation drawing lives in `Animations/PetRenderer.cs`.

| Event | Animation |
|---|---|
| Normal pet idle | Gentle bounce, blinking, mood-based expression |
| Speech bubble | Random cat/dog messages based on mood |
| Task completed | XP gain, mood gain, and coin gain floating text |
| Task overdue | Health loss and mood loss floating text |
| Coin reward | Coin gain animation after task completion |
| Shop purchase | Item-specific visual effect and stat gain animation |

---

## 🛍️ Shop Items & Purchase Animations

Coins are earned by completing tasks (`XP gained ÷ 2` per task). Spend them in the shop to restore your pet's health and mood.

| Item | Cost | Health | Mood | Purchase Animation |
|---|:---:|:---:|:---:|---|
| 🎀 Pink Ribbon | 10 | — | +15 | Ribbon sparkle / mood boost |
| 🍪 Star Cookie | 15 | +20 | +10 | Eating animation plus health and mood |
| 🍓 Strawberry Milk | 20 | +30 | — | Sip animation plus health |
| 🌸 Flower Crown | 25 | — | +30 | Bloom / mood boost |
| 🛏️ Cozy Blanket | 30 | +25 | +20 | Cozy effect plus health and mood |
| 🌈 Rainbow Toy | 40 | — | +40 | Play animation plus mood |

---

## 🌱 Pet Evolution

Your pet evolves through five stages as you level up. Each level costs `current_level × 50 XP`, so leveling gets progressively harder.

| Stage | Level | Cat 🐱 | Dog 🐶 |
|---|---|---|---|
| 🥚 **Egg** | 1 | `🥚` | `🥚` |
| 🐱 **Baby** | 2–3 | `🐱` | `🐶` |
| 🐈 **Junior** | 4–6 | `🐈‍⬛` | `🐕` |
| 🐈 **Adult** | 7–9 | `🐈` | `🦮` |
| ✨ **Legend** | 10+ | `✨🐈‍⬛✨` | `✨🐕‍🦺✨` |

**How XP works:** cats earn more XP per task but lose mood faster when they miss one. Dogs earn slightly less XP but are more forgiving on mood, though they take more health damage.

| Pet | High priority | Medium priority | Low priority |
|---|---:|---:|---:|
| 🐱 Cat XP | +30 | +20 | +10 |
| 🐶 Dog XP | +25 | +15 | +8 |

> Each pet starts with **Health 80 · Mood 70 · Level 1 · 0 coins**. Health and mood are clamped between 0–100, and coins can never go below 0.

---

## 😺 Mood System

Your pet's mood is a 0–100 value that maps to one of four states:

| Mood value | State | Emoji | Effect |
|---|---|---|---|
| 70–100 | Happy | `🐾✨` | Positive greetings and happy animation state |
| 40–69 | Neutral | `🐾` | Calm, waiting behavior |
| 20–39 | Sad | `😿` / `🥺` | Sad expression and sad animation state |
| 0–19 | Sick | `🤒` | Urgent state — complete tasks or buy helpful items |

### Task Effects

| Event | Cat 🐱 | Dog 🐶 |
|---|---|---|
| Complete high task | +30 XP, +15 Mood, +5 Health, +15 Coins | +25 XP, +20 Mood, +8 Health, +12 Coins |
| Complete medium task | +20 XP, +15 Mood, +5 Health, +10 Coins | +15 XP, +20 Mood, +8 Health, +7 Coins |
| Complete low task | +10 XP, +15 Mood, +5 Health, +5 Coins | +8 XP, +20 Mood, +8 Health, +4 Coins |
| Miss overdue task | −20 Mood, −8 Health | −12 Mood, −10 Health |

Overdue penalties are applied only once per task using `TaskItem.OverduePenaltyApplied`.

---

## 🎮 Features

| Feature | Status |
|---|:---:|
| Login with username and pet name | ✅ |
| Multi-profile support | ✅ |
| Choose Cat 🐱 or Dog 🐶 | ✅ |
| Add, edit, delete, and complete tasks | ✅ |
| Task priority and due-date tracking | ✅ |
| Complete tasks → pet gains XP, mood, health, and coins | ✅ |
| Overdue tasks → pet loses health and mood once per overdue task | ✅ |
| Pet levels up and evolves | ✅ |
| Animated cat and dog pet drawings | ✅ |
| Speech bubbles based on mood | ✅ |
| Task completion animations | ✅ |
| XP, mood, health, and coin floating animations | ✅ |
| Shop item purchase animations | ✅ |
| Coin-based shop system | ✅ |
| Daily streak tracking | ✅ |
| Productivity stats and analytics screen | ✅ |
| Consistent pink kawaii theme | ✅ |
| Data persistence across sessions | ✅ |
| Atomic save writes | ✅ |

---

## 🎓 OOP Principles

Pawductivity is built as a deliberate showcase of the four core OOP concepts.

### 🔒 Encapsulation — `Pet.cs`

`Pet` protects its core stats with private backing fields:

```csharp
private int _health;
private int _mood;
private int _xp;
private int _level;
private int _coins;
```

Public properties enforce rules whenever values change:

```csharp
public int Health
{
    get => _health;
    set => _health = Math.Clamp(value, 0, 100);
}

public int XP
{
    get => _xp;
    set { _xp = value; CheckLevelUp(); }
}

public int Coins
{
    get => _coins;
    set => _coins = Math.Max(0, value);
}
```

This keeps health and mood between 0–100, prevents negative coins, and automatically checks for level-ups whenever XP changes.

### 🧬 Inheritance — `Pet.cs` → `CatPet` / `DogPet`

`Pet` is an abstract base class. It owns shared pet data, mood calculation, level-up logic, evolution, starting stats, and persistence restoration.

```csharp
public abstract class Pet { ... }
public class CatPet : Pet { ... }
public class DogPet : Pet { ... }
```

`CatPet` and `DogPet` inherit the shared system, then define their own rewards, penalties, greetings, and stage emojis.

### 🔀 Polymorphism — `PetTypes.cs`

`Pet` requires each subclass to implement its own reactions:

```csharp
public abstract void ReactToTaskCompleted(TaskItem task);
public abstract void ReactToTaskMissed();
public abstract string GetGreeting();
```

The same call produces different behavior depending on whether the current pet is a cat or dog. `GameManager` can call `Pet.ReactToTaskCompleted(task)` without needing separate UI code for each pet type.

### 🏗️ Abstraction — `GameManager.cs`, `PetAnimationControl.cs`, `SaveManager.cs`

`GameManager` hides the rules for completing tasks, applying overdue penalties, buying items, updating streaks, and calculating stat changes. Forms call simple methods such as:

```csharp
var change = _gm.CompleteTask(task.Id);
var overdue = _gm.ApplyOverduePenalties();
var purchase = _gm.BuyItem(item);
```

Those methods return `PetChangeResult`, which tells the UI exactly what changed. `PetAnimationControl` then turns those changes into animations without duplicating game logic.

Persistence is abstracted too. Forms call `SaveManager.Save(_gm)` and `SaveManager.Restore(data)` without knowing about JSON, app data folders, temp files, or atomic writes.

---

## 🌸 Theming

All colors and fonts live in `PawTheme.cs`. Change a value here and it updates every form, button, progress bar, and themed control in the app.

```csharp
public static readonly Color Background = Color.FromArgb(255, 240, 245);
public static readonly Color Surface    = Color.FromArgb(255, 220, 230);
public static readonly Color Primary    = Color.FromArgb(255, 105, 150);
public static readonly Color Secondary  = Color.FromArgb(255, 182, 193);
public static readonly Color TextDark   = Color.FromArgb( 80,  30,  50);
public static readonly Color TextMuted  = Color.FromArgb(160,  90, 120);
public static readonly Color HealthBar  = Color.FromArgb(255,  80, 120);
public static readonly Color MoodBar    = Color.FromArgb(255, 200,  80);
public static readonly Color XpBar      = Color.FromArgb(140, 200, 255);
```

`PawTheme.StyleButton(btn)` and `PawTheme.StyleButton(btn, outlined: true)` apply consistent styling and hover behavior from one helper method.

---

<div align="center">

## 👥 Team

**Team LAVA** · CS-2202 · Batangas State University

*Made with 💖 for CS 222 — Advanced Object-Oriented Programming*

</div>
