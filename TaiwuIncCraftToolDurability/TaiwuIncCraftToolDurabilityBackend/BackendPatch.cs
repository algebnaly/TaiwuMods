using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using GameData.Domains.Building;
using HarmonyLib;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using Config;
using System.Collections.Generic;
using TaiwuModdingLib.Core.Utils;

namespace TaiwuIncCraftToolDurabilityBackend
{
    [PluginConfig("TaiwuIncCraftToolDurability", "在下炮灰", "0.1.0")]
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

            AdaptableLog.Info("提升制造工具的最大耐久 后端Mod 启动了");
        }
    }

    [HarmonyPatch(typeof(CraftTool), "GetItem")]
    public class PatchGalbalConfigBackend
    {
        static bool is_modified = false;
        public static void Prefix()
        {
            if (!is_modified)
            {
                try
                {
                    if(CraftTool.Instance == null)
                    {
                        return;
                    }
                    List<CraftToolItem>  _dataArray = (List<CraftToolItem>)typeof(CraftTool).GetField("_dataArray", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(CraftTool.Instance);
                    if(_dataArray == null)
                    {
                        return;
                    }
                    if(_dataArray.Count >= 54)
                    {
                        for(int i = 0; i < 54; i++)
                        {
                            if((i % 9) < 5)
                            {
                                _dataArray[i].ModifyField("MaxDurability", (short)(3 * _dataArray[i].MaxDurability));
                            }
                        }
                    }
                }catch(System.Reflection.TargetInvocationException)
                {
                    AdaptableLog.Info("exception in reflection 制造工具耐久MOD");
                }
                is_modified = true;
            }
        }
    }
}
