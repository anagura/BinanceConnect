using BinanceConnect.Configuration;
using BinanceConnect.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BinanceConnect
{
	public class FxRate
	{
		public static decimal Rate { get; private set; }

		private static string _fxCurrecyPair;
		public static string _fxRateUrl;

		public static async Task<bool> Start(int spanHours)
		{
			_fxCurrecyPair = AppSettings.FxCurrecyPair;
			_fxRateUrl = AppSettings.FxRateUrl;

			await GetFxRate(_fxCurrecyPair);
			Observable.Interval(TimeSpan.FromHours(spanHours)).TimeInterval().Subscribe(async x =>
			{
				await GetFxRate(_fxCurrecyPair);
			});

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pair"></param>
		/// <returns></returns>
		private static async Task<bool> GetFxRate(string pair)
		{
			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_fxRateUrl);
				if (response.IsSuccessStatusCode)
				{
					var data = await response.Content.ReadAsStringAsync();
					var ticker = JsonConvert.DeserializeObject<FxRateInfoList>(data);
					ticker.Quotes.ForEach(rate =>
					{
						if (rate.CurrencyPairCode == pair)
						{
							Rate = rate.Open;
							Console.WriteLine(string.Format("{1} rate is {0}", Rate, DateTime.Now));
						}
					});
				}
			}

			return true;
		}

	}
}
