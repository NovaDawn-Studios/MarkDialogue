#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using UnityEditor.AssetImporters;

namespace NovaDawnStudios.MarkDialogue.Editor
{
    [ScriptedImporter(1, "dlg.md")]
    public class MDImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var collection = MDScriptCollectionAsset.ParseScript(ctx.assetPath);
            ctx.AddObjectToAsset("scriptcollection", collection);
            ctx.SetMainObject(collection);

            foreach (var script in collection.Scripts)
            {
                ctx.AddObjectToAsset($"script_{script.name}", script);
            }
        }
    }
}
