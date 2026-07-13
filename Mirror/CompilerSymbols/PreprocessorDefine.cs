using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mirror
{
    static class PreprocessorDefine
    {
        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void AddDefineSymbols()
        {
            // Art/minimal slice (no Assets/Private/Scripts): do not force-add bare MIRROR.
            // PrivateModuleConfigurator strips it for that mode; re-adding here caused an
            // infinite define-change → domain-reload loop.
            bool privateGameplayPresent = Directory.Exists(
                Path.Combine(Application.dataPath, "Private", "Scripts"));

            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var defines = new HashSet<string>(
                currentDefines.Split(';').Where(d => !string.IsNullOrWhiteSpace(d)));

            if (privateGameplayPresent)
                defines.Add("MIRROR");

            string[] versionDefines =
            {
                "MIRROR_17_0_OR_NEWER",
                "MIRROR_18_0_OR_NEWER",
                "MIRROR_24_0_OR_NEWER",
                "MIRROR_26_0_OR_NEWER",
                "MIRROR_27_0_OR_NEWER",
                "MIRROR_28_0_OR_NEWER",
                "MIRROR_29_0_OR_NEWER",
                "MIRROR_30_0_OR_NEWER",
                "MIRROR_30_5_2_OR_NEWER",
                "MIRROR_32_1_2_OR_NEWER",
                "MIRROR_32_1_4_OR_NEWER",
                "MIRROR_35_0_OR_NEWER",
                "MIRROR_35_1_OR_NEWER",
                "MIRROR_37_0_OR_NEWER",
                "MIRROR_38_0_OR_NEWER",
                "MIRROR_39_0_OR_NEWER",
                "MIRROR_40_0_OR_NEWER",
                "MIRROR_41_0_OR_NEWER",
                "MIRROR_42_0_OR_NEWER",
                "MIRROR_43_0_OR_NEWER",
                "MIRROR_44_0_OR_NEWER",
                "MIRROR_46_0_OR_NEWER",
                "MIRROR_47_0_OR_NEWER",
                "MIRROR_53_0_OR_NEWER",
                "MIRROR_55_0_OR_NEWER",
                "MIRROR_57_0_OR_NEWER",
                "MIRROR_58_0_OR_NEWER",
                "MIRROR_65_0_OR_NEWER"
            };
            foreach (string define in versionDefines)
                defines.Add(define);

            // Preserve existing order; append only missing symbols so we don't
            // rewrite PlayerSettings (and trigger domain reload) every launch.
            var ordered = currentDefines.Split(';')
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .ToList();
            if (!privateGameplayPresent)
                ordered.RemoveAll(d => d == "MIRROR");
            foreach (string define in defines)
            {
                if (!ordered.Contains(define))
                    ordered.Add(define);
            }

            string newDefines = string.Join(";", ordered);
            if (newDefines != currentDefines)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
            }
        }
    }
}
