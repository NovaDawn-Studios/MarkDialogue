using NovaDawnStudios.MarkDialogue.Data;
using System.Collections.Generic;

namespace NovaDawnStudios.MarkDialogue.Interfaces
{
    public interface IMDRunnerState
    {
        List<MDLink> Choices { get; }
        MDScriptCollectionAsset Collection { get; }
        MDCharacter CurrentCharacter { get; }
        int CurrentScriptLineNumber { get; }
        bool IsComplete { get; }
        MDBaseRunner Player { get; }
        MDScriptAsset Script { get; }
        IMDVariableStore VariableStore { get; }

        string CreateLoggingString(string message);
        string ResolveScriptPath(string relativePath);
        string SkipToNextTag(string[] targetTags, string[] scopeStartTags = null, string[] scopeEndTags = null);
        void StartNewScript(MDScriptAsset newScript, MDScriptCollectionAsset newCollection = null);
    }
}