return {
	Title = "[测试]战斗历练获取修改",
	Author = "algebnaly",
	Cover = "Cover.png",
	WorkshopCover = "Cover.png",
	Source = 0,
	GameVersion = "0.0.77.18-test",
	Version = "1.0.0.2",
	FileId = 3172379059,
	Visibility = 0,
	UpdateLogList = {
		[1] = {
			Timestamp = 1709473281,
		},
		[2] = {
			Timestamp = 1709967278,
			LogList = {
				[1] = "修复了因为每次战斗后都倍增历练获取基数导致数值溢出的bug",
			},
		},
		[3] = {
			Timestamp = 1749101142,
			LogList = {
				[1] = "修复了战斗后爆红字的错误",
			},
		},
	},
	TagList = {
		[1] = "Modifications",
	},
	BackendPlugins = {
		[1] = "CombatGetExpModifyBackend.dll",
	},
	Description = "        简单地增加战斗历练获取基础值\n        原来的历练获取基础值为\n        {\n          15, 30, 60, 90, 135, 180, 240, 315, 390, 480,\n          570, 690, 840, 1020, 1230, 1500, 1830, 2280, 2850\n        };\n        我将其设置为该基础值的倍数, 向下取整。\n        默认为3倍, 可以在mod设置中修改, 注意不要超过11倍否则可能数值溢出倒扣历练值!\n        Github: https://github.com/yalikes/TaiwuCombatGetExpModify\n        ",
	DefaultSettings = {
		[1] = {
			SettingType = "Slider",
			Key = "combat_get_exp_scale_factor",
			DisplayName = "战斗历练获取倍数设置(单位: 0.1倍)",
			Description = "1个单位表示原获取历练的0.1倍, 默认值20表示获取历练为原来的两倍",
			MinValue = 1,
			MaxValue = 110,
			StepSize = 1,
			DefaultValue = 30,
		},
	},
	ChangeConfig = false,
	HasArchive = false,
	NeedRestartWhenSettingChanged = false,
}
