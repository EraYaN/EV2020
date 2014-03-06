using EV2020.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV2020.Director
{
	public static class Data
	{
		public static string ComPort = "COM1";
		public static int BaudRate = 115200;
		static public SerialInterface com;

	}
}
