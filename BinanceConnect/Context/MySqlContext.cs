using BinanceConnect.Configuration;
using BinanceConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceConnect.Context
{
	//DbContext（DBとクラス情報をマップする）
	public class MySqlContext : DbContext
	{
		public DbSet<PriceStatsSecond> PriceStatsSecond { get; set; }
		public DbSet<PriceStatsMinute> PriceStatsMinute { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySQL(AppSettings.DefaultConnection);

			// この設定により引数のログが出力される
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}
