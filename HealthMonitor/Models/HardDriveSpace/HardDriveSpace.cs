using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Models.HardDriveSpace
{
	public class HardDriveSpace
	{
		public string name { get; set; }
		public string label { get; set; }
		public long totalSize { get; set; }
		public long totalFreeSpace { get; set; }
		public int percentageFreeSpace { get; set; }




		public static bool Compare(List<HardDriveSpace> hardDriveSpace1,List<HardDriveSpace> hardDriveSpace2)
		{
			return (hardDriveSpace1.Any(s1 =>
			{
				var s2 = hardDriveSpace2.FirstOrDefault(t => t.name == s1.name);
				return s1.Equals(s2) == false;
			}) || hardDriveSpace2.Any(s2 =>
			{
				var s1 = hardDriveSpace1.FirstOrDefault(t => t.name == s2.name);
				return s2.Equals(s1) == false;
			})) == false;
		}

		public bool Equals(HardDriveSpace hardDriveSpace)
		{
			if (hardDriveSpace == null) return false;
			if (name != hardDriveSpace.name) return false;
			if (label != hardDriveSpace.label) return false;
			if (totalSize != hardDriveSpace.totalSize) return false;
			if (totalFreeSpace != hardDriveSpace.totalFreeSpace) return false;
			if (percentageFreeSpace != hardDriveSpace.percentageFreeSpace) return false;


			return true;
		}
	}





}
