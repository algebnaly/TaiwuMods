using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;
using GameData.Domains.Item.Display;

namespace BookRepairEnhanceFrontend
{
    [PluginConfig("BookRepairEnhance", "在下炮灰", "0.1.0")]
    public class FrontendPatch: TaiwuRemakePlugin
    {
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
            harmony = new Harmony("com.paohui.mod");
            harmony.PatchAll();
            Debug.Log("启动 藏书阁增强 前端Mod");
        }
    }
    [HarmonyPatch(typeof(SkillBookPageDisplayData))]
    [HarmonyPatch(nameof(SkillBookPageDisplayData.GetFixProgress))]
    public class PatchGalbalConfigFrontend
    {
        public static void Prefix()//前端和后端都有一个GlobalConfig所以为了保证前端显示正常, 这里也需要修改FixBookTotalProgress
        {
            GlobalConfig.Instance.FixBookTotalProgress = new short[]
            {
                25,
                50,
                100,
                150,
                200,
                300,
                500,
                750,
                1000,
            };
        }
    }
}
