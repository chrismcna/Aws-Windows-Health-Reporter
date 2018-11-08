using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Models
{
	public abstract class Monitoring
	{
		protected Action<object> callBack = null;

		public Monitoring(Action<object> callBack)
		{
			this.callBack = callBack;
		}


		public abstract void Start();
		public abstract void Stop();

	}
}
