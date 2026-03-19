# ChainRacePatternUnity

Unity向けのチェーンベースのアニメーション制御ライブラリです。  
Sequence（順次）、Parallel（並列）、Race（競争）の組み合わせで、複雑なアニメーションフローを宣言的に記述できます。  
また、演出チェーンのスキップ制御を **Race** として扱うアプローチを、本リポジトリでは **ChainRacePattern** と呼んでいます。

**[English version](README.md)**

> **注意:** ChainRacePattern は現時点では提案段階の設計パターンです。  
> この仕組みは完成品のライブラリではなく、あくまで実装例です。  
> 必要最低限の構成に留めているため、そのまま利用するのではなく、用途や案件に応じて機能追加・調整したうえで利用することを想定しています。

## デモ

WebGLで動作するデモを公開しています。

- [Scene1 - 基本操作](https://morishift.github.io/ChainRacePatternUnity-Demo/Scene1-Base/)
- [Scene2 - リザルト演出](https://morishift.github.io/ChainRacePatternUnity-Demo/Scene2-ResultMock/)

## Chainの基本ルール

`Chain` はアニメーションや手続きを表す基底クラスです。独自のChainを実装する場合は、以下のルールに従ってください。

1. `StartInternal()` に開始時の処理を実装し、処理が完了したら `Complete()` を呼び出す
2. `SkipInternal()` は開始後に呼び出される場合がある。呼び出された場合は、直ちに終了状態へ遷移しなければならない
3. `SkipInternal()` の内部から `Complete()` は呼び出さない（呼び出しても無視される）
4. `StartInternal()` / `SkipInternal()` は、それぞれ多くても 1 回しか呼び出されない。`Chain` は内部に状態を持つため、使い回さず毎回 `new` して利用する
5. `isFastForward` は、開始直後に `Skip()` が実行されることが確定している場合に `true` になる。`Chain` は開始時にこの値を参照することで、不要な処理（アニメーション開始やリソース確保など）を省略できる

## この設計の考え方

- スキップは停止ではなく、残りを消費して整合した最終状態へ寄せる操作として扱う
- Tween / 待機 / 入力 / Animator / SE などを、同じ `Chain` 単位で扱う
- スキップ時の責務は外側で一括処理せず、各 `Chain` に閉じ込める
- 演出全体は巨大な手続きではなく、`Sequence` / `Parallel` / `Race` の合成として表現する
- ひとまとまりの意味を持つ処理やアニメーションを Chain として表現することで、再利用しやすくなる
- 素朴にコルーチンやフラグで管理すると、暫定フラグや制御用コードが増え、スキップ処理が複雑化しやすい

## 注意

- `isFastForward` が `true` のとき、各 `Chain` がどのように振る舞うかを十分確認してください
- `SkipInternal()` をあえて実装しない `Chain` はスキップできませんが、演出スキップを前提としない用途では利用価値があります

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

ゲームやアプリの演出では、ユーザーがスキップボタンを押して演出を飛ばせるようにすることがよくあります。

通常の実装では「スキップボタンが押されたときに、実行中のアニメーションを個別に停止・完了させる手続き」を書く必要があり、アニメーションが複雑になるほどスキップ処理も煩雑になります。

ChainRacePatternでは、この問題を以下のように解決しています。

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

各 `Chain` は `SkipInternal()` で、スキップ時に自身を正しい完了状態へ遷移させます。
そのため、スキップが発生しても演出は正しい最終状態に到達し、演出フロー側で個別のスキップ処理を書く必要はありません。

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
new ChainSequence(移動2→3),                             // スキップ不可（必ず再生）
new ChainRace(new ChainButton(skipButton), 移動3→4),  // スキップ可能
```

### Scene2 - リザルト演出

ゲームのリザルト画面を想定した実践的なサンプルです。フェード、ダイアログ表示、ランキングアニメーション、ボーナス演出、タッチ待ちといった複数の演出要素をChainの組み合わせで制御しています。

演出の流れは以下のとおりです。

1. **フェードアウト + ダイアログ表示** — `ChainParallel` でフェードとダイアログ表示を同時に実行。`ChainRace` で画面タップによるスキップにも対応
2. **ボーナスポイント演出** — 各プレイヤーのボーナスを時間差で表示。こちらも `ChainRace` でスキップ可能
3. **タッチ待ち** — `ChainButton` で画面タップを待機
4. **ダイアログ非表示 + フェードイン** — `ChainParallel` でダイアログの退場アニメーションとフェードインを同時に実行

```csharp
new ChainSequence(
    // 1. フェードアウト + ダイアログ表示（スキップ可能）
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
    // 4. ダイアログ非表示 + フェードイン
    new ChainParallel(
        resultDialog.ChainHideDialog(),
        fadePanel.ChainFade(true)
    )
)
```

Sequence、Parallel、Raceを組み合わせることで、複雑な演出フローとスキップ制御を宣言的に記述できていることがわかります。

## その他のクラス

| クラス | 説明 |
|---|---|
| `ChainAction` | 単一のアクションを実行して即完了 |
| `ChainDelay` | 指定秒数待機 |
| `ChainAnimator` | Animatorのステート再生を待機 |
| `ChainWork` | 毎フレーム更新処理を実行（`onStart` / `onUpdate` / `onSkip` イベント） <br>※柔軟ですが、これを多用するのはあまり美しくありません。 |
| `ChainHalt` | 完了しないChain（外部からのSkipでのみ終了） |
| `ChainNop` | 何もせず即完了 |

## 導入方法

### サンプルプロジェクトとして利用する場合
このリポジトリを clone またはダウンロードし、Unity でプロジェクトを開いてください。  
UniTask を含む必要なパッケージは、Unity Package Manager により自動的にインストールされます。

### 自分のプロジェクトに組み込む場合
`ChainPattern` フォルダ内のスクリプトのみを既存の Unity プロジェクトへコピーして利用する場合は、  
依存ライブラリである UniTask を事前に導入してください。

## ライセンス

[MIT License](LICENSE)
