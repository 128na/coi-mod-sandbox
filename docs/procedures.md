# COI Mod 開発手順

## ビルド・デプロイ

```bash
dotnet build ExampleMod2/ExampleMod2.csproj -c Debug
```

- ビルド成功時、`%APPDATA%\Captain of Industry\Mods\ExampleMod2\` へ自動デプロイされる
- `COI_ROOT` 環境変数がゲームインストールパスを指している必要がある

## 基本構造

### エントリポイント

`ExampleMod.cs` の `RegisterPrototypes` で全データを登録する。

```csharp
public override void RegisterPrototypes(ProtoRegistrator registrator) {
    registrator.RegisterAllProducts();                        // Products をアトリビュートで一括登録
    registrator.RegisterData<SomeDataClass>();               // IModData 実装クラスを個別登録
    registrator.RegisterDataWithInterface<IResearchNodesData>(); // インターフェース経由
}
```

### データクラスの作り方

`IModData` を実装し、`RegisterData` メソッドに登録処理を書く。

```csharp
internal class MyData : IModData {
    public void RegisterData(ProtoRegistrator registrator) {
        // ここにレシピ・マシン等の登録
    }
}
```

### ID の定義

`ExampleModIds` の各 partial class に static フィールドとして定義する。
型はプロトの種類ごとに異なる（`RecipeID`、`MachineID`、`StaticEntityProto.ID` 等）。

```csharp
// レシピ
public static readonly RecipeID FastIronSmelting = Ids.Recipes.CreateId("FastIronSmelting");

// コンベア（StaticEntityProto.ID を直接 new）
public static readonly TransportID FastFlatConveyor = new StaticEntityProto.ID("ExampleMod2__FastFlatConveyor");
```

---

## レシピを追加する（既存マシン向け）

prefab 不要・Unity 作業不要で最も手軽。

```csharp
registrator.RecipeProtoBuilder
    .Start(name: "レシピ名", recipeId: MyIds.Recipes.SomeRecipe, machineId: Ids.Machines.SmeltingFurnaceT1)
    .AddInput(6, Ids.Products.IronOre)
    .AddInput(1, Ids.Products.Coal)
    .SetDuration(10.Seconds())
    .AddOutput(6, Ids.Products.MoltenIron)
    .BuildAndAdd();
```

---

## コンベアを追加する

### 制約と注意事項

- `TransportProto` に**ビルダーパターンは存在しない**。直接 `new` して `ProtosDb.Add` する。
- `IoPortShapeProto` と `TransportProto` は **1対1マッピング**。同じ `IoPortShapeProto` インスタンスを複数の `TransportProto` で共有するとゲーム起動時にクラッシュする。
  - `LayoutEntityBlueprintGenerator` が `IoPortShape → Transport` の辞書を構築するため。
- コンベアとポートの接続は **`IoPortShapeProto` のインスタンス同一性**で判定される（`LayoutChar` の一致ではない）。
  - 倉庫等の既存建物への接続には、建物が使っているポートシェイプと同一インスタンスを使う必要がある。

### クラッシュを回避しながら既存ポートシェイプを再利用する方法

`LayoutEntityBlueprintGenerator` は `Upgrade.PreviousTier.IsSome`（アップグレードチェーンに前段がある）のトランスポートを辞書から除外する。
そのため、**既存 T3 コンベアの次段として登録**することで同じポートシェイプを使いながらクラッシュを防げる。

```csharp
var t1 = registrator.PrototypesDb.GetOrThrow<TransportProto>(Ids.Transports.FlatConveyor);
var t3 = registrator.PrototypesDb.GetOrThrow<TransportProto>(Ids.Transports.FlatConveyorT3);
var portShape = registrator.PrototypesDb.GetOrThrow<IoPortShapeProto>(Ids.IoPortShapes.FlatConveyor);

var proto = new TransportProto(
    id: MyIds.Transports.FastFlatConveyor,
    strings: new Proto.Str(Loc.Str("FastFlatConveyor__Name", "Fast Flat Conveyor", "description")),
    surfaceRelativeHeight: t1.SurfaceRelativeHeight,
    maxQuantityPerTransportedProduct: t1.MaxQuantityPerTransportedProduct,
    transportedProductsSpacing: t1.TransportedProductsSpacing,
    speedPerTick: t1.SpeedPerTick * 2,        // 2倍速
    zStepLength: t1.ZStepLength,
    needsPillarsAtGround: t1.NeedsPillarsAtGround,
    canBeBuried: t1.CanBeBuried,
    tileSurfaceWhenOnGround: t1.TileSurfaceWhenOnGround,
    maxPillarSupportRadius: t1.MaxPillarSupportRadius,
    portsShape: portShape,                     // 元と同じインスタンスを使う
    baseElectricityCost: t1.BaseElectricityCost * 2,
    cornersSharpnessPercent: t1.CornersSharpnessPercent,
    allowMixedProducts: t1.AllowMixedProducts,
    isBuildable: t1.IsBuildable,
    costs: t1.Costs,
    constructionDurationPerProduct: t1.ConstructionDurationPerProduct,
    graphics: t1.Graphics                      // 見た目は T1 と同じ
);

registrator.PrototypesDb.Add(proto, lockOnInit: false);

// T3 のアップグレード先として登録 → BlueprintGenerator の辞書から除外される
t3.Upgrade.SetNextTier(proto, overwrite: false);
```

---

## ゲーム DLL から ID を調べる

```powershell
$dir = "$env:COI_ROOT\Captain of Industry_Data\Managed\"
# 依存 DLL を先に全ロード（ReflectionOnly モードが必要）
Get-ChildItem $dir -Filter '*.dll' | % { try { [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($_.FullName) | Out-Null } catch {} }

$assembly = [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($dir + 'Mafi.Base.dll')
try { $types = $assembly.GetTypes() } catch [System.Reflection.ReflectionTypeLoadException] { $types = $_.Exception.Types | ? { $_ -ne $null } }
$type = $types | ? { $_.FullName -eq 'Mafi.Base.Ids+Machines' }  # Machines / Products / Recipes / Transports 等
$type.GetFields([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static) `
    | Where-Object { $_.Name -like "*Iron*" } | ForEach-Object { $_.Name }
```

| DLL | 調べられるもの |
|---|---|
| `Mafi.Base.dll` | `Ids+Machines`, `Ids+Products`, `Ids+Recipes`, `Ids+Transports`, `Ids+IoPortShapes`, `Costs+Transports` |
| `Mafi.Core.dll` | `TransportProto`, `IoPortShapeProto` などのプロトタイプ型・フィールド構造 |
