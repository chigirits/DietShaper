# WORK IN PROGRESS

# DietShaper

アバターの腕などを細くするためのシェイプキーを追加し、新しいメッシュとして保存するUnityエディタ拡張です。

Unity上でアバターに衣装を着せるとき、姿勢によって体の各部が衣装を貫通するのを防ぐなどの用途に有用です。

![Banner](https://repository-images.githubusercontent.com/298298937/85faac80-0817-11eb-8cce-efd45fe7784d)

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

(執筆中)


### 注意事項

- 隣り合うシェイプキーを100%で重ね掛けすると、重複部分が膨張することがあります。この場合、67％程度に調整することでほぼ平均的に細くすることができますが、節ができますので服に隠れることが前提となります。

## ライセンス

[MIT License](./LICENSE)

<!-- ## 更新履歴 -->
