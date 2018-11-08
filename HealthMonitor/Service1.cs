using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor
{
	public partial class Service1 : ServiceBase
	{

		Process process = null;

		public Service1()
		{
			InitializeComponent();

			process = new Process();
		}

		protected override void OnStart(string[] args)
		{
			process.Start();
		}

		protected override void OnStop()
		{
			process.Stop();
		}
	}
}
