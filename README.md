# coi-mod-sandbox

Captain of Industry (COI) のmod開発学習用リポジトリ。
既存アイテムの改造や新要素の追加を試しながらmodding手法を習得することを目的とする。

## 前提条件

- [Captain of Industry](https://store.steampowered.com/app/1594320/Captain_of_Industry/) をSteamでインストール済みであること
- .NET Framework 4.8 がインストールされていること
- .NET 10 SDK がインストールされていること
- 環境変数 `COI_ROOT` にゲームのインストールパスを設定していること
  - 例: `C:\Steam\steamapps\common\Captain of Industry`

## プロジェクト構成

```
ExampleMod2/
  manifest.json       mod のメタ情報
  config.json         プレイヤー設定オプション
  ExampleMod2.csproj  プロジェクトファイル
  ExampleMod2.slnx    ソリューションファイル
  *.cs                ソースコード
```

## ビルド

```bash
cd ExampleMod2
dotnet build ExampleMod2.slnx -c Release
```

> **注意**: `dotnet build` を使用すること。Visual Studio の MSBuild は環境によって動作しない場合がある。

### ビルド成果物

```
ExampleMod2/bin/Release/net48/ExampleMod2.dll
```

## 自動デプロイ

ビルド時に自動的にゲームのmodsフォルダへデプロイされる。

**デプロイ先:**

```
%APPDATA%\Captain of Industry\Mods\ExampleMod2\
```

デプロイされるファイル:

| ファイル | 説明 |
|---|---|
| `ExampleMod2.dll` | コンパイル済みmod本体 |
| `manifest.json` | modのメタ情報 |
| `config.json` | プレイヤー設定オプション |
| `readme.txt` | インストール手順 |

デプロイを無効にする場合は `ExampleMod2.csproj` の `DeployToModsFolder` を `false` に変更する。

## 参考

- [Captain of Industry modding (公式テンプレート)](https://github.com/pvilim/captain-of-industry-modding)
- [COI Discord #modding-dev-general](https://discord.gg/JxmUbGsNRU)
- ゲームのログ: `%APPDATA%\Captain of Industry\Logs`
