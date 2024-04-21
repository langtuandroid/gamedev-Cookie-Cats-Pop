using System;
using System.Collections.Generic;

namespace Prime31
{
	public class GoogleSkuInfo
	{
		public GoogleSkuInfo(Dictionary<string, object> dict)
		{
			if (dict.ContainsKey("title"))
			{
				this.title = (dict["title"] as string);
			}
			if (dict.ContainsKey("price_amount_micros") && dict["price_amount_micros"].GetType() == typeof(long))
			{
				double num = (double)((long)dict["price_amount_micros"]);
				this.price = (num / 1000000.0).ToString();
				if (dict.ContainsKey("price"))
				{
					this.formattedPrice = (dict["price"] as string);
				}
				else
				{
					this.formattedPrice = this.price;
				}
			}
			else if (dict.ContainsKey("price"))
			{
				this.price = (dict["price"] as string);
				this.formattedPrice = (dict["price"] as string);
			}
			if (dict.ContainsKey("type"))
			{
				this.type = (dict["type"] as string);
			}
			if (dict.ContainsKey("description"))
			{
				this.description = (dict["description"] as string);
			}
			if (dict.ContainsKey("productId"))
			{
				this.productId = (dict["productId"] as string);
			}
			if (dict.ContainsKey("price_currency_code"))
			{
				this.priceCurrencyCode = (dict["price_currency_code"] as string);
			}
			if (dict.ContainsKey("price_amount_micros"))
			{
				long? num2 = dict["price_amount_micros"] as long?;
				if (num2 != null)
				{
					this.priceAmountMicros = num2.Value;
				}
			}
			if (dict.ContainsKey("subscriptionPeriod"))
			{
				this.subscriptionPeriod = (dict["subscriptionPeriod"] as string);
			}
		}

		public string title { get; private set; }

		public string price { get; private set; }

		public string type { get; private set; }

		public string description { get; private set; }

		public string productId { get; private set; }

		public string priceCurrencyCode { get; private set; }

		public long priceAmountMicros { get; private set; }

		public string formattedPrice { get; private set; }

		public string subscriptionPeriod { get; private set; }

		public static List<GoogleSkuInfo> fromList(List<object> items)
		{
			List<GoogleSkuInfo> list = new List<GoogleSkuInfo>();
			foreach (object obj in items)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)obj;
				list.Add(new GoogleSkuInfo(dict));
			}
			return list;
		}

		public override string ToString()
		{
			return string.Format("<GoogleSkuInfo> title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}, priceCurrencyCode: {5}", new object[]
			{
				this.title,
				this.price,
				this.type,
				this.description,
				this.productId,
				this.priceCurrencyCode
			});
		}
	}
}
