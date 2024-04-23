using System;
using System.Collections;
using ConfigSchema;
using JetBrains.Annotations;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.DataClasses
{
	public sealed class ActivatedFeatureInstanceData
	{
		[Obsolete("Use constructor with parameters instead", true)]
		public ActivatedFeatureInstanceData()
		{
		}

		public ActivatedFeatureInstanceData([NotNull] FeatureInstanceCustomData featureInstanceCustomData, [NotNull] FeatureInstanceActivationData featureInstanceActivationData)
		{
			if (featureInstanceCustomData == null)
			{
				throw new ArgumentNullException("featureInstanceCustomData");
			}
			if (featureInstanceActivationData == null)
			{
				throw new ArgumentNullException("featureInstanceActivationData");
			}
			this.FeatureInstanceCustomData = new Hashtable();
			this.featureInstanceCustomData = featureInstanceCustomData;
			this.FeatureInstanceActivationData = featureInstanceActivationData;
		}

		[JsonSerializable("ficd", null)]
		[Description("Serialized instance of the custom data for this feature instance.")]
		private Hashtable FeatureInstanceCustomData { get; set; }

		[JsonSerializable("cdt", null)]
		[Description("The type of the custom data for this feature instance.")]
		public string CustomDataTypeAsString { get; set; }

		public Type CustomDataType
		{
			get
			{
				return string.IsNullOrEmpty(this.CustomDataTypeAsString) ? null : Type.GetType(this.CustomDataTypeAsString);
			}
		}

		[JsonSerializable("fiad", null)]
		[Description("The activation data for this feature instance.")]
		public FeatureInstanceActivationData FeatureInstanceActivationData { get; set; }

		public FeatureData FeatureData
		{
			get
			{
				return this.FeatureInstanceActivationData.ActivatedFeatureData;
			}
		}

		public string Id
		{
			get
			{
				return this.FeatureData.Id;
			}
		}

		[JsonPreSerialize]
		private void Serialize()
		{
			if (this.CustomDataType == null && !string.IsNullOrEmpty(this.CustomDataTypeAsString))
			{
				return;
			}
			if (this.featureInstanceCustomData == null)
			{
				this.FeatureInstanceCustomData = new Hashtable();
				this.CustomDataTypeAsString = typeof(FeatureInstanceCustomData).AssemblyQualifiedName;
			}
			else
			{
				this.FeatureInstanceCustomData = JsonSerializer.ObjectToHashtable(this.featureInstanceCustomData);
				this.CustomDataTypeAsString = this.featureInstanceCustomData.GetType().AssemblyQualifiedName;
			}
		}

		[JsonPostDeserialize]
		private void Deserialize()
		{
			if (this.CustomDataType == null && !string.IsNullOrEmpty(this.CustomDataTypeAsString))
			{
				return;
			}
			this.featureInstanceCustomData = (((FeatureInstanceCustomData)JsonSerializer.HashtableToObject(this.CustomDataType, this.FeatureInstanceCustomData, JsonSerializer.SerializationType.WithPreAndPostCallbacks)) ?? new FeatureInstanceCustomData());
		}

		public T GetCustomInstanceData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			return (T)((object)this.featureInstanceCustomData);
		}

		public FeatureInstanceCustomData GetCustomInstanceData()
		{
			return this.featureInstanceCustomData;
		}

		public U GetMetaData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			return FeatureManager.Instance.GetFeatureInstanceMetaData<T, U, V>(featureTypeHandler, this.FeatureData);
		}

		public U GetMetaData<U>() where U : FeatureMetaData
		{
			return FeatureManager.Instance.GetFeatureInstanceMetaData<U>(this.FeatureData);
		}

		public V GetFeatureTypeCustomData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			return FeatureManager.Instance.GetFeatureTypeCustomData<T, U, V>(featureTypeHandler);
		}

		private FeatureInstanceCustomData featureInstanceCustomData;
	}
}
