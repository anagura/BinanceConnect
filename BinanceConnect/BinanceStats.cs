using Binance;
using Binance.Cache;
using Binance.WebSocket;
using BinanceConnect.Context;
using BinanceConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinanceConnect
{
	public class BinanceStats
	{

		private readonly MySqlContext _context;
		private DepthWebSocketCache _webSocketCache;
		private static readonly bool _isDebug = false;

		private static readonly int _notifySpan = 60 * 5;    // 5分
		private static readonly decimal _limit = (decimal)0.5;

		public BinanceStats()
		{
			_context = new MySqlContext();

			// Initialize web socket cache (with automatic streaming).
			_webSocketCache = new DepthWebSocketCache();

			// Handle error events.
			_webSocketCache.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };
		}

		public void Start()
		{
			int minutes = -1;
			int idx = 0;
			Queue<decimal> minutesList = new Queue<decimal>();
			Dictionary<int, decimal> prices = new Dictionary<int, decimal>();

			Symbol symbol;
			decimal topAskPrice = 0;
			decimal topBidPrice = 0;
			decimal currentSecondPrice = 0;
			decimal currentMinutePrice = 0;
			PriceStats stats = new PriceStats();
			PriceStatsMinutes minutesStats = new PriceStatsMinutes();
			DateTime lastNotifyTime = DateTime.Now;

			using (var context = new MySqlContext())
			{
				// Subscribe callback to BTC/USDT (automatically begin streaming).
				_webSocketCache.Subscribe(Symbol.BTC_USDT, async evt =>
				{
					// Get symbol from cache (update cache if a symbol is missing).
					symbol = Symbol.Cache.Get(evt.OrderBook.Symbol);

					topAskPrice = evt.OrderBook.Top.Ask.Price;
					topBidPrice = evt.OrderBook.Top.Bid.Price;

					// Handle order book update events.
					//Console.WriteLine(string.Format("{0}, {1}", topAskPrice, topBidPrice));
					var now = DateTime.Now;
					stats.Name = "BTC_USDT";
					stats.Ask = topAskPrice;
					stats.Bid = topBidPrice;
					stats.CreateTime = now;
					currentSecondPrice = (topAskPrice + topBidPrice) / 2;
					prices.Add(idx++, currentSecondPrice);

					// 分処理
					if (now.Minute != minutes && prices.Any())
					{
						var average = Math.Round(prices.Values.Select(x => x).Average(), 2);
						decimal minutesDiff = 0;
						if (minutesList.Count() > 0)
						{
							var min = minutesList.Min();
							var max = minutesList.Max();

							var minutesMinDiff = Math.Round(100 - ((min / currentMinutePrice) * 100), 3);
							var minutesMaxDiff = Math.Round(100 - ((max / currentMinutePrice) * 100), 3);
							var msg = string.Format("[m][{5}] min:{0}, max:{1}, current:{2}, min_diff:{3}, max_diff{4}", min, max, currentMinutePrice, minutesMinDiff, minutesMaxDiff, now);
							//							Console.WriteLine(msg);

							// 差が大きい方を採用
							minutesDiff = Math.Abs(minutesMinDiff) > Math.Abs(minutesMaxDiff) ? minutesMinDiff : minutesMaxDiff;
							if (Math.Abs(minutesDiff) >= _limit)
							{
								if ((now - lastNotifyTime).TotalSeconds > _notifySpan)
								{
									var lineMsg = string.Format("ビットコインに値動きがありました。${1}({0}%)\n{2:#,0} 円",
									minutesDiff, currentMinutePrice, (int)(currentMinutePrice * FxRate.Rate));
									Console.WriteLine(lineMsg);
									var result = await LineNotify.Send(lineMsg);

									// 1度通知したら暫く通知を停止する
									lastNotifyTime = DateTime.Now;
								}
							}
						}
						// 5分チェック用
						minutesList.Enqueue(average);
						if (minutesList.Count > 5)
						{
							minutesList.Dequeue();
						}

						minutesStats.Name = "BTC_USDT";
						minutesStats.Price = average;
						minutesStats.Diff = minutesDiff;
						minutesStats.CreateTime = now;
						if (!_isDebug)
						{
							await _context.PriceStatsMinutes.AddAsync(minutesStats);
						}
						idx = 0;
						prices.Clear();
						currentMinutePrice = average;
					}
					minutes = stats.CreateTime.Minute;
					//Console.WriteLine(string.Format("ask:{0}, bid:{1}", topAskPrice * _fxRate, topBidPrice * _fxRate));
					if (!_isDebug)
					{
						await _context.PriceStats.AddAsync(stats);
						await _context.SaveChangesAsync();
					}

				});
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
	}
}