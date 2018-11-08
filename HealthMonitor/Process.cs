using Amazon;
using Amazon.IotData;
using Amazon.IotData.Model;
using HealthMonitor.Models;
using HealthMonitor.Models.HardDriveSmart;
using HealthMonitor.Models.HardDriveSpace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitor
{
	public class Process
	{

		private String thingName = ConfigurationManager.AppSettings["thingName"];
		private String awsAccessKeyId = ConfigurationManager.AppSettings["awsAccessKeyId"];
		private String awsSecretAccessKey = ConfigurationManager.AppSettings["awsAccessKeyId"];
		private AmazonIotDataClient Client = null;

		private List<Monitoring> Monitorings = null;



		public void Start()
		{
			AmazonIotDataConfig amazonIotDataConfig = new AmazonIotDataConfig();
			amazonIotDataConfig.RegionEndpoint = RegionEndpoint.EUWest1;
			//amazonIotDataConfig.UseHttp = false;
			//amazonIotDataConfig.SignatureVersion = "4";
			amazonIotDataConfig.ServiceURL = amazonIotDataConfig.DetermineServiceURL();

			Client = new AmazonIotDataClient(awsAccessKeyId, awsSecretAccessKey, amazonIotDataConfig);


			Monitorings = new List<Monitoring>();


			HardDriveSmartMonitoring hardDriveSmartMonitoring = new HardDriveSmartMonitoring((data) =>
			{
				var payload = new
				{
					state = new
					{
						reported = new
						{
							Monitoring = new
							{
								HardDriveSmart = data
							}
						}
					}
				};


				var request = new UpdateThingShadowRequest();
				request.ThingName = thingName;
				request.Payload = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));

				var response = Client.UpdateThingShadow(request);

			});
			Monitorings.Add(hardDriveSmartMonitoring);


			HardDriveSpaceMonitoring hardDriveSpaceMonitoring = new HardDriveSpaceMonitoring((data) =>
			{
				var payload = new
				{
					state = new
					{
						reported = new
						{
							Monitoring = new
							{
								HardDriveSpace = data
							}
						}
					}
				};


				var request = new UpdateThingShadowRequest();
				request.ThingName = thingName;
				request.Payload = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));

				var response = Client.UpdateThingShadow(request);

			});
			Monitorings.Add(hardDriveSpaceMonitoring);


			Monitorings.ForEach((monitoring) => monitoring.Start());
		}

		public void Stop()
		{
			Monitorings.ForEach((monitoring) => monitoring.Stop());
		}

	}
}
