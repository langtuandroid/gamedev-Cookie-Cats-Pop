using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class ProductData : Jsonable
	{
		public string Sku { get; set; }

		public string ProductType { get; set; }

		public string Price { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string SmallIconUrl { get; set; }

		public string ToJson()
		{
			string result;
			try
			{
				Dictionary<string, object> objectDictionary = this.GetObjectDictionary();
				result = Json.Serialize(objectDictionary);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while Jsoning", inner);
			}
			return result;
		}

		public override Dictionary<string, object> GetObjectDictionary()
		{
			Dictionary<string, object> result;
			try
			{
				result = new Dictionary<string, object>
				{
					{
						"sku",
						this.Sku
					},
					{
						"productType",
						this.ProductType
					},
					{
						"price",
						this.Price
					},
					{
						"title",
						this.Title
					},
					{
						"description",
						this.Description
					},
					{
						"smallIconUrl",
						this.SmallIconUrl
					}
				};
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
			return result;
		}

		public static ProductData CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			ProductData result;
			try
			{
				if (jsonMap == null)
				{
					result = null;
				}
				else
				{
					ProductData productData = new ProductData();
					if (jsonMap.ContainsKey("sku"))
					{
						productData.Sku = (string)jsonMap["sku"];
					}
					if (jsonMap.ContainsKey("productType"))
					{
						productData.ProductType = (string)jsonMap["productType"];
					}
					if (jsonMap.ContainsKey("price"))
					{
						productData.Price = (string)jsonMap["price"];
					}
					if (jsonMap.ContainsKey("title"))
					{
						productData.Title = (string)jsonMap["title"];
					}
					if (jsonMap.ContainsKey("description"))
					{
						productData.Description = (string)jsonMap["description"];
					}
					if (jsonMap.ContainsKey("smallIconUrl"))
					{
						productData.SmallIconUrl = (string)jsonMap["smallIconUrl"];
					}
					result = productData;
				}
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
			return result;
		}

		public static ProductData CreateFromJson(string jsonMessage)
		{
			ProductData result;
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				result = ProductData.CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
			return result;
		}

		public static Dictionary<string, ProductData> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, ProductData> dictionary = new Dictionary<string, ProductData>();
			foreach (KeyValuePair<string, object> keyValuePair in jsonMap)
			{
				ProductData value = ProductData.CreateFromDictionary(keyValuePair.Value as Dictionary<string, object>);
				dictionary.Add(keyValuePair.Key, value);
			}
			return dictionary;
		}

		public static List<ProductData> ListFromJson(List<object> array)
		{
			List<ProductData> list = new List<ProductData>();
			foreach (object obj in array)
			{
				list.Add(ProductData.CreateFromDictionary(obj as Dictionary<string, object>));
			}
			return list;
		}
	}
}
