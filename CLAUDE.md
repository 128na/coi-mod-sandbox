# CLAUDE.md — COI Mod 開発ナレッジインデックス

Captain of Industry (COI) mod 開発プロジェクト。学習目的で既存アイテムの改造や新要素追加を試す。

## ドキュメント構成

| ファイル | 内容 |
|---|---|
| [docs/procedures.md](docs/procedures.md) | ビルド手順・データクラスの作り方・レシピ追加・コンベア追加パターン |
| [docs/game-data/machines.md](docs/game-data/machines.md) | Ids.Machines — マシン ID 一覧 |
| [docs/game-data/products.md](docs/game-data/products.md) | Ids.Products — 素材 ID 一覧 |
| [docs/game-data/recipes.md](docs/game-data/recipes.md) | Ids.Recipes — レシピ ID 一覧 |
| [docs/game-data/transports.md](docs/game-data/transports.md) | TransportProto 仕様・制約・動作パターン |
| [docs/sessions/](docs/sessions/) | 作業セッションログ（日付別） |

## 実装済みコンテンツ

| ファイル | 内容 |
|---|---|
| `FastIronSmeltingData.cs` | SmeltingFurnaceT1 向け 2 倍速 Iron 製錬レシピ |
| `FastFlatConveyorData.cs` | FlatConveyor の 2 倍速バリアント（T3 の次段として登録） |
| `ExampleMachineData.cs` | カスタムマシン + レシピのサンプル（テンプレート由来） |

## 環境

- `COI_ROOT` 環境変数: `F:\SteamLibrary\steamapps\common\Captain of Industry`
- デプロイ先: `%APPDATA%\Captain of Industry\Mods\ExampleMod2\`
