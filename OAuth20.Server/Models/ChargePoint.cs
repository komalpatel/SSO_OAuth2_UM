using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OAuth20.Server.Models
{

	public class ChargePoint : TableEntity
	{
		public ChargePoint()
		{
		}
		private Guid _Guid { get; set; }
		public Guid Guid
		{
			get { return _Guid; }
			set
			{
				_Guid = value;
				RowKey = value.ToString();
			}
		}
		public Guid CpLocation { get; set; }
		[JsonProperty("ConnectorId")]
		public int ConnectorId { get; set; } = 1;
		public string Name { get; set; }
		public string CpName { get; set; }
		public string Owner { get; set; }
		public string OwnerFullName { get; set; }
		public string UStNr { get; set; }
		public double Lat { get; set; }
		public double Lon { get; set; }

		public int Type { get { return Convert.ToInt32(CpType); } set { CpType = (ChargePointType)value; } }
		[JsonIgnore]
		[XmlIgnore]
		public ChargePointType CpType { get; set; }
		public int Connector { get { return Convert.ToInt32(CpConnector); } set { CpConnector = (ChargePointConnector)value; } }
		[JsonIgnore]
		[XmlIgnore]
		public ChargePointConnector CpConnector { get; set; }
		public int State { get { return Convert.ToInt32(CpState); } set { CpState = (ChargePointState)value; } }
		[JsonIgnore]
		[XmlIgnore]
		public ChargePointState CpState { get; set; }
		public bool Available { get; set; }
		public bool IsAvailable { get; set; }
		public double Tarif { get; set; }
		public double ServiceFee { get; set; }
		public int MinimalFee { get; set; }
		public double? SunShineRate { get { return (float)Tarif; } set { Tarif = value != null ? (float)value : Tarif; } }
		public double MoonShineRate { get; set; }
		/// <summary>
		/// SunShineRate for Parking Sessions only
		/// </summary>
		[JsonProperty("SunShineParkingRate")]
		public double SunShineParkingRate { get; set; }
		/// <summary>
		/// MoonShineRate for Parking Sessions only
		/// </summary>
		[JsonProperty("MoonShineParkingRate")]
		public double MoonShineParkingRate { get; set; }
		/// <summary>
		/// C@F Service Fee (inkl. Stripe) in Ct.
		/// </summary>
		public double Power { get; set; }
		public string ThumbnailUrl { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		private int _ZipCode { get; set; }
		public int? Credit { get; set; }
		public enum ChargePointState
		{
			Active = 0x1,
			Charging = 0x2,
			Service = 0x4,
			Error = 0x8,
			Planned = 0x10,
			StickerRequested = 0x20,
			StickerInProduction = 0x40,
			StickerSend = 0x80
		}
		public enum ChargePointType
		{
			/// <summary>
			/// Offline CP
			/// </summary>
			Manual = 0x1,
			/// <summary>
			/// Not implemented
			/// </summary>
			OCPP15 = 0x2,
			/// <summary>
			/// currently supported OCPP Protocol version
			/// </summary>
			OCPP16 = 0x4,
			/// <summary>
			/// for future use, replaced by OCPP21
			/// </summary>
			OCPP20 = 0x8,
			/// <summary>
			/// only QR scan enabled, no reservation possible, decimal value is 16
			/// </summary>
			AdHoc = 0x10,
			/// <summary>
			/// for future use, station will only appear on the map for CUG-members
			/// </summary>
			ClosedUserGroupOnly = 0x20,
			/// <summary>
			/// The Future OCPP Protocol Version
			/// </summary>
			OCPP21 = 0x40,
			/// <summary>
			/// Station is calibrated = eichrechtskonform
			/// </summary>
			Calibrated = 0x80,
			/// <summary>
			/// Only charging by Parking time, no kWh invoicing
			/// </summary>
			ParkingOnly = 0x100
		}
		public enum ChargePointConnector
		{
			Typ1 = 1,
			Typ2 = 2,
			Schuko = 4,
			AC = 8,
			DC = 16
		}
		public int ZipCode
		{
			get { return _ZipCode; }
			set
			{
				_ZipCode = value;
				//PartitionKey = value.ToString();
			}
		}
		[JsonIgnore]
		public byte[] ImageData { get; set; }

		public bool hasState(ChargePointState value)
		{
			return (CpState & value) == value;
		}
		public void setState(ChargePointState value)
		{
			CpState |= value;
		}
		public void clearState(ChargePointState value)
		{
			CpState = CpState & (~value);
		}
		public bool hasType(ChargePointType value)
		{
			return (CpType & value) == value;
		}
		public void setType(ChargePointType value)
		{
			CpType |= value;
		}
		public void clearType(ChargePointType value)
		{
			CpType = CpType & (~value);
		}
	}
	public partial class OcppChargePoint
	{
		public OcppChargePoint()
		{
			Transactions = new HashSet<Transaction>();
		}

		public string ChargePointId { get; set; }
		public string Name { get; set; }
		public string Comment { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ClientCertThumb { get; set; }

		public virtual ICollection<Transaction> Transactions { get; set; }
	}
	public partial class Transaction
	{
		public int TransactionId { get; set; }
		public string Uid { get; set; }
		public string ChargePointId { get; set; }
		public int ConnectorId { get; set; }
		public string StartTagId { get; set; }
		public DateTime StartTime { get; set; }
		public double MeterStart { get; set; }
		public string StartResult { get; set; }
		public string StopTagId { get; set; }
		public DateTime? StopTime { get; set; }
		public double? MeterStop { get; set; }
		public string StopReason { get; set; }
		public string SampledValueValue { get; set; }

		public virtual OcppChargePoint ChargePoint { get; set; }
	}
}
