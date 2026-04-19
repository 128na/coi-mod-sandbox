using Mafi;
using Mafi.Base;
using Mafi.Core.Mods;

namespace ExampleMod;

internal class FastIronSmeltingData : IModData {

	public void RegisterData(ProtoRegistrator registrator) {
		// SmeltingFurnaceT1 向けに、IronSmeltingT1Coal の2倍速バリアント
		// 元レシピ: IronOre x6 + Coal x1 → MoltenIron x6 / 20秒
		// このレシピ: 同じ投入量 → 同じ産出量 / 10秒（2倍速）
		registrator.RecipeProtoBuilder
			.Start(
				name: "Fast Iron Smelting",
				recipeId: ExampleModIds.Recipes.FastIronSmelting,
				machineId: Ids.Machines.SmeltingFurnaceT1)
			.AddInput(6, Ids.Products.IronOre)
			.AddInput(1, Ids.Products.Coal)
			.SetDuration(10.Seconds())
			.AddOutput(6, Ids.Products.MoltenIron)
			.BuildAndAdd();
	}
}
