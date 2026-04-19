using Mafi.Base;
using Mafi.Core.Factory.Transports;
using Mafi.Core.Mods;
using Mafi.Core.Ports.Io;
using Mafi.Core.Prototypes;
using Mafi.Localization;

namespace ExampleMod;

internal class FastFlatConveyorData : IModData {

	public void RegisterData(ProtoRegistrator registrator) {
		var t1 = registrator.PrototypesDb.GetOrThrow<TransportProto>(Ids.Transports.FlatConveyor);
		var t3 = registrator.PrototypesDb.GetOrThrow<TransportProto>(Ids.Transports.FlatConveyorT3);
		var portShape = registrator.PrototypesDb.GetOrThrow<IoPortShapeProto>(Ids.IoPortShapes.FlatConveyor);

		var proto = new TransportProto(
			id: ExampleModIds.Transports.FastFlatConveyor,
			strings: new Proto.Str(
				Loc.Str("FastFlatConveyor__Name", "Fast Flat Conveyor", "Name of the fast flat conveyor")),
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

		// LayoutEntityBlueprintGenerator が Upgrade.PreviousTier.IsSome でフィルタするなら、
		// T3 のアップグレード先として登録することで dict から除外され、クラッシュを防げる。
		// TierData.SetNextTierIndirect は効果がなかったため Upgrade 系 API を使う。
		t3.Upgrade.SetNextTier(proto, overwrite: false);
	}
}
