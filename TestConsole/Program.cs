using Core.Network.Sms;

using System;

namespace TestConsole
{
	public class Program
	{
		private static void Main(string[] args)
		{
			string url = "https://sens.apigw.ntruss.com";
			string uri = "/sms/v2/services/{0}/messages";
			string serviceId = "ncp:sms:kr:260165252261:romance_tour";
			string accessKey = "eQLq6kKSxChwuTrhcZDB";
			string secretKey = "8p7Wt6dm07SgXWaNsbaEH36zbxE0HsMomyTMhocF";
			string from = "01064308733";

			SmsMessage message = new SmsMessage(url, uri, serviceId, accessKey, secretKey, from);
			(var status, var response) = message.SendMessage(new Message
			{
				To = new string[] { "01080792838" },
				Content = "[낭만투어] SMS 메세지 발송을 테스트합니다."
			});
			Console.WriteLine($"STATUS : {status}\tRESPONSE : {response}");
		}
	}
}