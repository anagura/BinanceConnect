using BinanceConnect.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConnect
{
	class Program
	{
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args"></param>
		public static async Task Main(string[] args)
		{
			Console.WriteLine("Binance Connect Started.");

			AppSettings.Initialize();

			await FxRate.Start(1);

			BinanceStats binance = new BinanceStats();
			binance.Start();

			SuperLoop(binance);

			binance.End();
		}

		/// <summary>
		/// メッセージループ
		/// </summary>
		/// <param name="token"></param>
		private static void SuperLoop(BinanceStats binance, CancellationToken token = default)
		{
			do
			{
				try
				{
					var stdin = Console.ReadLine()?.Trim();
					if (string.IsNullOrWhiteSpace(stdin))
					{
						Console.WriteLine("IsNullOrWhiteSpace");
						continue;
					}

					// Quit/Exit
					if (stdin.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
						stdin.Equals("exit", StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
					else if (stdin.StartsWith("limit", StringComparison.OrdinalIgnoreCase))
					{
						var args = stdin.Split(' ');

						decimal value = 0;
						if (args.Length > 1)
						{
							try
							{
								value = Convert.ToDecimal(args[1]);
								if (value > 0)
								{
									binance.NoticeLimit = value;
									Console.WriteLine(string.Format("limit set to {0}", binance.NoticeLimit));
								}
							}
							catch (FormatException)
							{
								Console.WriteLine(string.Format("FormatException {0}", args[1]));
							}
						}
						else
						{
							Console.WriteLine(string.Format("limit is {0}", binance.NoticeLimit));
						}

						continue;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine();
					Console.WriteLine($"! Exception: {e.Message}");
					if (e.InnerException != null)
					{
						Console.WriteLine($"  -> {e.InnerException.Message}");
					}
				}
			}
			while (true);
		}
	}
}
