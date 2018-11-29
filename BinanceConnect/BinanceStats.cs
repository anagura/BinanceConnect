using Binance;
using Binance.Cache;
using Binance.WebSocket;
using BinanceConnect.Context;
using BinanceConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceConnect
{
	public class BinanceStats
	{

		private readonly MySqlContext _context;
		private SymbolStatisticsWebSocketCache _webSocketCache;
		private static readonly bool _isDebug = false;

		private static readonly int _notifySpan = 60 * 5;    // 5分
		private static decimal _limit = (decimal)0.5;

		private DateTime _lastNotifyTime;
		private PriceStatsMinute _lastMinutePrice;
		private List<PriceStatsSecond> _pricesSeconds;
		private Queue<PriceStatsMinute> _priceMinutes;

		public decimal NoticeLimit
		{
			get
			{
				return _limit;
			}

			set
			{
				_limit = value;
			}
		}

		public BinanceStats()
		{
			_lastMinutePrice = new PriceStatsMinute();
			_pricesSeconds = new List<PriceStatsSecond>();
			_priceMinutes = new Queue<PriceStatsMinute>();

			_context = new MySqlContext();

			// Initialize web socket cache (with automatic streaming).
			_webSocketCache = new SymbolStatisticsWebSocketCache();

			// Handle error events.
			_webSocketCache.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };
		}

		public void Start()
		{
			_pricesSeconds.Clear();
			_priceMinutes.Clear();
			using (var context = new MySqlContext())
			{
				int lastMinute = DateTime.Now.Minute;
				string symbol = string.Empty;
				var statInfo = new PriceStatsSecond();

				_webSocketCache.Subscribe(async evt =>
				{
					var now = DateTime.Now;
					foreach (var stat in evt.Statistics)
					{
						symbol = stat.Symbol;
						statInfo.Name = symbol;
						statInfo.LastPrice = stat.LastPrice;
						statInfo.Ask = stat.AskPrice;
						statInfo.AskQuantity = stat.AskQuantity;
						statInfo.Bid = stat.BidPrice;
						statInfo.BidQuantity = stat.BidQuantity;
						statInfo.CreateTime = now;
					}
					_pricesSeconds.Add(statInfo.DeepCopy());
					if (!_isDebug)
					{
						await _context.PriceStatsSecond.AddAsync(statInfo);
					}

					// 秒単位の通知処理
					if (_lastMinutePrice.LastPrice > 0)
					{
						var secondDiff = Math.Round(100 - ((_lastMinutePrice.LastPrice / statInfo.LastPrice) * 100), 3);
						await NotifyIfLimitOver(secondDiff, now, statInfo.LastPrice, statInfo.AskQuantity, statInfo.BidQuantity, "秒");
					}

					// 分処理
					if (now.Minute != lastMinute && _pricesSeconds.Any())
					{
						await Minute(statInfo.Name, now);
					}

					if (!_isDebug)
					{
						await _context.SaveChangesAsync();
					}

					lastMinute = now.Minute;

				}, new string[] { Symbol.BTC_USDT });
			}
		}

		public void End()
		{
			if (_webSocketCache == null)
			{
				return;
			}

			_webSocketCache.Unsubscribe();
		}

		/// <summary>
		/// 分ごとの処理
		/// </summary>
		/// <param name="symbol"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		private async Task<bool> Minute(string symbol, DateTime now)
		{
			var currentMinutePrice = _pricesSeconds.Average(x => x.LastPrice);
			_lastMinutePrice.Name = symbol;
			_lastMinutePrice.LastPrice = currentMinutePrice;
			_lastMinutePrice.Ask = _pricesSeconds.Average(x => x.Ask);
			_lastMinutePrice.AskQuantity = _pricesSeconds.Sum(x => x.AskQuantity);
			_lastMinutePrice.Bid = _pricesSeconds.Average(x => x.Bid);
			_lastMinutePrice.BidQuantity = _pricesSeconds.Sum(x => x.BidQuantity);
			_lastMinutePrice.CreateTime = now;


			if (_priceMinutes.Any())
			{
				var min = _priceMinutes.Min(x => x.LastPrice);
				var max = _priceMinutes.Max(x => x.LastPrice);

				var minutesMinDiff = Math.Round(100 - ((min / currentMinutePrice) * 100), 3);
				var minutesMaxDiff = Math.Round(100 - ((max / currentMinutePrice) * 100), 3);
				var minutesDiff = Math.Abs(minutesMinDiff) > Math.Abs(minutesMaxDiff) ? minutesMinDiff : minutesMaxDiff;
				await NotifyIfLimitOver(minutesDiff, now, currentMinutePrice, _lastMinutePrice.AskQuantity, _lastMinutePrice.BidQuantity, "分");
				_lastMinutePrice.Diff = minutesDiff;
			}

			_pricesSeconds.Clear();
			_priceMinutes.Enqueue(_lastMinutePrice.DeepCopy());
			if (_priceMinutes.Count > 5)
			{
				_priceMinutes.Dequeue();
			}
			if (!_isDebug)
			{
				await _context.PriceStatsMinute.AddAsync(_lastMinutePrice);
			}

			return true;
		}

		/// <summary>
		/// 閾値以上の変動があったら通知する
		/// </summary>
		/// <param name="diff"></param>
		/// <param name="now"></param>
		/// <param name="currentPrice"></param>
		/// <param name="askQuantity"></param>
		/// <param name="bidQuantity"></param>
		/// <param name="prefix"></param>
		/// <returns></returns>
		private async Task<string> NotifyIfLimitOver(decimal diff, DateTime now, decimal currentPrice, decimal askQuantity, decimal bidQuantity, string prefix)
		{
			string result = string.Empty;

			if (Math.Abs(diff) >= _limit)
			{
				if (_lastNotifyTime == null || (now - _lastNotifyTime).TotalSeconds > _notifySpan)
				{
					// 1度通知したら暫く通知を停止する
					_lastNotifyTime = DateTime.Now;

					var lineMsg = string.Format("[{5}]ビットコインに値動きがありました。${1}({0}%)\n{2:#,0} 円\n[注文量]\n買↑:{3}\n売↓:{4}",
					diff, currentPrice.ToString("#.##"), (int)(currentPrice * FxRate.Rate),
					askQuantity, bidQuantity, prefix);
					Console.WriteLine(lineMsg);
					result = await LineNotify.Send(lineMsg);
				}
			}

			return result;
		}
	}
}