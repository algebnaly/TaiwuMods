using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using GameData.Domains.Item;
using HarmonyLib;
using GameData.Domains.Item.Display;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Diagnostics;
using GameData.Domains.Combat;
using GameData.Domains;
using GameData.Domains.Mod;
using GameData;


namespace CombatGetExpModify
{
    [PluginConfig("CombatGetExpModify", "algebnaly", "0.1.0")]
    public class BackendPatch : TaiwuRemakePlugin
    {
        public static int scale_factor_value = 3;
        public static int scale_area_spiritual_debt_factor_value = 10;
        public static string modIdStr;
        Harmony harmony;
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
        }
    }

    [HarmonyPatch(typeof(CombatDomain))]
    [HarmonyPatch(nameof(CombatDomain.EndCombat))]
    public class PatchGalbalConfigBackend
    {
        public static bool is_saved = false;
        public static short[] orginalCombatGetExpBase = null;
        public static void Prefix()
        {
            if (!is_saved)
            {
                orginalCombatGetExpBase = (short[])GlobalConfig.Instance.CombatGetExpBase.Clone();
                is_saved = true;
            }
            var is_success = DomainManager.Mod.GetSetting(BackendPatch.modIdStr, "combat_get_exp_scale_factor", ref BackendPatch.scale_factor_value);
            if (!is_success)
            {
                AdaptableLog.Info("战斗恩义历练获取修改: 没有能够读取设置" + BackendPatch.scale_factor_value);
            }
            for (int i = 0; i < GlobalConfig.Instance.CombatGetExpBase.Length; i++)
            {
                GlobalConfig.Instance.CombatGetExpBase[i] = (short)(orginalCombatGetExpBase[i] * BackendPatch.scale_factor_value);
            }
        }
    }

    [HarmonyPatch(typeof(CombatDomain))]
    [HarmonyPatch("GetAddAreaSpiritualDebt", typeof(int[]))]
    public class PatchSpiritualAquire
    {
        public static void Postfix(int[] enemyList, ref int __result)
        {
            var is_success = DomainManager.Mod.GetSetting(BackendPatch.modIdStr, "combat_get_area_spiritual_debt_scale_factor", ref BackendPatch.scale_area_spiritual_debt_factor_value);
            if (!is_success)
            {
                AdaptableLog.Info("战斗恩义历练获取修改: 没有能够读取设置" + BackendPatch.scale_factor_value);
            }
            __result = __result * BackendPatch.scale_area_spiritual_debt_factor_value;
        }
    }
}
