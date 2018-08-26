using Microsoft.Extensions.Configuration;
using System.IO;

namespace BinanceConnect.Configuration
{
	public class AppSettings
	{
		public static IConfigurationRoot Configuration;

		public static string FxCurrecyPair { get; private set; }
		public static string FxRateUrl { get; private set; }

		public static string LineNotifyToken { get; private set; }
		public static string LineNotifyUrl { get; private set; }

		public static string DefaultConnection { get; private set; }

		public static bool Initialize()
		{
			// Load configuration.
			Configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", true, false)
					.Build();

			LineNotifyToken = Configuration.GetSection("Line")["NotifyToken"];
			LineNotifyUrl = Configuration.GetSection("Line")["Url"];

			FxCurrecyPair = Configuration.GetSection("FxRate")["CurrencyPair"];
			FxRateUrl = Configuration.GetSection("FxRate")["Url"];

			DefaultConnection = Configuration.GetSection("MySql")["DefaultConnection"];

			return true;
		}

	}
}
