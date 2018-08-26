using BinanceConnect.Configuration;
using BinanceConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceConnect.Context
{
	//DbContext（DBとクラス情報をマップする）
	public class MySqlContext : DbContext
	{
		public DbSet<PriceStats> PriceStats { get; set; }
		public DbSet<PriceStatsMinutes> PriceStatsMinutes { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySQL(AppSettings.DefaultConnection);

			// この設定により引数のログが出力される
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}
