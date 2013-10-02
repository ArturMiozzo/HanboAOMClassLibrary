﻿using Hanbo.Models.Argument;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Hanbo.Helper
{
	/// <summary>
	/// 應用程式設定 Helper
	/// </summary>
	public static class ConfigurationHelper
	{
		/// <summary>
		/// 取得工作目錄
		/// 若指定的目錄不存在，則會建立
		/// 若指定的目錄為空，則採用系統預設值 Environment.SpecialFolder.Personal
		/// </summary>
		/// <param name="defaultDirectory"></param>
		/// <returns></returns>
		public static string GetInitialDirectory(string defaultDirectory)
		{
			if (!String.IsNullOrEmpty(defaultDirectory))
			{
				if (!Directory.Exists(defaultDirectory))
					Directory.CreateDirectory(defaultDirectory);
			}
			var initialDirectory = String.IsNullOrEmpty(defaultDirectory) ?
				Environment.GetFolderPath(Environment.SpecialFolder.Personal) : defaultDirectory;
			return initialDirectory;
		}

		/// <summary>
		/// 取得自動輸出參數設定
		/// </summary>
		/// <returns></returns>
		public static AutoExportArgument GetAutoExportArgument()
		{
			var _settingPath = ConfigurationManager.AppSettings["AutoExportSettingFilepath"].ToString();
			AutoExportArgument _autoExportSettings = new AutoExportArgument();
			var fileExists = File.Exists(_settingPath);
			if (fileExists)
			{
				var lines = File.ReadAllLines(_settingPath, Encoding.UTF8);
				var line = lines.Skip(1).Take(1).SingleOrDefault();
				if (line != null)
				{
					var parts = line.Split(',');
					var postfixType = (parts[3] == "AutoNumber") ? AutoExportArgumentPostfixType.AutoNumber
									: (parts[3] == "DateTime") ? AutoExportArgumentPostfixType.DateTime
									: AutoExportArgumentPostfixType.Mixed;
					var postFixFormat = (parts[3] == "AutoNumber") ? "{0:0000}"
									: (parts[3] == "DateTime") ? "{0:yyyyMMddHHmmss}"
									: "{0:yyyyMMddHHmmss}-{1:0000}";
					if (parts.Length >= 5)
					{
						_autoExportSettings.OutputDirectory = parts[0];
						_autoExportSettings.PrefixType = parts[1] == "Default" ? AutoExportArgumentPrefixType.Default : AutoExportArgumentPrefixType.Custom;
						_autoExportSettings.Prefix = parts[2];
						_autoExportSettings.PostfixType = postfixType;
						_autoExportSettings.ExtensionName = parts[4];
						_autoExportSettings.PostfixFormat = postFixFormat;
					}
				}
			}
			return _autoExportSettings;
		}

		/// <summary>
		/// GetAutoExportFileName
		/// </summary>
		/// <param name="autoExportSetting">資料</param>
		/// <param name="autoNumber">目前自動編號</param>
		/// <param name="defaultPrefix">預設前置詞</param>
		/// <returns></returns>
		public static string GetAutoExportFileName(AutoExportArgument autoExportSetting, int autoNumber, string defaultPrefix)
		{
			var postfix = "";
			var postfixFormat = autoExportSetting.PostfixFormat;
			switch (autoExportSetting.PostfixType)
			{
				case AutoExportArgumentPostfixType.AutoNumber:
					postfix = String.Format(postfixFormat, autoNumber);
					break;
				case AutoExportArgumentPostfixType.DateTime:
					postfix = String.Format(postfixFormat, DateTime.Now);
					break;
				case AutoExportArgumentPostfixType.Mixed:
					postfix = String.Format(postfixFormat, DateTime.Now, autoNumber);
					break;
			}

			var prefix = autoExportSetting.PrefixType == AutoExportArgumentPrefixType.Default ?
							defaultPrefix : autoExportSetting.Prefix;
			var filename = prefix + postfix + autoExportSetting.ExtensionName;
			return Path.Combine(autoExportSetting.OutputDirectory, filename);
		}

		/// <summary>
		/// 取得是否開啟攝影機提示
		/// </summary>
		/// <returns></returns>
		public static bool GetOpenCameraNoticeSetting()
		{
			bool isNotice = false;
			var value = ConfigurationManager.AppSettings["OpenCameraNotice"];
			Boolean.TryParse(value, out isNotice);
			return isNotice;
		}

		/// <summary>
		/// 傳回一個新的 ShapeModel 存放路徑
		/// </summary>
		/// <returns></returns>
		public static string GetShapeModelFilePath()
		{
			var dir = getDir("ShapeModelDefaultDirectory", "ShapeModel");
			var extension = ".shm";
			var filename = Guid.NewGuid().ToString() + extension;
			return Path.Combine(dir, filename);
		}

		/// <summary>
		/// 傳回一個新的 影像訓練模型路徑
		/// </summary>
		/// <returns></returns>
		public static string GetTrainingImageFilepath()
		{
			var dir = GetTrainingImageDefaultDir();
			var extension = ".tiff";
			var filename = Guid.NewGuid().ToString() + extension;
			return Path.Combine(dir, filename);
		}

		/// <summary>
		/// 傳回訓練影像的儲存目錄
		/// </summary>
		/// <returns></returns>
		public static string GetTrainingImageDefaultDir()
		{
			return getDir("SaveImageDefaultDirectory", "ShapeTrainingImage");
		}

		/// <summary>
		/// 傳回暫存目錄
		/// </summary>
		/// <returns></returns>
		public static string GetTmpFileDir()
		{
			return getDir("TmpFileFolder", "Tmp");
		}

		/// <summary>
		/// 傳回 GevSCPSPacketSize (for Pylon Driver)
		/// 預設值為 1500
		/// </summary>
		/// <returns></returns>
		public static int GetGevSCPSPacketSize()
		{
			var gevSCPSPacketSize = ConfigurationManager.AppSettings["GevSCPSPacketSize"];
			int size;
			if (!Int32.TryParse(gevSCPSPacketSize, out size))
			{
				size = 1500;//Default value
			}
			return size;
		}

		private static string getDir(string key, string subDirName)
		{
			var dir = ConfigurationManager.AppSettings[key].ToString();
			if (String.IsNullOrEmpty(dir))
			{
				dir = Path.Combine(Environment.CurrentDirectory, subDirName);
			}
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			return dir;
		}

		public static string GetSaveImageFilter()
		{
			var filter = ConfigurationManager.AppSettings["SaveImageFilter"];
			if (String.IsNullOrEmpty(filter))
				filter = "tiff (*.tiff;*.tif)|*.tiff;*.tif|jpeg (*.jpg;*.jpeg)|*.jpg;*.jpeg|png (*.png)|*.png|bmp (*.bmp)|*.bmp|all files (*.*)|*.*";
			return filter;
		}

		public static string GetOpenImageFilter()
		{
			var filter = ConfigurationManager.AppSettings["OpenImageFilter"];
			if (String.IsNullOrEmpty(filter))
				filter = "Image Files(*.png;*.tiff;*.tif;*.jpg;*.jpeg)|*.png;*.tiff;*.tif;*.jpg;*.jpeg|png (*.png)|*.png|tiff (*.tiff;*.tif)|*.tiff;*.tif|jpeg (*.jpg;*.jpeg)|*.jpg;*.jpeg|all files (*.*)|*.*";
			return filter;
		}

		public static string[] GetValidImageExtensions()
		{
			var extensions = ConfigurationManager.AppSettings["OpenImageFilter"];
			if (String.IsNullOrEmpty(extensions))
				extensions = "bmp,jpg,png,tiff";

			return extensions.Split(',');
		}

		/// <summary>
		/// 取得偵測硬體連線的間隔時間，若無設定，預設時間為 500ms
		/// </summary>
		/// <returns></returns>
		public static int GetHeartbeatInterval()
		{
			var interval = ConfigurationManager.AppSettings["HeartbeatInterval"];
			int heartbeat;
			if (!Int32.TryParse(interval, out heartbeat))
			{
				heartbeat = 500;
			}
			return heartbeat;
		}
	}
}