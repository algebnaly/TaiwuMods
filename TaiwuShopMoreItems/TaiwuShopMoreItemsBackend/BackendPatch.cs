using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using GameData.Domains.Building;
using HarmonyLib;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Reflection;
using Config;
using System.Collections.Generic;
using TaiwuModdingLib.Core.Utils;
using GameData.Domains.Merchant;
using System.Reflection.Emit;
using Config.ConfigCells;
using System.Diagnostics;
using GameData.Domains.Item;
using GameData.Domains;
using HarmonyLib.Tools;
using System;

namespace TaiwuShopMoreItemBackend
{
    [PluginConfig("TaiwuShopMoreItems", "algebnaly", "0.1.0")]
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
            AdaptableLog.Info("提升商店物品数量 后端Mod 启动了");
        }
    }

    [HarmonyPatch(typeof(MerchantData), "GenerateGoodsLevelList")]
    public class PatchMerchantInstance
    {
        public static bool is_modified = false;
        public static void Prefix()
        {
            if (Merchant.Instance == null)
            {
                return;
            }
            if (!is_modified)
            {
                //读取Mod设置
                var is_success = DomainManager.Mod.GetSetting(BackendPatch.modIdStr, "rate_factor", ref BackendPatch.rate_factor);
                if (!is_success)
                {
                    BackendPatch.rate_factor = 1;
                    AdaptableLog.Info("没能成功读取rate_factor设置");
                }
                is_success = DomainManager.Mod.GetSetting(BackendPatch.modIdStr, "stack_factor", ref BackendPatch.stack_factor);
                if (!is_success)
                {
                    BackendPatch.rate_factor = 3;
                    AdaptableLog.Info("没能成功读取stack_factor设置");
                }
                short rate_factor = (short)BackendPatch.rate_factor;
                sbyte stack_factor = (sbyte)BackendPatch.stack_factor;
                foreach (MerchantItem merchantItem in Merchant.Instance)
                {
                    for (int i = 0; i < merchantItem.GoodsRate.Length; i++)
                    {
                        var target_rate = merchantItem.GoodsRate[i] * rate_factor;
                        merchantItem.GoodsRate[i] = (short)Math.Min(target_rate, short.MaxValue);
                    }

                    var goods_list = new List<List<PresetItemTemplateIdGroup>> {
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods0),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods1),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods2),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods3),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods4),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods5),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods6),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods7),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods8),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods9),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods10),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods11),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods12),
                        new List<PresetItemTemplateIdGroup>(merchantItem.Goods13),
                        };
                    foreach (var goods in goods_list)
                    {
                        for (int j = 0; j < goods.Count; j++)
                        {
                            sbyte grp_len = goods[j].GroupLength;
                            //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                            if (ItemTemplateHelper.IsStackable(
                                goods[j].ItemType, goods[j].StartId
                                ))
                            {
                                var target_num = (int)grp_len * stack_factor;
                                if (grp_len >= 0)//sbyte 的最大值为127
                                {
                                    grp_len = (sbyte)Math.Min(target_num, sbyte.MaxValue);
                                }
                            }

                            var type_name = ItemType.TypeId2TypeName[goods[j].ItemType];
                            var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, goods[j].StartId, grp_len);
                            goods[j] = new_preset_template_grp;
                        }
                    }
                    for (int i = 0; i < goods_list.Count;i++)
                    {
                        var field_info = typeof(MerchantItem).GetField("Goods"+i, BindingFlags.Public | BindingFlags.Instance);
                        if(field_info == null){
                            continue;
                        }
                        field_info.SetValue(merchantItem, goods_list[i]);
                    }
                }
                is_modified = true;
            }
        }
    }
}
