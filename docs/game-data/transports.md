# Transports — コンベア関連（Mafi.Base.dll / Mafi.Core.dll）

## Ids.Transports

| フィールド名              | 概要                 |
| ------------------------- | -------------------- |
| `FlatConveyor`            | 固形品コンベア T1    |
| `FlatConveyorT2`          | 固形品コンベア T2    |
| `FlatConveyorT3`          | 固形品コンベア T3    |
| `FlatConveyorSorter`      | 固形品ソーター       |
| `LooseMaterialConveyor`   | ルース品コンベア T1  |
| `LooseMaterialConveyorT2` | ルース品コンベア T2  |
| `LooseMaterialConveyorT3` | ルース品コンベア T3  |
| `PipeT1` 〜 `PipeT4`      | 液体パイプ各グレード |

## Ids.IoPortShapes

| フィールド名            | 用途                       |
| ----------------------- | -------------------------- |
| `FlatConveyor`          | 固形品コンベア用ポート     |
| `LooseMaterialConveyor` | ルース品コンベア用ポート   |
| `Pipe`                  | パイプ用ポート             |
| `MoltenMetalChannel`    | 溶融金属チャンネル用ポート |
| `Shaft`                 | シャフト用ポート           |

## Costs.Transports（電力・メンテナンスコスト定数）

| フィールド名                  | 内容                           |
| ----------------------------- | ------------------------------ |
| `ConveyorT1ElectricityPer100` | T1 電力消費（100タイルあたり） |
| `ConveyorT2ElectricityPer100` | T2 電力消費                    |
| `ConveyorT3ElectricityPer100` | T3 電力消費                    |
| `ConveyorMaintenancePer100T1` | T1 メンテナンスコスト          |
| `ConveyorMaintenancePer100T2` | T2 メンテナンスコスト          |
| `ConveyorMaintenancePer100T3` | T3 メンテナンスコスト          |

## TransportProto の主要フィールド

| フィールド名                       | 型          | 意味                          |
| ---------------------------------- | ----------- | ----------------------------- |
| `SpeedPerTick`                     | RelTile1f   | アイテムの移動速度（tick毎）  |
| `ThroughputPer60`                  | Quantity    | 60tick毎のスループット        |
| `MaxQuantityPerTransportedProduct` | Quantity    | スロット1つあたりの最大積載量 |
| `TransportedProductsSpacing`       | RelTile1f   | アイテム間隔                  |
| `BaseElectricityCost`              | Electricity | 電力消費（100タイルあたり）   |
| `AllowMixedProducts`               | Boolean     | 複数種混載可否                |
| `CanBeBuried`                      | Boolean     | 地中埋設可否                  |
| `ZStepLength`                      | RelTile1i   | スロープのステップ長          |
| `MaxPillarSupportRadius`           | RelTile1i   | 支柱の最大サポート半径        |

## コンストラクタシグネチャ

ビルダーパターンなし。直接 `new` して `ProtosDb.Add` する。

```csharp
new TransportProto(
    id,
    strings,　　
    surfaceRelativeHeight,            // ThicknessTilesF
    maxQuantityPerTransportedProduct, // Quantity
    transportedProductsSpacing,       // RelTile1f
    speedPerTick,                     // RelTile1f
    zStepLength,                      // RelTile1i
    needsPillarsAtGround,             // Boolean
    canBeBuried,                      // Boolean
    tileSurfaceWhenOnGround,          // Option<TileSurface>
    maxPillarSupportRadius,           // RelTile1i
    portsShape,                       // IoPortShapeProto
    baseElectricityCost,              // Electricity
    cornersSharpnessPercent,          // Percent
    allowMixedProducts,               // Boolean
    isBuildable,                      // Boolean
    costs,                            // EntityCosts
    constructionDurationPerProduct,   // Duration
    graphics)                         // TransportProto.Gfx
```

## TransportProto.Gfx のコンストラクタ

```csharp
new TransportProto.Gfx(
    crossSectionLods,            // ImmutableArray<TransportCrossSectionLod>
    renderProducts,              // Boolean
    materialPath,                // String
    transportUvLength,           // RelTile1f
    renderTransportedProducts,   // Boolean
    soundOnBuildPrefabPath,      // String
    flowIndicator,               // Option<...>
    verticalConnectorPrefabPath, // Option<String>
    pillarAttachments,           // IReadOnlyDictionary<...>
    uvShiftY,                    // Single
    instancedRenderingData,      // Option<...>
    crossSectionRadius,          // Single
    crossSectionScale,           // Single
    usePerProductColoring,       // Boolean
    customIconPath,              // Option<String>
    useInstancedRendering,       // Boolean
    maxRenderedLod,              // Int32
    categories,                  // Nullable<...>
    canBePickedUnderground)      // Boolean
```

## prefab・マテリアルパス（Assets.Base.Transports.ConveyorUnit）

| フィールド名                     | 用途                 |
| -------------------------------- | -------------------- |
| `ConveyorUnit_mat`               | コンベアのマテリアル |
| `FlatToFlat_Straight_prefab`     | 直線セクション       |
| `FlatToFlat_Turn_prefab`         | カーブセクション     |
| `FlatToRampUp_Straight_prefab`   | スロープ（上り）直線 |
| `FlatToRampDown_Straight_prefab` | スロープ（下り）直線 |
| `Port_prefab`                    | ポート               |
| `PortEnd_prefab`                 | 終端ポート           |

## 重要な制約・注意事項

### IoPortShapeProto と TransportProto の 1対1 制約

`LayoutEntityBlueprintGenerator` が起動時に `IoPortShape → TransportProto` の辞書を構築する。
**同じ `IoPortShapeProto` インスタンスを複数の `TransportProto` で共有するとクラッシュ**（`ArgumentException: Adding already existing element key`）。

### ポート接続はインスタンス同一性で判定

倉庫などの建物は特定の `IoPortShapeProto` インスタンスを保持している。
新しいインスタンスを作っても接続できない。**元の `PortsDb.GetOrThrow` で取得したインスタンスをそのまま使う必要がある**。

### クラッシュを回避しながら既存ポートシェイプを再利用する方法

`LayoutEntityBlueprintGenerator` は `Upgrade.PreviousTier.IsSome` のトランスポートを辞書から除外する。
**既存 T3 コンベアの次段として登録**することで同じポートシェイプを使いながらクラッシュを防げる。

```csharp
// NG: TierData.SetNextTierIndirect は PreviousTier を設定しない → クラッシュ回避にならない
// OK: Upgrade.SetNextTier を使う
t3.Upgrade.SetNextTier(proto, overwrite: false);
```

### 動作する完全なパターン

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
    speedPerTick: t1.SpeedPerTick * 2,
    zStepLength: t1.ZStepLength,
    needsPillarsAtGround: t1.NeedsPillarsAtGround,
    canBeBuried: t1.CanBeBuried,
    tileSurfaceWhenOnGround: t1.TileSurfaceWhenOnGround,
    maxPillarSupportRadius: t1.MaxPillarSupportRadius,
    portsShape: portShape,
    baseElectricityCost: t1.BaseElectricityCost * 2,
    cornersSharpnessPercent: t1.CornersSharpnessPercent,
    allowMixedProducts: t1.AllowMixedProducts,
    isBuildable: t1.IsBuildable,
    costs: t1.Costs,
    constructionDurationPerProduct: t1.ConstructionDurationPerProduct,
    graphics: t1.Graphics
);

registrator.PrototypesDb.Add(proto, lockOnInit: false);
t3.Upgrade.SetNextTier(proto, overwrite: false);  // T3 の次段として登録
```
