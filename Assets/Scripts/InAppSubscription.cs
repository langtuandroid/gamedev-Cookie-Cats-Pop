using System;
using UnityEngine;

public class InAppSubscription
{
	public InAppSubscription()
	{
		this.Platform = RuntimePlatform.OSXEditor;
		this.ProductId = string.Empty;
		this.PurchaseDate = DateHelper.DefaultTime;
		this.ExpirationDate = DateHelper.DefaultTime;
		this.CancelationDate = DateHelper.DefaultTime;
	}

	[JsonSerializable("pl", null)]
	public RuntimePlatform Platform { get; set; }

	[JsonSerializable("pid", null)]
	public string ProductId { get; set; }

	[JsonSerializable("opd", null)]
	public DateTime PurchaseDate { get; set; }

	[JsonSerializable("ed", null)]
	public DateTime ExpirationDate { get; set; }

	[JsonSerializable("cd", null)]
	public DateTime CancelationDate { get; set; }

	public bool IsActive()
	{
		return this.CancelationDate < DateTime.UtcNow && this.PurchaseDate < DateTime.UtcNow && this.ExpirationDate > DateTime.UtcNow;
	}

	public override string ToString()
	{
		return string.Format("<InAppSubscription> purchased on platform: {0} productId: {1} purchase date: {2} expiration date: {3} cancelation date: {4}", new object[]
		{
			this.Platform,
			this.ProductId,
			this.PurchaseDate,
			this.ExpirationDate,
			this.CancelationDate
		});
	}
}
