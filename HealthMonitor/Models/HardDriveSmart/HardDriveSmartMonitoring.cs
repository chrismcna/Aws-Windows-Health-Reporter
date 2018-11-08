using Amazon;
using Amazon.IotData;
using Amazon.IotData.Model;
using HealthMonitor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitor.Models.HardDriveSmart
{
	public class HardDriveSmartMonitoring : Monitoring
	{
		
		private Dictionary<int, HDD> previousSmartInfos = null;
		private CancellationTokenSource cancellationToken = null;

		public HardDriveSmartMonitoring(Action<object> callBack) : base(callBack)
		{

		}

		public override void Start()
		{
			cancellationToken = new CancellationTokenSource();
			Task.Delay(2000, cancellationToken.Token).ContinueWith((t) => Run(), cancellationToken.Token);
		}

		public override void Stop()
		{
			cancellationToken.Cancel();
			cancellationToken = null;
		}


		private void Run()
		{
			var smartInfos = getSmartInfos();

			if (previousSmartInfos == null || HDD.Compare(smartInfos, previousSmartInfos) == false)
			{
				callBack(smartInfos);
			}

			previousSmartInfos = smartInfos;
		}






		public static Dictionary<int, HDD> getSmartInfos()
		{
			var dicDrives = new Dictionary<int, HDD>();

			var wdSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

			// extract model and interface information
			int iDriveIndex = 0;
			foreach (ManagementObject drive in wdSearcher.Get())
			{
				var hdd = new HDD();
				hdd.Model = drive["Model"].ToString().Trim();
				hdd.Type = drive["InterfaceType"].ToString().Trim();
				dicDrives.Add(iDriveIndex, hdd);
				iDriveIndex++;
			}

			var pmsearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

			// retrieve hdd serial number
			iDriveIndex = 0;
			foreach (ManagementObject drive in pmsearcher.Get())
			{
				// because all physical media will be returned we need to exit
				// after the hard drives serial info is extracted
				if (iDriveIndex >= dicDrives.Count)
					break;

				dicDrives[iDriveIndex].Serial = drive["SerialNumber"] == null ? "None" : drive["SerialNumber"].ToString().Trim();
				iDriveIndex++;
			}

			// get wmi access to hdd 
			var searcher = new ManagementObjectSearcher("Select * from Win32_DiskDrive");
			searcher.Scope = new ManagementScope(@"\root\wmi");

			// check if SMART reports the drive is failing
			searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictStatus");
			iDriveIndex = 0;
			foreach (ManagementObject drive in searcher.Get())
			{
				dicDrives[iDriveIndex].IsOK = (bool)drive.Properties["PredictFailure"].Value == false;
				iDriveIndex++;
			}

			// retrive attribute flags, value worste and vendor data information
			searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictData");
			iDriveIndex = 0;
			foreach (ManagementObject data in searcher.Get())
			{
				Byte[] bytes = (Byte[])data.Properties["VendorSpecific"].Value;
				for (int i = 0; i < 30; ++i)
				{
					try
					{
						int id = bytes[i * 12 + 2];

						int flags = bytes[i * 12 + 4]; // least significant status byte, +3 most significant byte, but not used so ignored.
													   //bool advisory = (flags & 0x1) == 0x0;
						bool failureImminent = (flags & 0x1) == 0x1;
						//bool onlineDataCollection = (flags & 0x2) == 0x2;

						int value = bytes[i * 12 + 5];
						int worst = bytes[i * 12 + 6];
						int vendordata = BitConverter.ToInt32(bytes, i * 12 + 7);
						if (id == 0) continue;

						var attr = dicDrives[iDriveIndex].Attributes[id];
						attr.Current = value;
						attr.Worst = worst;
						attr.Data = vendordata;
						attr.IsOK = failureImminent == false;
					}
					catch
					{
						// given key does not exist in attribute collection (attribute not in the dictionary of attributes)
					}
				}
				iDriveIndex++;
			}

			// retreive threshold values foreach attribute
			searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictThresholds");
			iDriveIndex = 0;
			foreach (ManagementObject data in searcher.Get())
			{
				Byte[] bytes = (Byte[])data.Properties["VendorSpecific"].Value;
				for (int i = 0; i < 30; ++i)
				{
					try
					{

						int id = bytes[i * 12 + 2];
						int thresh = bytes[i * 12 + 3];
						if (id == 0) continue;

						var attr = dicDrives[iDriveIndex].Attributes[id];
						attr.Threshold = thresh;
					}
					catch
					{
						// given key does not exist in attribute collection (attribute not in the dictionary of attributes)
					}
				}

				iDriveIndex++;
			}


			return dicDrives;
		}
	}
}
