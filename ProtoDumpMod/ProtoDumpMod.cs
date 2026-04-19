using Mafi;
using Mafi.Collections;
using Mafi.Core;
using Mafi.Core.Mods;

namespace ProtoDumpMod;

public sealed class ProtoDumpMod : DataOnlyMod {

	internal static string OutputDir = "";

	public ProtoDumpMod(ModManifest manifest) : base(manifest) {
		OutputDir = manifest.RootDirectoryPath;
		Log.Info($"ProtoDumpMod: output dir = {OutputDir}");
	}

	public override void RegisterPrototypes(ProtoRegistrator registrator) {
		registrator.RegisterData<DumpData>();
	}

	public override void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues) { }
}
