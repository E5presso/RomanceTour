using System;
using System.Xml;
using Core.Security;

namespace PasswordChanger
{
	public class Program
	{
		private static readonly XmlDocument config = new XmlDocument();
		private static void Main()
		{
			Console.Write("서버의 설치 경로를 지정해주세요 : ");
			string path = Console.ReadLine();
			try
			{
				path += @"\config.xml";
				config.Load(path);
				string password = string.Empty;
				string confirmPassword = string.Empty;
				do
				{
					Console.Write("새 비밀번호 : ");
					password = Console.ReadLine();
					Console.Write("비밀번호 확인 : ");
					confirmPassword = Console.ReadLine();
					if (password != confirmPassword)
					{
						Console.WriteLine("비밀번호가 일치하지 않습니다.");
						Console.WriteLine("다시 입력해주세요.");
					}
				} while (password != confirmPassword);

				string salt = KeyGenerator.GenerateString(32);
				string hash = Password.Hash(password, salt);
				config["configuration"]["administrator"]["password"].InnerText = hash;
				config["configuration"]["administrator"]["hashsalt"].InnerText = salt;

				config.Save(path);
				Console.WriteLine("비밀번호가 정상적으로 변경되었습니다.");
				Console.WriteLine("종료하시려면 아무 키나 누르세요...");
				Console.ReadKey();
			}
			catch (Exception e)
			{
				Console.WriteLine("비밀번호를 변경하는데에 실패하였습니다.");
				Console.WriteLine($"오류 : {e.Message}");
				Console.WriteLine($"스택 추적 : {e.StackTrace}");
				Console.WriteLine("자세한 사항은 시스템 개발자에게 문의해주세요.");
				Console.WriteLine("시스템 개발자 : 백세종");
				Console.WriteLine("연락처 : 010-6430-8733");
				Console.WriteLine("종료하시려면 아무 키나 누르세요...");
				Console.ReadKey();
			}
		}
	}
}