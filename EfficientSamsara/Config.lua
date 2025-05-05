return {
	Title = "[测试]高效轮回",
	Author = "algebnaly",
	Cover = "Cover.png",
	WorkshopCover = "Cover.png",
	Source = 0,
	TagList = {
		[1] = "Modifications",
	},
	BackendPlugins = {
		[1] = "EfficientSamsaraBackend.dll",
	},
	Description = "第十次轮回时不清空轮回记录，而是使用先入先出的顺序, 记录最近的九次轮回记录。\n开启MOD设置中的\"切换装备页时加满前世\"选项后，切换太吾的装备页会直接使用当前太吾的复制角色填满太吾的前世记录，用于激活大般若涅槃法的特效，提高六维和资质等。",
	Version = "1.0.1.0",
	Visibility = 0,
	DefaultSettings = {
		[1] = {
			SettingType = "Toggle",
			Key = "enable_add_preIds",
			DisplayName = "切换装备页时加满前世",
			Description = "切换装备页时使用当前角色的复制角色加满前世",
			DefaultValue = true,
		},
	},
	ChangeConfig = false,
	HasArchive = false,
	GameVersion = "0.0.77.16-test",
	UpdateLogList = {
		[1] = {
			Timestamp = 1745246523,
		},
		[2] = {
			Timestamp = 1746451165,
		},
		[3] = {
			Timestamp = 1746451175,
		},
		[4] = {
			Timestamp = 1746451494,
		},
	},
	FileId = 3468026278,
	NeedRestartWhenSettingChanged = false,
}
