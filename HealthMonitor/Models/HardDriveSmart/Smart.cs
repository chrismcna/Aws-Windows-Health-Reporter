using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Models.HardDriveSmart
{
	public class Smart
	{
		public bool HasData
		{
			get
			{
				if (Current == 0 && Worst == 0 && Threshold == 0 && Data == 0)
					return false;
				return true;
			}
		}
		public string Attribute { get; set; }
		public int Current { get; set; }
		public int Worst { get; set; }
		public int Threshold { get; set; }
		public int Data { get; set; }
		public bool IsOK { get; set; }

		public Smart()
		{

		}

		public Smart(string attributeName)
		{
			this.Attribute = attributeName;
		}



		public static bool Compare(Dictionary<int, Smart> smart1, Dictionary<int, Smart> smart2)
		{
			return (smart1.Any(s1 =>
			{
				var s2 = smart2.FirstOrDefault(t => t.Key == s1.Key);
				return s1.Value.Equals(s2.Value) == false;
			}) || smart2.Any(s2 =>
			{
				var s1 = smart1.FirstOrDefault(t => t.Key == s2.Key);
				return s2.Value.Equals(s1.Value) == false;
			})) == false;
		}

		public bool Equals(Smart smart)
		{
			if (smart == null)
				return false;
			if (Attribute != smart.Attribute)
				return false;
			if (Current != smart.Current)
				return false;
			if (Worst != smart.Worst)
				return false;
			if (Threshold != smart.Threshold)
				return false;
			if (Data != smart.Data)
				return false;
			if (IsOK != smart.IsOK)
				return false;

			return true;
		}
	}
}
