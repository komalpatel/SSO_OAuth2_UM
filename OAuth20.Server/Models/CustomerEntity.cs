using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace OAuth20.Server.Models
{
	public class CustomerEntity : TableEntity
	{
		private Customer _customer;

		public CustomerEntity() { }
		public CustomerEntity(Customer customer)
		{
		}

		public string Id { get; set; }
		public string StripeId { get; set; }
		public bool? IsSmallBusiness { get; set; }
		public int InvoiceNo { get; set; }
		public int OperatorId { get; set; }
		public Guid Guid { get; set; }
		public void SetStateGuid(Guid guid)
		{
			StateGuid = guid;
		}
		public string Email { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
		public string Partner { get; set; }
		public string VatId { get; set; }
		public string Country { get; set; } = "DE";

		private string _Password { get; set; }
		[XmlIgnore]
		[JsonIgnore]
		public string Password { private get { return _Password; } set { _Password = value; } }
		public string PasswordHash { get; set; }
		public string Salt { get; set; }
		public Guid StateGuid { get; set; }

		public CustomerEntity SetCustomer(Customer customer, Guid guid)
		{
			string[] emailParts = customer.Email.Split('@');
			this.PartitionKey = emailParts[1];
			this.RowKey = emailParts[0];
			_customer = customer;
			Guid = guid;
			this.Id = _customer.Id;
			this.Email = _customer.Email;
			this.Name = _customer.Name;
			return this;
		}

		internal string CreateHash()
		{
			Salt = Email.ToLower();
			if (string.IsNullOrEmpty(PartitionKey))
			{
				string[] emailParts = Email.ToLower().Split('@');
				PartitionKey = emailParts[1];
				RowKey = emailParts[0];

			}
			PasswordHash = GetHashString(Password + Salt);
			return PasswordHash;
		}

		public static byte[] GetHash(string inputString)
		{
			using (HashAlgorithm algorithm = SHA256.Create())
				return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		public static string GetHashString(string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in GetHash(inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}
	}
	public class CustomerEntityExt : CustomerEntity
	{
		public string Phone { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
		public string Country { get; set; }

	}
}
