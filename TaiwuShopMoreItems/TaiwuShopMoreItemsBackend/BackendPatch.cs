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

namespace TaiwuIncCraftToolDurabilityBackend
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

    [HarmonyPatch(typeof(MerchantDomain), "GetMaxLevelCaravanMerchantData")]
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
                foreach (MerchantItem merchantItem in ((IEnumerable<MerchantItem>)Merchant.Instance))
                {
                    for (int i = 0; i < merchantItem.GoodsRate.Length; i++)
                    {
                        merchantItem.GoodsRate[i] *= rate_factor;
                    }
                    for (int j = 0; j < merchantItem.Goods0.Count; j++)
                    {

                        sbyte grp_len = merchantItem.Goods0[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods0[j].ItemType, merchantItem.Goods0[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }

                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods0[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods0[j].StartId, grp_len);
                        merchantItem.Goods0[j] = new_preset_template_grp;
                    }
                    for (int j = 0; j < merchantItem.Goods1.Count; j++)
                    {
                        sbyte grp_len = merchantItem.Goods1[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods1[j].ItemType, merchantItem.Goods1[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }
                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods1[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods1[j].StartId, grp_len);
                        merchantItem.Goods1[j] = new_preset_template_grp;
                    }

                    for (int j = 0; j < merchantItem.Goods2.Count; j++)
                    {
                        sbyte grp_len = merchantItem.Goods2[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods2[j].ItemType, merchantItem.Goods2[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }
                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods2[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods2[j].StartId, grp_len);
                        merchantItem.Goods2[j] = new_preset_template_grp;
                    }

                    for (int j = 0; j < merchantItem.Goods3.Count; j++)
                    {
                        sbyte grp_len = merchantItem.Goods3[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods3[j].ItemType, merchantItem.Goods3[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }
                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods3[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods3[j].StartId, grp_len);
                        merchantItem.Goods3[j] = new_preset_template_grp;
                    }
                    for (int j = 0; j < merchantItem.Goods4.Count; j++)
                    {
                        sbyte grp_len = merchantItem.Goods4[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods4[j].ItemType, merchantItem.Goods4[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }
                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods4[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods4[j].StartId, grp_len);
                        merchantItem.Goods4[j] = new_preset_template_grp;
                    }
                    for (int j = 0; j < merchantItem.Goods5.Count; j++)
                    {
                        sbyte grp_len = merchantItem.Goods5[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods5[j].ItemType, merchantItem.Goods5[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }
                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods5[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods5[j].StartId, grp_len);
                        merchantItem.Goods5[j] = new_preset_template_grp;
                    }
                    for (int j = 0; j < merchantItem.Goods6.Count; j++)
                    {
                        sbyte grp_len = merchantItem.Goods6[j].GroupLength;
                        //我们需要检查物品是否可堆叠, 仅增加可堆叠物品的数量
                        if (ItemTemplateHelper.IsStackable(
                            merchantItem.Goods6[j].ItemType, merchantItem.Goods6[j].StartId
                            ))
                        {
                            if (grp_len >= 0 && grp_len <= 12)//sbyte 的最大值为127
                            {
                                grp_len *= stack_factor;
                            }
                        }
                        var type_name = GameData.Domains.Item.ItemType.TypeId2TypeName[merchantItem.Goods6[j].ItemType];
                        var new_preset_template_grp = new PresetItemTemplateIdGroup(type_name, merchantItem.Goods6[j].StartId, grp_len);
                        merchantItem.Goods6[j] = new_preset_template_grp;
                    }
                }
                is_modified = true;
            }
        }
    }
}
