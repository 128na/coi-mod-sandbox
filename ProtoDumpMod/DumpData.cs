using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mafi;
using Mafi.Core.Factory.Machines;
using Mafi.Core.Factory.Recipes;
using Mafi.Core.Factory.Transports;
using Mafi.Core.Mods;
using Mafi.Core.Ports.Io;
using Mafi.Core.Products;
using Mafi.Core.Research;

namespace ProtoDumpMod;

internal class DumpData : IModData {

	public void RegisterData(ProtoRegistrator r) {
		WriteJson("transports.json", DumpTransports(r));
		WriteJson("products.json", DumpProducts(r));
		WriteJson("machines.json", DumpMachines(r));
		WriteJson("recipes.json", DumpRecipes(r));
		WriteJson("io_port_shapes.json", DumpIoPortShapes(r));
		WriteJson("research.json", DumpResearch(r));
		Log.Info("ProtoDumpMod: dump complete -> " + ProtoDumpMod.OutputDir);
	}

	private static void WriteJson(string filename, string json) {
		var path = Path.Combine(ProtoDumpMod.OutputDir, filename);
		File.WriteAllText(path, json, Encoding.UTF8);
	}

	private static string DumpTransports(ProtoRegistrator r) {
		var rows = r.PrototypesDb.Filter<TransportProto>(_ => true).Select(p => new Dictionary<string, object> {
			["id"] = p.Id.ToString(),
			["speedPerTick"] = p.SpeedPerTick.ToString(),
			["maxQtyPerProduct"] = p.MaxQuantityPerTransportedProduct.ToString(),
			["transportedProductsSpacing"] = p.TransportedProductsSpacing.ToString(),
			["baseElectricityCost"] = p.BaseElectricityCost.ToString(),
			["canBeBuried"] = p.CanBeBuried,
			["allowMixedProducts"] = p.AllowMixedProducts,
			["isBuildable"] = p.IsBuildable,
			["needsPillarsAtGround"] = p.NeedsPillarsAtGround,
		});
		return ToJsonArray(rows);
	}

	private static string DumpProducts(ProtoRegistrator r) {
		var rows = r.PrototypesDb.Filter<ProductProto>(_ => true).Select(p => new Dictionary<string, object> {
			["id"] = p.Id.ToString(),
			["type"] = p.Type.ToString(),
			["isStorable"] = p.IsStorable,
			["isWaste"] = p.IsWaste,
			["isRecyclable"] = p.IsRecyclable,
			["maxQtyPerTransport"] = p.MaxQuantityPerTransportedProduct.ToString(),
		});
		return ToJsonArray(rows);
	}

	private static string DumpMachines(ProtoRegistrator r) {
		var rows = r.PrototypesDb.Filter<MachineProto>(_ => true).Select(p => new Dictionary<string, object> {
			["id"] = p.Id.ToString(),
			["electricityConsumed"] = p.ElectricityConsumed.ToString(),
			["recipeCount"] = p.Recipes.Count,
		});
		return ToJsonArray(rows);
	}

	private static string DumpRecipes(ProtoRegistrator r) {
		var rows = r.PrototypesDb.Filter<RecipeProto>(_ => true).Select(p => new Dictionary<string, object> {
			["id"] = p.Id.ToString(),
			["duration"] = p.Duration.ToString(),
			["inputs"] = p.AllInputs.Select(i => i.ToString()).ToArray(),
			["outputs"] = p.AllOutputs.Select(o => o.ToString()).ToArray(),
		});
		return ToJsonArray(rows);
	}

	private static string DumpIoPortShapes(ProtoRegistrator r) {
		var rows = r.PrototypesDb.Filter<IoPortShapeProto>(_ => true).Select(p => new Dictionary<string, object> {
			["id"] = p.Id.ToString(),
		});
		return ToJsonArray(rows);
	}

	private static string DumpResearch(ProtoRegistrator r) {
		var rows = r.PrototypesDb.Filter<ResearchNodeProto>(_ => true).Select(p => new Dictionary<string, object> {
			["id"] = p.Id.ToString(),
		});
		return ToJsonArray(rows);
	}

	private static string ToJsonArray(IEnumerable<Dictionary<string, object>> rows) {
		var sb = new StringBuilder();
		sb.AppendLine("[");
		var list = rows.ToList();
		for (int i = 0; i < list.Count; i++) {
			sb.Append("  {");
			var entries = list[i].ToList();
			for (int j = 0; j < entries.Count; j++) {
				sb.Append($" {JsonPair(entries[j].Key, entries[j].Value)}");
				if (j < entries.Count - 1) sb.Append(",");
			}
			sb.Append(" }");
			if (i < list.Count - 1) sb.Append(",");
			sb.AppendLine();
		}
		sb.Append("]");
		return sb.ToString();
	}

	private static string JsonPair(string key, object value) {
		var k = $"\"{Esc(key)}\"";
		string v;
		if (value is bool b) v = b ? "true" : "false";
		else if (value is int n) v = n.ToString();
		else if (value is string[] arr) v = "[" + string.Join(", ", arr.Select(s => $"\"{Esc(s)}\"")) + "]";
		else v = $"\"{Esc(value?.ToString() ?? "")}\"";
		return $"{k}: {v}";
	}

	private static string Esc(string s) =>
		s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
}
