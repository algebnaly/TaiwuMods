return {
	Title = "修改太吾实际年龄",
	Author = "在下炮灰",
	Cover = "Cover.png",
	WorkshopCover = "Cover.png",
	Source = 0,
	TagList = {
		[1] = "Modifications",
	},
	BackendPlugins = {
		[1] = "TaiwuModifyActualAgeBackend.dll",
	},
	Description = "修改太吾的实际年龄(用于测试先天少阳剑气)",
	Version = "1.0.0.0",
	GameVersion = "0.0.70.51",
	Visibility = 0,
	DefaultSettings = {
		[1] = {
			SettingType = "Slider",
			Key = "want_age",
			DisplayName = "实际年龄",
			Description = "修改实际年龄,范围为1到32767",
			MinValue = 1,
			MaxValue = 32767,
			StepSize = 1,
			DefaultValue = 32767,
		},
	},
	ChangeConfig = false,
	HasArchive = false,
	NeedRestartWhenSettingChanged = false,
}
