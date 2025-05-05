using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using HarmonyLib;
using System.Reflection;
using Config;
using System.Collections.Generic;
using Config.ConfigCells;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using GameData.Domains;
using GameData.Common;
using System;
using System.Reflection.Emit;
using System.Linq;
using Redzen.Random;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GameData.ArchiveData;

namespace TaiwuShopMoreItemBackend
{
    [PluginConfig("EfficientSamsara", "algebnaly", "0.2.0")]
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
    }


    [HarmonyPatch(typeof(GameData.Domains.Character.Character), "OfflineSetPreexistenceCharId")]
    public class PatchOfflineSetPreexistenceCharId
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

    [HarmonyPatch(typeof(GameData.Domains.Taiwu.TaiwuDomain), "SwitchEquipmentPlan")]
    public class PatchSwitchEquipmentPlan
    {
        public static void Prefix(TaiwuDomain __instance, DataContext context)
        {
            bool enable_add_preIds =false;
            DomainManager.Mod.GetSetting(BackendPatch.modIdStr, "enable_add_preIds", ref enable_add_preIds);
            if(enable_add_preIds){
                GameData.Domains.Character.Character taiwu = DomainManager.Character.GetElement_Objects(DomainManager.Taiwu.GetTaiwuCharId());
                FieldInfo pre_char_id_field_info = typeof(GameData.Domains.Character.Character).GetField("_preexistenceCharIds", BindingFlags.Instance | BindingFlags.NonPublic);
                var param_expr = Expression.Parameter(typeof(GameData.Domains.Character.Character), "c");
                var field_access_expr = Expression.Field(param_expr, pre_char_id_field_info);
                var get_pre_char_ids_lambda = Expression.Lambda<Func<GameData.Domains.Character.Character, PreexistenceCharIds>>(field_access_expr, param_expr).Compile();
                var para_expr_pre_char_ids = Expression.Parameter(typeof(PreexistenceCharIds), "p");
                PreexistenceCharIds pre_char_ids = get_pre_char_ids_lambda(taiwu);



                int count = pre_char_ids.Count;
                while (count < 9)
                {
                    var copy_char = DomainManager.Character.CreateTemporaryCopyOfCharacter(context, taiwu);
                    copy_char.SetOrganizationInfo(OrganizationInfo.None, context);

                    DomainManager.Character.ConvertTemporaryIntelligentCharacter(context, copy_char);
                    copy_char.SetOrganizationInfo(copy_char.GetOrganizationInfo(), context);

                    pre_char_id_field_info.SetValue(copy_char, pre_char_ids);
                    SaveCharChange(copy_char, pre_char_ids);

                    // clear relations here
                    var copy_char_id = copy_char.GetId();
                    DomainManager.Character.RemoveAllRelations(context, copy_char_id, true);


                    DomainManager.Character.MakeCharacterDead(context, copy_char, 1);//Nature Death

                    // //clean up code
                    var copy_char_grave = DomainManager.Character.GetElement_Graves(copy_char_id);
                    DomainManager.Character.PossessionRemoveWaitingReincarnationChar(context, copy_char_id);
                    DomainManager.Character.RemoveGrave(context, copy_char_grave);

                    pre_char_ids.Add(context.Random, copy_char_id);
                    count++;
                }
                pre_char_id_field_info.SetValue(taiwu, pre_char_ids);
                SaveCharChange(taiwu, pre_char_ids);
            }
        }
        private static unsafe void  SaveCharChange(GameData.Domains.Character.Character instance, PreexistenceCharIds preIds)
        {
            // Offset value is from FixedFieldInfos
            byte* pData  = OperationAdder.DynamicObjectCollection_SetFixedField(4, 0, instance.GetId(), 954U, preIds.GetSerializedSize());
            preIds.Serialize(pData);
        }
    }
}
