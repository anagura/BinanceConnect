using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinanceConnect.Models
{
	public class PriceStatsSecond
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal LastPrice { get; set; }
		public decimal Ask { get; set; }
		public decimal AskQuantity { get; set; }
		public decimal Bid { get; set; }
		public decimal BidQuantity { get; set; }
		public DateTime CreateTime { get; set; }

		public PriceStatsSecond DeepCopy()
		{
			return (PriceStatsSecond)this.MemberwiseClone();
		}
	}
}
