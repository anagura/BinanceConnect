using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinanceConnect.Models
{
	public class PriceStatsMinute
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal LastPrice { get; set; }
		public decimal Diff { get; set; }
		public decimal Ask { get; set; }
		public decimal AskQuantity { get; set; }
		public decimal Bid { get; set; }
		public decimal BidQuantity { get; set; }
		public DateTime CreateTime { get; set; }

		public PriceStatsMinute DeepCopy()
		{
			return (PriceStatsMinute)this.MemberwiseClone();
		}
	}
}
