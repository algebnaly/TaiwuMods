using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using GameData.Domains.Item;
using HarmonyLib;
using GameData.Domains.Item.Display;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Diagnostics;
using GameData.Domains.Taiwu;
using GameData.Domains;
using GameData.Domains.Mod;
using GameData;
using System.Reflection;
using System;

namespace ModifyCardPool
{
    [PluginConfig("ModifyCardPool", "algebnaly", "0.1.0")]
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

    [HarmonyPatch(typeof(TaiwuDomain))]
    [HarmonyPatch("GetRandomAvailableLegacy")]
    public class PatchSpiritualAquire
    {
        public static void Prefix(sbyte groupId, ref short __result)
        {
            if(groupId < 0){
                return;
            }
            var taiwu_domain_type = typeof(TaiwuDomain);
            FieldInfo fieldInfo = taiwu_domain_type.GetField("_categorizedNormalLegacyConfigs",
                BindingFlags.NonPublic | BindingFlags.Static);
            sbyte worldCreationGroupLevel = TaiwuDomain.GetWorldCreationGroupLevel(groupId);
            if (fieldInfo != null)
            {
                var value = (List<ValueTuple<short, short>>[][])fieldInfo.GetValue(null); // 静态成员传入null
                for (int j = 0; j < 3; j++)
                {
                    var legacy_list = value[j][worldCreationGroupLevel];
                    for (int i = 0; i < legacy_list.Count; i++)
                    {
                        (short, short) legacy_value = legacy_list[i];
                        var template_id = legacy_value.Item1;
                        var weight = legacy_value.Item2;
                        var legacy_item = Config.Legacy.Instance[template_id];
                        if (legacy_item.Name == "寿元激发" || legacy_item.Name == "魅力激发")
                        {
                            legacy_list[i] = (template_id, 1);
                        }
                        else if (legacy_item.Name.Contains("弃舍"))
                        {
                            legacy_list[i] = (template_id, 0);
                        }
                        else if (legacy_item.Name.Contains("恩义"))
                        {
                            legacy_list[i] = (template_id, 0);
                        }
                        else if (legacy_item.Name.Contains("奇才"))
                        {
                            legacy_list[i] = (template_id, 1);
                        }
                        else if (legacy_item.Name.Contains("激发"))
                        {
                            legacy_list[i] = (template_id, 10);
                        } else if (legacy_item.Cost < 0)
                        {
                            legacy_list[i] = (template_id, 0);
                        } else if (legacy_item.Name == "传功铸气")
                        {
                            legacy_list[i] = (template_id, 1);
                        }
                    }
                }
            }
        }
    }
}
