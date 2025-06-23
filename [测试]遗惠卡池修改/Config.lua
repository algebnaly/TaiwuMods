return {
	Title = "[测试]遗惠卡池修改",
	Author = "algebnaly",
	Cover = "Cover.png",
	WorkshopCover = "Cover.png",
	Source = 0,
	GameVersion = "0.0.78.30-test",
	Version = "1.0.0.4",
	FileId = 3495268984,
	Visibility = 0,
	TagList = {
		[1] = "Modifications",
	},
	BackendPlugins = {
		[1] = "ModifyCardPoolBackend.dll",
	},
	Description = "修改遗惠卡池权重, 让必死难度下寿元激发和魅力激发没那么频繁, 将负面遗惠的权重改为0， 地区恩义类权重设为0， 传功铸气类权重降低为1,  激发类权重设置为10。\n已修复击败五方神龙时爆红字问题",
	DefaultSettings = { },
	ChangeConfig = false,
	HasArchive = false,
	NeedRestartWhenSettingChanged = false,
	UpdateLogList = {
		[1] = {
			Timestamp = 1749366632,
		},
		[2] = {
			Timestamp = 1749369332,
		},
		[3] = {
			Timestamp = 1749369465,
			LogList = {
				[1] = "调整传功铸气类遗惠的权重",
			},
		},
		[4] = {
			Timestamp = 1749370831,
		},
		[5] = {
			Timestamp = 1750685658,
			LogList = {
				[1] = "修复击败五方神龙时爆红字的问题",
			},
		},
	},
}
