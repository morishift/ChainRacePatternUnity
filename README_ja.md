# ChainRacePatternUnity

ChainRacePattern は、Unity における演出スキップ問題に対する設計パターンです。  
演出・入力・待機などを Chain として表現し、Sequence / Parallel / Race の組み合わせで宣言的にフローを記述できます。

**[English version](README.md)**

> **注意:** ChainRacePattern は現在、提案段階の設計パターンです。  
> この仕組みは完成品のライブラリではなく、あくまで実装例です。  
> 必要最低限の構成に留めているため、そのまま利用するのではなく、用途や案件に応じて機能追加・調整したうえで利用することを想定しています。

## デモ

WebGLで動作するデモを公開しています。

- [Scene1 - 基本操作](https://morishift.github.io/ChainRacePatternUnity-Demo/Scene1-Base/)  
<img src="https://github.com/user-attachments/assets/b22f074c-74b5-49b0-a035-1fad09d267be" width="400" alt="Scene1-Simple"><br>  

- [Scene2 - リザルト演出](https://morishift.github.io/ChainRacePatternUnity-Demo/Scene2-ResultMock/)  
<img src="https://github.com/user-attachments/assets/2c69a4d4-7ae7-46ec-ab46-9da0d1142285" width="400" alt="Scene2-ResultScreenMockup"><br>     

発想の核だけ先に見たい場合は、[演出スキップの実装](#演出スキップの実装) から読むのがおすすめです。

## Chainの基本ルール

`Chain` はアニメーションや手続きを表す基底クラスです。独自のChainを実装する場合は、以下のルールに従ってください。

1. `StartInternal()` に開始時の手続きを実装し、手続きが完了したら `Complete()` を呼び出す
2. `SkipInternal()` は開始後に呼び出される場合がある。呼び出された場合は直ちに終了状態に遷移しなければならない
3. `SkipInternal()` の内部から `Complete()` は呼び出さない（呼び出しても無視される）
4. `StartInternal()` と `SkipInternal()` はそれぞれ最大1回だけ呼ばれる。`Chain` は使い捨てなので再利用せず、毎回新しく生成する
5. `isFastForward` は、開始直後に `Skip()` が実行されることが確定している場合に `true` になる。Chainは開始時にこの値を参照することで、不要な手続き（アニメーション開始やリソース確保など）を省略できる

## 設計方針

- Skipは「止める」処理ではなく、残りのフローを消費して整合した最終状態へ進める処理として扱う
- Tween / 待機 / 入力 / Animator / SE などを同じ `Chain` 単位で扱う
- Skip責務は外側の制御ロジックに集中させず、各 `Chain` に閉じ込める
- 演出全体は巨大な1つの手続きではなく、`Sequence` `Parallel` `Race` の組み合わせとして表現する
- ひとまとまりの意味を持つ処理やアニメーションを `Chain` として表現することで、再利用しやすくなる
- コルーチンとフラグで素朴に管理すると、一時的な状態フラグや制御コードが増えやすく、スキップ対応がスパゲッティ化しやすい
- Chain内部で発生する例外は利用者側の責任で扱うものとし、フレームワーク側では例外の捕捉・回復・継続実行は行わない

## 注意点

- `isFastForward` が `true` のときの挙動は、各 `Chain` ごとに必ず確認してください
- あえて `SkipInternal()` を実装しない `Chain` はスキップできませんが、スキップ対応が不要な箇所ではそれでも有用です

## 主なクラス

### ChainSequence

複数のChainを**順番に**実行します。1つ目が完了したら2つ目、2つ目が完了したら3つ目、と順次進行します。

```csharp
await new ChainSequence(
    new ChainDelay(0.5f),                       // 0.5秒待つ
    new ChainAction(() => Debug.Log("Hello")),  // ログ出力
    new ChainDelay(1.0f)                        // 1秒待つ
).Start();
```

### ChainParallel

複数のChainを**同時に**実行します。**全てのChainが完了**したときに終了します。

```csharp
await new ChainParallel(
    ChainMoveTween(rectA, targetA, 1.0f),  // Aを移動
    ChainMoveTween(rectB, targetB, 1.0f)   // Bを同時に移動
).Start();
// 両方の移動が終わったら次へ
```

### ChainRace

複数のChainを**同時に**実行します。**いずれか1つが完了**した時点で、残りのChainをスキップして終了します。

```csharp
await new ChainRace(
    new ChainButton(skipButton),    // スキップボタンの入力待ち
    new ChainSequence(              // アニメーション本体
        ChainMoveTween(rect, pos1, 1.0f),
        ChainMoveTween(rect, pos2, 1.0f)
    )
).Start();
// アニメーションが終わるか、スキップボタンが押されたら次へ
```

## 演出スキップの実装

ゲームやアプリの演出では、ユーザーがボタンを押してカットシーンや演出を飛ばせるようにすることがよくあります。

通常の実装では、「スキップボタンが押されたときに、実行中のアニメーションを個別に停止・完了させる処理」を書く必要があり、演出が複雑になるほどスキップ処理も煩雑になります。

ChainRacePattern では、この問題を以下のように扱います。

1. ユーザー入力を `ChainButton` のようなChainとして表現する
2. `ChainRace` にアニメーションのChainと `ChainButton` を並べて実行する
3. ボタンが押されれば `ChainRace` が残りのアニメーションを自動的にスキップする

```csharp
// スキップボタンが押されるか、アニメーションが最後まで再生されるか、
// どちらか先に起きた方で次に進む
new ChainRace(
    new ChainButton(skipButton),
    new ChainSequence(
        演出A,
        演出B,
        演出C
    )
)
```

各Chainは `SkipInternal()` でスキップ時の終了処理を実装しているため、スキップが発生しても正しい最終状態に遷移します。  
そのため、外側のフロー制御に場当たり的なスキップ処理を書き散らさずに済みます。

## サンプルシーンの解説

### Scene1 - スキップの3パターン

矩形が位置1→2→3→4と移動するアニメーションを使って、ChainRaceによるスキップの3つのパターンを示しています。

**Sequence1：全体スキップ**

アニメーション全体を1つの `ChainRace` で囲んでいます。スキップボタンを押すと、残りのアニメーションが全てスキップされ、一気に最終状態まで遷移します。

```csharp
new ChainRace(
    new ChainButton(skipButton),
    new ChainSequence(
        移動1→2, 移動2→3, 移動3→4  // 全てまとめてスキップ対象
    )
)
```

**Sequence2：セクション単位スキップ**

各移動を個別の `ChainRace` で囲んでいます。スキップボタンを押すと現在のセクションだけがスキップされ、次のセクションが始まります。

```csharp
new ChainRace(new ChainButton(skipButton), 移動1→2),  // 1回目のスキップ
new ChainRace(new ChainButton(skipButton), 移動2→3),  // 2回目のスキップ
new ChainRace(new ChainButton(skipButton), 移動3→4),  // 3回目のスキップ
```

**Sequence3：スキップ不可区間**

セクション2だけ `ChainRace` で囲まず、素の `ChainSequence` にしています。セクション1・3はスキップ可能ですが、セクション2は必ず最後まで再生されます。

```csharp
new ChainRace(new ChainButton(skipButton), 移動1→2),  // スキップ可能
new ChainSequence(移動2→3),                           // スキップ不可（必ず再生）
new ChainRace(new ChainButton(skipButton), 移動3→4),  // スキップ可能
```

### Scene2 - リザルト演出

ゲームのリザルト画面を想定した実践的なサンプルです。フェード、ダイアログ表示、ランキングアニメーション、ボーナス演出、タッチ待ちといった複数の演出要素をChainの組み合わせで制御しています。

演出の流れは以下のとおりです。

1. **オーバーレイのフェード + ダイアログ表示** — `ChainParallel` でフェードとダイアログ表示を同時に実行。`ChainRace` で画面タップによるスキップにも対応
2. **ボーナスポイント演出** — 各プレイヤーのボーナスを時間差で表示。こちらも `ChainRace` でスキップ可能
3. **タッチ待ち** — `ChainButton` で画面タップを待機
4. **ダイアログ非表示 + オーバーレイ復帰** — `ChainParallel` でダイアログの退場アニメーションとオーバーレイ遷移を同時に実行

```csharp
new ChainSequence(
    // 1. オーバーレイのフェード + ダイアログ表示（スキップ可能）
    new ChainRace(
        new ChainButton(screenButton),
        new ChainParallel(
            fadePanel.ChainFade(false),
            resultDialog.ChainShowDialog()
        )
    ),
    // 2. ボーナス演出（スキップ可能）
    new ChainRace(
        new ChainButton(screenButton),
        resultDialog.ChainShowBonus()
    ),
    // 3. タッチ待ち
    ChainTouchScreen(),
    // 4. ダイアログ非表示 + オーバーレイ復帰
    new ChainParallel(
        resultDialog.ChainHideDialog(),
        fadePanel.ChainFade(true)
    )
)
```

Sequence、Parallel、Raceを組み合わせることで、複雑な演出フローとスキップ制御を宣言的に記述できます。

## その他のクラス

| クラス | 説明 |
|---|---|
| `ChainAction` | 単一のアクションを実行して即完了 |
| `ChainDelay` | 指定秒数待機 |
| `ChainAnimator` | Animatorのステート再生を待機 |
| `ChainWork` | 毎フレーム更新処理を実行（`onStart` / `onUpdate` / `onSkip` イベント）<br>柔軟ですが、多用するとあまり美しくありません。 |
| `ChainHalt` | 完了しないChain（外部からのSkipでのみ終了） |
| `ChainNop` | 何もせず即完了 |

## 導入方法

### このサンプルプロジェクトをそのまま開く

このリポジトリを clone またはダウンロードして Unity で開いてください。  
UniTask を含む必要なパッケージは、Unity Package Manager により自動で解決されます。

### ChainRacePattern のスクリプトだけを自分のプロジェクトへ持ち込む

ChainRacePattern のスクリプトだけを別の Unity プロジェクトへコピーする場合は、事前にそのプロジェクトへ UniTask を導入してください。

## ライセンス

[MIT License](LICENSE)
