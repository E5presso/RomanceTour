using Core.Network.NCloud;

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

			Sens message = new Sens(url, uri, serviceId, accessKey, secretKey, from);
			bool result = message.SendMessage(MessageType.SMS, "[낭만투어] 메세지 전송 테스트", "01064308733", "[낭만투어] SMS 메세지 발송을 테스트합니다.").Result;
			if (result) Console.WriteLine("전송 완료!");
			else Console.WriteLine("전송 실패...");
			Console.ReadKey();
		}
	}
}