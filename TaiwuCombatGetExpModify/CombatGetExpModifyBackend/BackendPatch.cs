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


namespace CombatGetExpModify
{
    [PluginConfig("CombatGetExpModify", "在下炮灰", "0.1.0")]
    public class BackendPatch: TaiwuRemakePlugin
    {
        public static int scale_factor_value = 30;
        public static string modIdStr;
        Harmony harmony;
        public override void Dispose()
        {
            if(harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            harmony = new Harmony("com.paohui.mod");
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
                BackendPatch.scale_factor_value= 30;
                AdaptableLog.Info("战斗历练获取修改: 没有能够读取设置" + BackendPatch.scale_factor_value);
            }
            double scale_factor = BackendPatch.scale_factor_value/10.0;
            for (int i = 0; i < GlobalConfig.Instance.CombatGetExpBase.Length; i++)
            {
                GlobalConfig.Instance.CombatGetExpBase[i]=(short)(orginalCombatGetExpBase[i] * scale_factor);
            }
        }
    }
}
