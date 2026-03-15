# ChainRacePatternUnity

A chain-based animation and async flow control library for Unity.  
Compose complex animation flows declaratively using Sequence, Parallel, and Race.  
In this repository, the approach of handling skip controls in animation chains as a **Race** is called the **ChainRacePattern**.

**[日本語版はこちら (Japanese)](README_ja.md)**

> **Note:** This is not a production-ready library, but rather a reference implementation.  
> It is kept to a minimal set of features, and is intended to be extended and adapted to suit your specific use case and project requirements.

## Demo

Live demos are available in WebGL:

- [Scene1 - Basic Operations](https://morishift.github.io/ChainRacePatternUnity-Demo/Scene1-Base/)
- [Scene2 - Result Screen](https://morishift.github.io/ChainRacePatternUnity-Demo/Scene2-ResultMock/)

## Chain Base Rules

`Chain` is the base class representing an animation or procedure. When implementing a custom Chain, follow these rules:

- Implement the startup procedure in `StartInternal()`
- Call `Complete()` when the procedure finishes
- `SkipInternal()` may be called after the Chain has started. It must transition to the final state immediately
- Do not call `Complete()` from inside `SkipInternal()` (it will be ignored)
- `isFastForward` is `true` when `Skip()` is guaranteed to be called immediately after start. The Chain can check this value at startup to skip unnecessary work (e.g. starting animations or allocating resources)

## Core Classes

### ChainSequence

Executes multiple Chains **in order**. The next Chain starts only after the previous one completes.

```csharp
await new ChainSequence(
    new ChainDelay(0.5f),                       // Wait 0.5 seconds
    new ChainAction(() => Debug.Log("Hello")),  // Log output
    new ChainDelay(1.0f)                        // Wait 1 second
).Start();
```

### ChainParallel

Executes multiple Chains **simultaneously**. Completes when **all Chains** have finished.

```csharp
await new ChainParallel(
    ChainMoveTween(rectA, targetA, 1.0f),  // Move A
    ChainMoveTween(rectB, targetB, 1.0f)   // Move B at the same time
).Start();
// Proceeds after both movements finish
```

### ChainRace

Executes multiple Chains **simultaneously**. Completes when **any one Chain** finishes, and skips the rest.

```csharp
await new ChainRace(
    new ChainButton(skipButton),    // Wait for skip button
    new ChainSequence(              // Animation body
        ChainMoveTween(rect, pos1, 1.0f),
        ChainMoveTween(rect, pos2, 1.0f)
    )
).Start();
// Proceeds when the animation ends OR the skip button is pressed
```

## Implementing Skip

In games and apps, it's common to let users skip cutscenes and animations by pressing a button.

A traditional approach requires writing logic to individually stop and finalize each running animation when the skip button is pressed. As animations grow more complex, the skip logic becomes increasingly tangled.

ChainRacePattern solves this problem as follows:

1. Represent user input as a Chain (e.g. `ChainButton`)
2. Place the animation Chain and `ChainButton` together inside a `ChainRace`
3. When the button is pressed, `ChainRace` automatically skips the remaining animations

```csharp
// Whichever finishes first — the skip button or the animation —
// the other is skipped and execution moves on
new ChainRace(
    new ChainButton(skipButton),
    new ChainSequence(
        CutsceneA,
        CutsceneB,
        CutsceneC
    )
)
```

Each Chain implements `SkipInternal()` to handle its own cleanup, so the final state is always correct regardless of when a skip occurs. There is no need to write separate skip logic.

## Sample Scenes

### Scene1 - Three Skip Patterns

Demonstrates three skip patterns using an animation where a rectangle moves through positions 1→2→3→4.

**Sequence1: Skip All**

The entire animation is wrapped in a single `ChainRace`. Pressing skip jumps straight to the final state.

```csharp
new ChainRace(
    new ChainButton(skipButton),
    new ChainSequence(
        Move 1→2, Move 2→3, Move 3→4  // All skipped at once
    )
)
```

**Sequence2: Skip Per Section**

Each movement is wrapped in its own `ChainRace`. Pressing skip advances to the next section.

```csharp
new ChainRace(new ChainButton(skipButton), Move 1→2),  // 1st skip
new ChainRace(new ChainButton(skipButton), Move 2→3),  // 2nd skip
new ChainRace(new ChainButton(skipButton), Move 3→4),  // 3rd skip
```

**Sequence3: Non-Skippable Section**

Section 2 is a plain `ChainSequence` without `ChainRace`. Sections 1 and 3 are skippable, but section 2 always plays to completion.

```csharp
new ChainRace(new ChainButton(skipButton), Move 1→2),  // Skippable
new ChainSequence(Move 2→3),                             // Non-skippable (must watch)
new ChainRace(new ChainButton(skipButton), Move 3→4),  // Skippable
```

### Scene2 - Result Screen

A practical sample simulating a game result screen. It combines fade effects, dialog animations, ranking displays, bonus effects, and touch input — all controlled through Chain composition.

The flow is as follows:

1. **Fade out + Show dialog** — `ChainParallel` runs the fade and dialog animation simultaneously. `ChainRace` allows skipping by tapping the screen
2. **Bonus point animation** — Each player's bonus is displayed with staggered timing. Also skippable via `ChainRace`
3. **Wait for touch** — `ChainButton` waits for a screen tap
4. **Hide dialog + Fade in** — `ChainParallel` runs the exit animation and fade-in simultaneously

```csharp
new ChainSequence(
    // 1. Fade out + Show dialog (skippable)
    new ChainRace(
        new ChainButton(screenButton),
        new ChainParallel(
            fadePanel.ChainFade(false),
            resultDialog.ChainShowDialog()
        )
    ),
    // 2. Bonus animation (skippable)
    new ChainRace(
        new ChainButton(screenButton),
        resultDialog.ChainShowBonus()
    ),
    // 3. Wait for touch
    ChainTouchScreen(),
    // 4. Hide dialog + Fade in
    new ChainParallel(
        resultDialog.ChainHideDialog(),
        fadePanel.ChainFade(true)
    )
)
```

By combining Sequence, Parallel, and Race, complex animation flows and skip controls can be described declaratively.

## Other Classes

| Class | Description |
|---|---|
| `ChainAction` | Executes a single action and completes immediately |
| `ChainDelay` | Waits for a specified duration |
| `ChainAnimator` | Waits for an Animator state to finish playing |
| `ChainWork` | Runs per-frame update logic (`onStart` / `onUpdate` / `onSkip` events) |
| `ChainHalt` | Never completes (only ends via external Skip) |
| `ChainNop` | Does nothing and completes immediately |

## Installation

1. Copy the scripts in the `ChainPattern` folder into your Unity project
2. [UniTask](https://github.com/Cysharp/UniTask) is required. Install it beforehand

## License

[MIT License](LICENSE)
