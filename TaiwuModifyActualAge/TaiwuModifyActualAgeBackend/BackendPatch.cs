using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using HarmonyLib;
using GameData.Domains.Character;
using GameData.Domains;
using TaiwuModdingLib.Core.Utils;

namespace TaiwuModifyActualAgeBackend
{
    [PluginConfig("TaiwuModifyActualAge", "在下炮灰", "0.1.0")]
    public class BackendPatch: TaiwuRemakePlugin
    {
        Harmony harmony;
        public static string modIdStr;
        public static short want_age;
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
            AdaptableLog.Info("修改实际年龄 后端Mod 启动了");
        }
    }

    [HarmonyPatch(typeof(Character), "GetActualAge")]
    public class PatchBackend {
        public static void Prefix() {
            int want_age_set = 1;
            var success = DomainManager.Mod.GetSetting(BackendPatch.modIdStr, "want_age", ref want_age_set);
            BackendPatch.want_age = (short)want_age_set;
            if (!success)
            {
                BackendPatch.want_age = 32767;
            }
            DomainManager.Taiwu.GetTaiwu().ModifyField("_actualAge", BackendPatch.want_age);
        }
    }
}
