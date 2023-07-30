#nullable enable

using NovaDawnStudios.MarkDialogue.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     An example variable store for MarkDialogue. This store will persist for the lifetime of your game, but variables will be lost when the
    ///     game shuts down. Use this as an example to implement your own store with persistance if required.
    /// </summary>
    public class MarkDialogueVariableStore : MonoBehaviour, IMDVariableStore
    {
        public static MarkDialogueVariableStore? Instance { get; private set; }

        public static MarkDialogueVariableStore CreateInstanceIfRequired()
        {
            if (Instance != null)
            {
                return Instance;
            }

            var go = new GameObject("MarkDialogue Variables");
            Instance = go.AddComponent<MarkDialogueVariableStore>();
            return Instance;
        }

        [field: SerializeField] public Dictionary<string, string> Variables = new(StringComparer.OrdinalIgnoreCase);
        [field: SerializeField] public Dictionary<string, int> ScriptVisits = new(StringComparer.OrdinalIgnoreCase);

        void OnEnable()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"Tried to enable a new {nameof(MarkDialogueVariableStore)}, but an instance already exists in the scene!", this);
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <inheritdoc/>
        public string? GetMarkDialogueVariable(string variableName)
        {
            if (Variables.TryGetValue(variableName, out var result))
            {
                return result;
            }

            return null;
        }

        /// <inheritdoc/>
        public void SetMarkDialogueVariable(string variableName, string? value)
        {
            if (value == null)
            {
                Variables.Remove(variableName);
            }
            else
            {
                Variables[variableName] = value;
            }
        }

        /// <inheritdoc/>
        public void SetMarkDialogueScriptVisited(string scriptAssetPath)
        {
            if (ScriptVisits.ContainsKey(scriptAssetPath))
            {
                ++ScriptVisits[scriptAssetPath];
            }
            else
            {
                ScriptVisits[scriptAssetPath] = 1;
            }
        }

        /// <inheritdoc/>
        public int GetMarkDialogueScriptVisitCount(string scriptAssetPath)
        {
            if (ScriptVisits.TryGetValue(scriptAssetPath, out int count))
            {
                return count;
            }

            return 0;
        }
    }
}
