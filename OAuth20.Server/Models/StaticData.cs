using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Models
{
	public class StaticData
	{


		public static IConfiguration Configuration { get; set; }


		public static Dictionary<string, Guid> StripStateValues = new Dictionary<string, Guid>();
		public static Dictionary<string, Guid> UserAccountGuids = new Dictionary<string, Guid>();
		public static CultureInfo locale = new CultureInfo("de-DE");

		//public static Dictionary<Guid, CpLocation> ChargeLocations = new Dictionary<Guid, CpLocation>();
	}
}
