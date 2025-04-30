using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using HarmonyLib;
using System.Reflection;
using Config;
using System.Collections.Generic;
using Config.ConfigCells;
using GameData.Domains.Character;
using GameData.Domains;
using GameData.Common;
using System;
using System.Reflection.Emit;
using System.Linq;
using Redzen.Random;

namespace TaiwuShopMoreItemBackend
{
    [PluginConfig("EfficientSamsara", "algebnaly", "0.1.0")]
    public class BackendPatch : TaiwuRemakePlugin
    {
        Harmony harmony;
        public static int rate_factor = 1;
        public static int stack_factor = 3;
        public static string modIdStr;
        public override void Dispose()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            harmony = new Harmony("com.algebnaly.mod");
            harmony.PatchAll();
            modIdStr = this.ModIdStr;
            AdaptableLog.Info("高效轮回 后端Mod 启动了");
        }
    }

    [HarmonyPatch(typeof(CharacterDomain), "RecordDeletedFromOthersPreexistence")]
    public class PatchRecordDeletedFromOthersPreexistence
    {
        public static unsafe bool Prefix(CharacterDomain __instance, DataContext context, ref PreexistenceCharIds preexistenceCharIds)
        {
            int count = preexistenceCharIds.Count;
            if (count == 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    AdaptableLog.Info($"删除前轮回角色ID: {preexistenceCharIds.CharIds[i]}");
                }
                fixed (int* ptr = preexistenceCharIds.CharIds)
                {
                    int charId = ptr[0];
                    DeadCharDeletionState state;
                    var typeinfo = typeof(CharacterDomain);
                    FieldInfo field = typeinfo.GetField("_deadCharDeletionStates", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field == null)
                    {
                        AdaptableLog.Error("错误: Field _deadCharDeletionStates not found");
                    }
                    Dictionary<int, DeadCharDeletionState> _deadCharDeletionStates = (Dictionary<int, DeadCharDeletionState>)field.GetValue(__instance);
                    bool flag = _deadCharDeletionStates.TryGetValue(charId, out state);
                    if (flag)
                    {
                        state.DeletedFromOthersPreexistence = true;
                        MethodInfo method = typeinfo.GetMethod("SetElement_DeadCharDeletionStates", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (method == null)
                        {
                            AdaptableLog.Error("错误: Method SetElement_DeadCharDeletionStates not found");
                        }
                        method.Invoke(__instance, new object[] { charId, state, context });
                    }
                    else
                    {
                        state.DeletedFromOthersPreexistence = true;
                        MethodInfo method = typeinfo.GetMethod("AddElement_DeadCharDeletionStates", BindingFlags.Instance | BindingFlags.NonPublic);
                        method.Invoke(__instance, new object[] { charId, state, context });
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        ptr[i] = ptr[i + 1];
                    }
                    preexistenceCharIds.Count = 8;
                    return false;
                }
            }
            return true;
        }
        // public static unsafe void Postfix(CharacterDomain __instance, DataContext context, ref PreexistenceCharIds preexistenceCharIds)
        // {
        //     fixed (int* ptr = preexistenceCharIds.CharIds) { 
        //         AdaptableLog.Info("Postfix of RecordDeletedFromOthersPreexistence count: " + preexistenceCharIds.Count);
        //         for (int i = 0; i < 9; i++)
        //         {
        //             AdaptableLog.Info("charId: " + ptr[i]);
        //         }
        //     }
        // }
    }

    [HarmonyPatch(typeof(GameData.Domains.Character.Character), "OfflineSetPreexistenceCharId")]
    public class Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method_add = typeof(PreexistenceCharIds).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            var fld_preexists_ids = typeof(GameData.Domains.Character.Character).GetField("_preexistenceCharIds", BindingFlags.Instance | BindingFlags.NonPublic);
            var fld_random = typeof(DataContext).GetField("Random", BindingFlags.Public | BindingFlags.Instance);
            var type_preexistscharids = typeof(GameData.Domains.Character.PreexistenceCharIds);
            var codes = new List<CodeInstruction>(instructions);
            CodeInstruction[] target_inst_list = {
                new(OpCodes.Ldarg_0, null),
                new(OpCodes.Ldflda, fld_preexists_ids),
                new(OpCodes.Ldarg_1, null),
                new(OpCodes.Ldfld, fld_random),
                new(OpCodes.Ldarg_2, null),
                new(OpCodes.Call, method_add)
            };
            int target_len = target_inst_list.Length;

            var indices = Enumerable.Range(0, (int)(codes.Count - target_len + 1))
                                    .Where(i => codes.Skip(i)
                                            .Take(target_len)
                                            .Zip(target_inst_list, (x, y) => x.opcode == y.opcode && x.operand == y.operand).All(match => match))
                                    .ToList();
            if (indices.Count == 0)
            {
                AdaptableLog.Info("Failed to find a match, patch failed!");
                return instructions;
            }
            if (indices.Count > 1)
            {
                AdaptableLog.Info("Found multiple match, patch failed!");
                return instructions;
            }
            var start_pos = indices.First();
            var new_codes = new List<CodeInstruction>(codes);

            List<CodeInstruction> assign_codes = new List<CodeInstruction>{
                new(OpCodes.Ldarg_0, null),// load 'this'
                new(OpCodes.Ldloc_1, null),//load ref preexistsCharIds,
                new(OpCodes.Ldobj, type_preexistscharids),//load preexistsCharIds
                new(OpCodes.Stfld, fld_preexists_ids),
            };
            new_codes.InsertRange(start_pos, assign_codes);
            return new_codes;
        }
    }
}
