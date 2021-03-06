﻿using Hanbo.Custom.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Device.SingleInstance
{
	public class DeviceCheckViewModel
	{
		[LocalizedDisplayNameAttribute("Model_DeviceName", "裝置名稱")]
		public string Name { get; set; }

		[LocalizedDisplayNameAttribute("Model_Description", "描述")]
		public string Description { get; set; }

		[LocalizedDisplayNameAttribute("Model_CheckResult", "檢查結果")]
		public string IsPass { get; set; }

		[LocalizedDisplayNameAttribute("Model_CheckMessage", "系統訊息")]
		public string Message { get; set; }

		public string StaticClassName { get; set; }

		public string InvokeMethod { get; set; }

		public string Params { get; set; }
	}
}
