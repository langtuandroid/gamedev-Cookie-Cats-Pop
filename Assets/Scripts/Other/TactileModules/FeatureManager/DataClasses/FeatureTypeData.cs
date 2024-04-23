using System;
using System.Collections;
using System.Collections.Generic;
using ConfigSchema;
using JetBrains.Annotations;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.DataClasses
{
	public sealed class FeatureTypeData
	{
		[Obsolete("Use non empty constructor instead", true)]
		public FeatureTypeData()
		{
		}

		public FeatureTypeData([NotNull] FeatureTypeCustomData customData)
		{
			if (customData == null)
			{
				throw new ArgumentNullException("customData");
			}
			this.InternalFeatureTypeCustomData = new Hashtable();
			this.ActivatedFeatureInstanceDatas = new List<ActivatedFeatureInstanceData>();
			this.FeatureTypeCustomData = customData;
		}

		[JsonSerializable("fhpsb", null)]
		[Description("Serialized instance of the featyre type's custom data.")]
		private Hashtable InternalFeatureTypeCustomData { get; set; }

		[JsonSerializable("cdt", null)]
		[Description("The type of the feature type's custom data.")]
		public string CustomDataTypeAsString { get; set; }

		public Type CustomDataType
		{
			get
			{
				return string.IsNullOrEmpty(this.CustomDataTypeAsString) ? null : Type.GetType(this.CustomDataTypeAsString);
			}
		}

		[JsonSerializable("afid", typeof(ActivatedFeatureInstanceData))]
		[Description("List of custom data of the instances of this feature type.")]
		public List<ActivatedFeatureInstanceData> ActivatedFeatureInstanceDatas { get; set; }

		public FeatureTypeCustomData FeatureTypeCustomData { private get; set; }

		[JsonPreSerialize]
		private void Serialize()
		{
			if (this.CustomDataType == null && !string.IsNullOrEmpty(this.CustomDataTypeAsString))
			{
				return;
			}
			if (this.FeatureTypeCustomData == null)
			{
				this.InternalFeatureTypeCustomData = new Hashtable();
				this.CustomDataTypeAsString = typeof(FeatureTypeCustomData).AssemblyQualifiedName;
			}
			else
			{
				this.InternalFeatureTypeCustomData = JsonSerializer.ObjectToHashtable(this.FeatureTypeCustomData);
				this.CustomDataTypeAsString = this.FeatureTypeCustomData.GetType().AssemblyQualifiedName;
			}
		}

		[JsonPostDeserialize]
		private void Deserialize()
		{
			if (this.CustomDataType == null && !string.IsNullOrEmpty(this.CustomDataTypeAsString))
			{
				return;
			}
			this.FeatureTypeCustomData = (((FeatureTypeCustomData)JsonSerializer.HashtableToObject(this.CustomDataType, this.InternalFeatureTypeCustomData, JsonSerializer.SerializationType.WithPreAndPostCallbacks)) ?? new FeatureTypeCustomData());
		}

		public V GetFeatureTypeCustomData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			if (this.FeatureTypeCustomData == null)
			{
				return (V)((object)null);
			}
			return (V)((object)this.FeatureTypeCustomData);
		}

		public FeatureTypeCustomData GetFeatureTypeCustomData()
		{
			return this.FeatureTypeCustomData;
		}
	}
}
