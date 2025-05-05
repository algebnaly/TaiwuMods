using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;
using GameData.Domains.Item.Display;
using System;
using System.Collections.Generic;

namespace EfficientSamsaraFrontend
{
    [PluginConfig("EfficientSamsaraFrontend", "algebnaly", "0.1.0")]
    public class FrontendPatch : TaiwuRemakePlugin
    {
        // Harmony harmony;
        private static GameObject gameObject;
        public override void Dispose()
        {
            UnityEngine.Object.Destroy(gameObject);
        }

        public override void Initialize()
        {
            Debug.Log("启动 高效轮回 前端Mod");
            gameObject = new GameObject($"taiwu.EfficientSamsara{DateTime.Now.Ticks}");
            gameObject.AddComponent<EfficientSamsaraFrontend.MainWindow>();
            Debug.Log("启动 高效轮回 前端Mod 成功");
        }
    }
}
