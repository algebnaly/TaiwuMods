using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using GameData.Domains.Building;
using HarmonyLib;


namespace BookRepairEnhanceBackend
{
    [PluginConfig("BookRepairEnhance", "在下炮灰", "0.1.0")]
    public class BackendPatch: TaiwuRemakePlugin
    {
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

            AdaptableLog.Info("藏书阁增强 后端Mod 启动了");
        }
    }

    [HarmonyPatch(typeof(BuildingDomain))]
    [HarmonyPatch(nameof(BuildingDomain.GetFixBookProgress))]
    public class PatchGalbalConfigBackend
    {
        public static void Prefix()//每次调用GetFixBookProgress都修改一次FixBookTotalProgress当然很低效, 不过我没有找到Patch GlobalConfig构造函数的办法
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
