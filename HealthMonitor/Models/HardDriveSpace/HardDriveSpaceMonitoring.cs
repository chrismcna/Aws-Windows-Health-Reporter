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

namespace HealthMonitor.Models.HardDriveSpace
{
	public class HardDriveSpaceMonitoring : Monitoring
	{

		private List<HardDriveSpace> previousResults = null;
		private CancellationTokenSource cancellationToken = null;

		public HardDriveSpaceMonitoring(Action<object> callBack) : base(callBack)
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
			var spaceInfos = DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => new HardDriveSpace()
			{
				name = d.Name,
				label = d.VolumeLabel,
				totalSize = d.TotalSize,
				totalFreeSpace = d.TotalFreeSpace,
				percentageFreeSpace = (int)( ((float)d.TotalFreeSpace / (float)d.TotalSize) * 100)
			}).ToList();


			if (previousResults == null || HardDriveSpace.Compare(spaceInfos, previousResults) == false) {
				callBack(spaceInfos);
			}

			previousResults = spaceInfos;


		}


	}
}
