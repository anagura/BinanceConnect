using BinanceConnect.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Principal;

namespace BinanceConnect
{
	public class LineNotify
	{
		private static LineNotify _lineNotify;

		private readonly string _url = null;
		private readonly string[] _token = null;

		public LineNotify(string url, string[] token)
		{
			_url = url;
			_token = token;
		}

		public static async Task<string> Send(string message)
		{
			if (_lineNotify == null)
			{
				_lineNotify = new LineNotify(AppSettings.LineNotifyUrl, AppSettings.LineNotifyToken);
			}

			return await _lineNotify.SendToLine(message);

		}

		/// <summary>
		/// LINE通知
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private async Task<string> SendToLine(string message)
		{
			if (_url == null || _token == null)
			{
				return null;
			}

            string response = string.Empty;
            var payload = "message=" + HttpUtility.UrlEncode(message, Encoding.UTF8);
            foreach (var notifyToken in _token)
            {
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    wc.Headers.Add("Authorization", "Bearer " + notifyToken);
                    response = await wc.UploadStringTaskAsync(_url, payload);
                }
            }
            return response;
		}
	}
}