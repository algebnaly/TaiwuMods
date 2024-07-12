return {
	Title = "商店更多物品",
	Author = "algebnaly",
	Cover = "Cover.png",
	WorkshopCover = "Cover.png",
	Source = 0,
	TagList = {
		[1] = "Modifications",
	},
	BackendPlugins = {
		[1] = "TaiwuShopMoreItemsBackend.dll",
	},
	Description = "提升商店出售物品数量,方便买材料精挑细选。\n可以在设置中调整物品刷新率与物品堆叠数量。",
	Version = "1.0.0.0",
	Visibility = 0,
	DefaultSettings = {
		[1] = {
			SettingType = "Slider",
			Key = "rate_factor",
			DisplayName = "物品刷新几率倍数",
			Description = "该倍数越高,物品越有可能出现在商店里",
			MinValue = 1,
			MaxValue = 10,
			StepSize = 1,
			DefaultValue = 1,
		},
		[2] = {
			SettingType = "Slider",
			Key = "stack_factor",
			DisplayName = "物品堆叠数量倍数",
			Description = "该倍数越高,物品出现在商店里时,堆叠数量越高, 只对可堆叠物品生效(材料, 茶酒等), 工具出现太多太占地方, 因此不加以改动",
			MinValue = 1,
			MaxValue = 10,
			StepSize = 1,
			DefaultValue = 5,
		},
	},
	ChangeConfig = false,
	HasArchive = false,
	NeedRestartWhenSettingChanged = true,
	FileId = 1,
	GameVersion = "0.0.71.66-test",
	UpdateLogList = {
		[1] = {
			Timestamp = 1720769257,
		},
	},
}
