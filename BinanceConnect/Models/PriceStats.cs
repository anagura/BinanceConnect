using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinanceConnect.Models
{
	public class PriceStats
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Ask { get; set; }
		public decimal Bid { get; set; }
		public DateTime CreateTime { get; set; }
	}
}
