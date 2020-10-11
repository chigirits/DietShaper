# DietShaper

アバターの腕などを細くするためのシェイプキーを追加し、新しいメッシュとして保存するUnityエディタ拡張です。

Unity上でアバターに衣装を着せるとき、姿勢によって体の各部が衣装を貫通するのを防ぐなどの用途に有用です。

![Banner](https://repository-images.githubusercontent.com/298298937/85faac80-0817-11eb-8cce-efd45fe7784d)

効果の例

![demo01](https://user-images.githubusercontent.com/61717977/95670954-979fd380-0bcc-11eb-89e7-3d16eb204919.gif)
<!-- ![demo01-001](https://user-images.githubusercontent.com/61717977/95671858-6af0b980-0bd6-11eb-89db-d24d5ebd9b3d.png)![demo01-033](https://user-images.githubusercontent.com/61717977/95671859-6c21e680-0bd6-11eb-8a10-8aa7ff4e82a7.png) -->

## 動作環境

Unity 2018.4 以降

## インポート手順

### unitypackageをインポートする方法

[Releasesページ](https://github.com/chigirits/DietShaper/releases) より最新版の `DietShaper-vX.X.X.unitypackage` をダウンロードし、Unityにインポートする

### パッケージマネージャを用いる方法

1. インポート先プロジェクトの `Packages/manifest.json` をテキストエディタ等で開く
2. `dependencies` オブジェクト内に以下の要素を追加
   
   ```
   "com.github.chigirits.dietshaper": "https://github.com/chigirits/DietShaper.git",
   ```

こちらの方法でインポートした場合、以下の説明文中で示される本パッケージのプレハブやプリセットは `Assets/Chigiri/DietShaper/...` 下ではなく `Packages/DietShaper/Assets/Chigiri/DietShaper/...` 下から選択してください。

## 使い方

1. シーンにアバターモデルを配置してください。可能な限りボーンの Transform を変更せず、TスタンスまたはAスタンスの状態であることが望ましいです。
2. 「メニュー/Chigiri/DietShaper」を選択すると、ヒエラルキーのトップレベルに DietShaper が配置されます。
3. DietShaper の `Target` に、操作対象となる SkinnedMeshRenderer（アバターの体）を指定してください。
このとき、対象にアタッチされているメッシュが `Source Mesh` に自動的にセットされます。
   
   ![usage-01](https://user-images.githubusercontent.com/61717977/95670934-8b1b7b00-0bcc-11eb-8930-72cd3dda31fb.png)
4. `Add Shape Key From Preset` のプルダウンから、追加するシェイプキーのプリセットを選択してください。最初は `All` を選択して全プリセットを追加し、それぞれの効果を試してみることをおすすめします。
   
   ![usage-02](https://user-images.githubusercontent.com/61717977/95670935-8bb41180-0bcc-11eb-99c7-d0274e21972e.png)<br>
   ![usage-03](https://user-images.githubusercontent.com/61717977/95670936-8c4ca800-0bcc-11eb-9aef-917cc3b21fec.png)
5. `Process And Save As...` ボタンを押して、生成された新しいメッシュを保存してください。
   
   ![usage-04](https://user-images.githubusercontent.com/61717977/95670937-8c4ca800-0bcc-11eb-974f-db347c8887d1.png)
   
   保存が完了すると、`Target` の `SkinnedMeshRenderer` に新しいメッシュがアタッチされます。 この差し替えられたメッシュに追加されている新しいシェイプキーの値を変更してみて、期待どおりの効果がかかることを確認してください。
   
   ![usage-05](https://user-images.githubusercontent.com/61717977/95670938-8ce53e80-0bcc-11eb-9244-df54b997e905.png)

## パラメータ

### DietShaper全体に影響するパラメータ

- `Avatar Root` : 処理対象の Humanoid アバターのルートオブジェクト。
- `Target` : 処理対象の SkinnedMeshRenderer。`Avatar Root` に含まれるボーンに関連付けられたオブジェクトを指定する必要があります。
- `Source Mesh` : オリジナルのメッシュ。`Target` を変更すると、`Target` にアタッチされているメッシュがこのフィールドに自動的に指定されます。
- `Always Show Gizmo` : チェックすると、この DietShaper がヒエラルキーで非選択状態の間もギズモを表示し続けます。

### シェイプキーごとに設定するパラメータ

- `Name` : 作成するシェイプキーの名前。
- `Radius` : 処理対象に含める範囲を表す円筒の半径。大きすぎると無関係な頂点まで変形されてしまうため、適度な値を設定する必要があります。
- `Start Margin`, `End Margin` : 変形範囲を狭くするとき、それぞれ開始ボーン・終端ボーンからの距離を比率で指定します。<br>
  - 例1 : `Start Margin = 0 / End Margin = 0`<br>
    ![margin-000-000](https://user-images.githubusercontent.com/61717977/95670949-9373b600-0bcc-11eb-9901-ef6bd93d0d4f.png)
  - 例2 : `Start Margin = 0.35 / End Margin = 0`<br>
    ![margin-350-000](https://user-images.githubusercontent.com/61717977/95670950-9373b600-0bcc-11eb-9585-f23fe18698f2.png)
  - 例3 : `Start Margin = 0 / End Margin = 0.499`<br>
    ![margin-000-499](https://user-images.githubusercontent.com/61717977/95670951-9373b600-0bcc-11eb-8a70-fc2c67000071.png)
- `Is Leaf` : 手足の先など、終端点を超えてスキンの先端まですべての頂点を処理対象に含めるときにチェックします。通常のボーンに沿うような変形とは異なり、開始点に向かって均等に縮められます。
  - 例1 : チェックなし<br>
    ![demo01](https://user-images.githubusercontent.com/61717977/95670954-979fd380-0bcc-11eb-89e7-3d16eb204919.gif)
    <!-- ![demo01-001](https://user-images.githubusercontent.com/61717977/95671858-6af0b980-0bd6-11eb-89db-d24d5ebd9b3d.png)![demo01-033](https://user-images.githubusercontent.com/61717977/95671859-6c21e680-0bd6-11eb-8a10-8aa7ff4e82a7.png) -->
  - 例2 : チェックあり<br>
    ![demo02](https://user-images.githubusercontent.com/61717977/95670955-98d10080-0bcc-11eb-8eb0-24f249fffbf6.gif)
    <!-- ![demo02-001](https://user-images.githubusercontent.com/61717977/95671860-6cba7d00-0bd6-11eb-9cf7-258870d9f558.png)![demo02-028](https://user-images.githubusercontent.com/61717977/95671861-6d531380-0bd6-11eb-9a3e-878889cc2390.png) -->
- `Shape` : 変形の形状。開始点を time=0（左端）、終端点を time=1（右端）とし、縦軸にボーンへの吸着強度（0=最大、1=変形なし）を指定します。
  - 例1<br>
    ![curve-01c](https://user-images.githubusercontent.com/61717977/95670942-91a9f280-0bcc-11eb-9cb7-78cc4393eda1.png)<br>
    ![curve-01r](https://user-images.githubusercontent.com/61717977/95670941-91115c00-0bcc-11eb-8970-1953bf9dc148.png)
  - 例2<br>
    ![curve-02c](https://user-images.githubusercontent.com/61717977/95670943-92428900-0bcc-11eb-912b-a3efbaeb2349.png)<br>
    ![curve-02r](https://user-images.githubusercontent.com/61717977/95670945-92428900-0bcc-11eb-8b78-3ffbfc668c75.png)
  - 例3<br>
    ![curve-03c](https://user-images.githubusercontent.com/61717977/95670946-92db1f80-0bcc-11eb-86e4-f3099584e315.png)<br>
    ![curve-03r](https://user-images.githubusercontent.com/61717977/95670947-92db1f80-0bcc-11eb-8056-7d4d57641318.png)
- `Add Normal` : 法線を元にした成分の影響力（単位：メートル）。通常は 0 にしてください。Shoulder プリセットで、わきの下をボーンとは垂直な方向に移動するために用います。
  - 例1 : `Add Normal = 0`<br>
    ![normal-off](https://user-images.githubusercontent.com/61717977/95670956-98d10080-0bcc-11eb-91b6-9a22fa70671f.png)
  - 例2 : `Add Normal = 0.05`<br>
    ![normal-on](https://user-images.githubusercontent.com/61717977/95670957-99699700-0bcc-11eb-9ad5-cbd2be4476a6.png)
- `Gizmo Color` : ギズモの表示色。処理内容への影響はありません。
- `Body Lines[i]` : i番目の処理対象ボディライン。左右の腕など、複数のボーンを別々に処理した結果を1つのシェイプキーにするときは2つ以上のボディラインを持ちます。ボディラインの数は変更できません。通常はプリセットの設定を変更しないでください。
  - `Bones[j]` : j番目のボーン。これらのボーンをつないだ線分または折れ線（ボディライン）に向かって周囲の頂点が吸着するように変形されます。ボーンの数は変更できません。通常はプリセットの設定を変更しないでください。
  - `X Sign Range` : 処理対象に含める頂点のX座標の符号範囲。脚など、左右で円筒が重なりやすい部分の排他処理に用います。通常はプリセットの設定を変更しないでください。

### 注意事項

- 位置的に隣り合うシェイプキーをウェイト100で重ね掛けすると、重複部分が膨張することがあります。この場合、ウェイトを67程度に調整することでほぼ平均的に細くすることができます。ただし、節ができますので服に隠れることが前提となります。

## ライセンス

[MIT License](./LICENSE)

- このソフトウェアは商用・非商用問わず無償で利用できますが、無保証です。利用に際して発生する問題については、作者は一切の責任を負いません。
- コードを再利用する際は、著作権表示と上記リンク先のライセンス条文を同梱する必要があります。詳しくは条文（英語）をお読みください。

## 更新履歴

- v1.0.0
  - 初回リリース
