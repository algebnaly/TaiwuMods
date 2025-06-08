return {
	Title = "[测试]战斗历练恩义获取修改",
	Author = "algebnaly",
	Cover = "Cover.png",
	WorkshopCover = "Cover.png",
	Source = 0,
	GameVersion = "0.0.78.24-test",
	Version = "1.0.1.0",
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
		[4] = {
			Timestamp = 1749353981,
		},
	},
	TagList = {
		[1] = "Modifications",
	},
	BackendPlugins = {
		[1] = "CombatGetExpModifyBackend.dll",
	},
	Description = "        简单地增加战斗历练获取基础值\n        原来的历练获取基础值为\n        {\n          15, 30, 60, 90, 135, 180, 240, 315, 390, 480,\n          570, 690, 840, 1020, 1230, 1500, 1830, 2280, 2850\n        };\n        我将其设置为该基础值的倍数, 向下取整。\n        默认为3倍, 可以在mod设置中修改, 注意不要超过11倍否则可能数值溢出倒扣历练值!\n\t现在可以修改恩义获取倍数了\n        ",
	DefaultSettings = {
		[1] = {
			SettingType = "Slider",
			Key = "combat_get_exp_scale_factor",
			DisplayName = "战斗历练获取倍数设置",
			Description = "战斗历练获取的倍数",
			MinValue = 1,
			MaxValue = 100,
			StepSize = 1,
			DefaultValue = 3,
		},
		[2] = {
			SettingType = "Slider",
			Key = "combat_get_area_spiritual_debt_scale_factor",
			DisplayName = "战斗地区恩义获取倍数设置",
			Description = "战斗地区恩义获取的倍数",
			MinValue = 1,
			MaxValue = 100,
			StepSize = 1,
			DefaultValue = 10,
		},
	},
	ChangeConfig = false,
	HasArchive = false,
	NeedRestartWhenSettingChanged = false,
}
