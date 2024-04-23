using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.Analytics;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace Shared.FeatureManager.Module.Merging
{
    public class FeatureMergeUtil : IFeatureMergeUtil
    {
        public FeatureMergeUtil(IFeatureManagerAnalytics featureManagerAnalytics)
        {
            this.featureManagerAnalytics = featureManagerAnalytics;
        }

        public List<ActivatedFeatureInstanceData> MergeMultiple(IFeatureTypeHandler featureTypeHandler, FeatureTypeData current, FeatureTypeData cloud)
        {
            List<ActivatedFeatureInstanceData> result;
            try
            {
                List<ActivatedFeatureInstanceData> list = new List<ActivatedFeatureInstanceData>();
                List<ActivatedFeatureInstanceData> activatedFeatureInstanceDatas = current.ActivatedFeatureInstanceDatas;
                List<ActivatedFeatureInstanceData> activatedFeatureInstanceDatas2 = cloud.ActivatedFeatureInstanceDatas;
                if (!featureTypeHandler.AllowMultipleFeatureInstances)
                {
                    if ((activatedFeatureInstanceDatas != null && activatedFeatureInstanceDatas.Count > 1) || (activatedFeatureInstanceDatas2 != null && activatedFeatureInstanceDatas2.Count > 1))
                    {
                        throw new Exception("Single instance feature handler has more than one instance!");
                    }
                    if (activatedFeatureInstanceDatas != null && activatedFeatureInstanceDatas2 != null && activatedFeatureInstanceDatas.Count == 1 && activatedFeatureInstanceDatas2.Count == 1 && activatedFeatureInstanceDatas[0] != null && activatedFeatureInstanceDatas2[0] != null)
                    {
                        ActivatedFeatureInstanceData item = this.MergeSingle(featureTypeHandler, activatedFeatureInstanceDatas[0], activatedFeatureInstanceDatas2[0]);
                        list.Add(item);
                    }
                    else if (activatedFeatureInstanceDatas != null && activatedFeatureInstanceDatas.Count == 1 && activatedFeatureInstanceDatas[0] != null)
                    {
                        list.Add(activatedFeatureInstanceDatas[0]);
                    }
                    else if (activatedFeatureInstanceDatas2 != null && activatedFeatureInstanceDatas2.Count == 1 && activatedFeatureInstanceDatas2[0] != null)
                    {
                        list.Add(activatedFeatureInstanceDatas2[0]);
                    }
                }
                else
                {
                    Dictionary<ActivatedFeatureInstanceData, ActivatedFeatureInstanceData> dictionary = new Dictionary<ActivatedFeatureInstanceData, ActivatedFeatureInstanceData>();
                    for (int i = activatedFeatureInstanceDatas.Count - 1; i >= 0; i--)
                    {
                        ActivatedFeatureInstanceData activatedFeatureInstanceData = activatedFeatureInstanceDatas[i];
                        for (int j = activatedFeatureInstanceDatas2.Count - 1; j >= 0; j--)
                        {
                            ActivatedFeatureInstanceData activatedFeatureInstanceData2 = activatedFeatureInstanceDatas2[j];
                            if (activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.Id == activatedFeatureInstanceData2.FeatureInstanceActivationData.ActivatedFeatureData.Id)
                            {
                                dictionary.Add(activatedFeatureInstanceData, activatedFeatureInstanceData2);
                                activatedFeatureInstanceDatas.Remove(activatedFeatureInstanceData);
                                activatedFeatureInstanceDatas2.Remove(activatedFeatureInstanceData2);
                            }
                        }
                    }
                    list.AddRange(current.ActivatedFeatureInstanceDatas);
                    list.AddRange(cloud.ActivatedFeatureInstanceDatas);
                    foreach (KeyValuePair<ActivatedFeatureInstanceData, ActivatedFeatureInstanceData> keyValuePair in dictionary)
                    {
                        ActivatedFeatureInstanceData item2 = this.MergeSingle(featureTypeHandler, keyValuePair.Key, keyValuePair.Value);
                        list.Add(item2);
                    }
                }
                FeatureMergeUtil.CleanFeatureList(featureTypeHandler, list);
                result = list;
            }
            catch (Exception exception)
            {
                this.featureManagerAnalytics.LogMergeMultipleFailed(featureTypeHandler, current, cloud, exception);
                throw;
            }
            return result;
        }

        public ActivatedFeatureInstanceData MergeSingle(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData current, ActivatedFeatureInstanceData cloud)
        {
            ActivatedFeatureInstanceData result;
            try
            {
                if (current == null)
                {
                    result = FeatureMergeUtil.CleanFeature(featureTypeHandler, cloud);
                }
                else if (cloud == null)
                {
                    result = FeatureMergeUtil.CleanFeature(featureTypeHandler, current);
                }
                else
                {
                    int timeLeftToFeatureDurationEnd = TactileModules.FeatureManager.FeatureManager.Instance.GetTimeLeftToFeatureDurationEnd(featureTypeHandler, current);
                    int timeLeftToFeatureDurationEnd2 = TactileModules.FeatureManager.FeatureManager.Instance.GetTimeLeftToFeatureDurationEnd(featureTypeHandler, cloud);
                    ActivatedFeatureInstanceData activatedFeatureInstanceData = (timeLeftToFeatureDurationEnd <= timeLeftToFeatureDurationEnd2) ? cloud : current;
                    FeatureInstanceCustomData customInstanceData = current.GetCustomInstanceData();
                    FeatureInstanceCustomData customInstanceData2 = cloud.GetCustomInstanceData();
                    if (current.FeatureInstanceActivationData.ActivatedFeatureData.Id == cloud.FeatureInstanceActivationData.ActivatedFeatureData.Id)
                    {
                        FeatureInstanceCustomData featureInstanceCustomData = FeatureHandlerInvokers.NewFeatureInstanceCustomData(featureTypeHandler, new FeatureData());
                        FeatureHandlerInvokers.MergeFeatureInstanceStates(featureTypeHandler, ref featureInstanceCustomData, customInstanceData, customInstanceData2);
                        ActivatedFeatureInstanceData feature = new ActivatedFeatureInstanceData(featureInstanceCustomData, activatedFeatureInstanceData.FeatureInstanceActivationData);
                        result = FeatureMergeUtil.CleanFeature(featureTypeHandler, feature);
                    }
                    else
                    {
                        bool flag = timeLeftToFeatureDurationEnd > 0;
                        bool flag2 = timeLeftToFeatureDurationEnd2 > 0;
                        IEndPreviousFeature endPreviousFeature = featureTypeHandler as IEndPreviousFeature;
                        if (flag && flag2)
                        {
                            this.featureManagerAnalytics.LogMergingMultipleActiveInstancesOfSingleFeature(new StackTrace(), featureTypeHandler, current.Id, cloud.Id);
                            FeatureInstanceCustomData featureInstanceCustomData2 = FeatureHandlerInvokers.NewFeatureInstanceCustomData(featureTypeHandler, new FeatureData());
                            FeatureHandlerInvokers.MergeFeatureInstanceStates(featureTypeHandler, ref featureInstanceCustomData2, customInstanceData, customInstanceData2);
                            ActivatedFeatureInstanceData feature2 = new ActivatedFeatureInstanceData(featureInstanceCustomData2, activatedFeatureInstanceData.FeatureInstanceActivationData);
                            result = FeatureMergeUtil.CleanFeature(featureTypeHandler, feature2);
                        }
                        else if (flag && !flag2)
                        {
                            if (endPreviousFeature != null)
                            {
                                FeatureMergeUtil.EndOld(endPreviousFeature, cloud);
                            }
                            result = FeatureMergeUtil.CleanFeature(featureTypeHandler, current);
                        }
                        else if (!flag && flag2)
                        {
                            if (endPreviousFeature != null)
                            {
                                FeatureMergeUtil.EndOld(endPreviousFeature, current);
                            }
                            result = FeatureMergeUtil.CleanFeature(featureTypeHandler, cloud);
                        }
                        else
                        {
                            FeatureData activatedFeatureData = current.FeatureInstanceActivationData.ActivatedFeatureData;
                            FeatureData activatedFeatureData2 = cloud.FeatureInstanceActivationData.ActivatedFeatureData;
                            if (activatedFeatureData.StartUnixTime > activatedFeatureData2.StartUnixTime)
                            {
                                if (endPreviousFeature != null)
                                {
                                    FeatureMergeUtil.EndOld(endPreviousFeature, cloud);
                                }
                                result = FeatureMergeUtil.CleanFeature(featureTypeHandler, current);
                            }
                            else
                            {
                                if (endPreviousFeature != null)
                                {
                                    FeatureMergeUtil.EndOld(endPreviousFeature, current);
                                }
                                result = FeatureMergeUtil.CleanFeature(featureTypeHandler, cloud);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.featureManagerAnalytics.LogMergeSingleFailed(featureTypeHandler, current, cloud, exception);
                throw;
            }
            return result;
        }

        private static void EndOld(IEndPreviousFeature endPreviousFeature, ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            if (TactileModules.FeatureManager.FeatureManager.Instance.IsFeatureDeactivated(activatedFeatureInstanceData.FeatureData))
            {
                return;
            }
            endPreviousFeature.EndPreviousInstanceOfFeature(activatedFeatureInstanceData);
        }

        private static ActivatedFeatureInstanceData CleanFeature(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData feature)
        {
            if (feature == null || feature.FeatureInstanceActivationData == null)
            {
                return null;
            }
            FeatureData activatedFeatureData = feature.FeatureInstanceActivationData.ActivatedFeatureData;
            if (TactileModules.FeatureManager.FeatureManager.Instance.IsFeatureDeactivated(activatedFeatureData))
            {
                TactileAnalytics.Instance.LogEvent(new FeatureDeactivatedFromMerge(activatedFeatureData), -1.0, null);
                return null;
            }
            FeatureData feature2 = FeatureMergeUtil.GetFeature(featureTypeHandler, activatedFeatureData.Id);
            if (feature2 != null)
            {
                feature.FeatureInstanceActivationData.ActivatedFeatureData = feature2;
            }
            return feature;
        }

        private static void CleanFeatureList(IFeatureTypeHandler featureTypeHandler, List<ActivatedFeatureInstanceData> features)
        {
            for (int i = features.Count - 1; i >= 0; i--)
            {
                ActivatedFeatureInstanceData activatedFeatureInstanceData = features[i];
                if (activatedFeatureInstanceData == null || activatedFeatureInstanceData.FeatureInstanceActivationData == null)
                {
                    features.RemoveAt(i);
                }
                else
                {
                    FeatureData activatedFeatureData = activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData;
                    if (TactileModules.FeatureManager.FeatureManager.Instance.IsFeatureDeactivated(activatedFeatureData))
                    {
                        TactileAnalytics.Instance.LogEvent(new FeatureDeactivatedFromMerge(activatedFeatureData), -1.0, null);
                        features.Remove(activatedFeatureInstanceData);
                    }
                    else
                    {
                        FeatureData feature = FeatureMergeUtil.GetFeature(featureTypeHandler, activatedFeatureData.Id);
                        if (feature != null)
                        {
                            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData = feature;
                        }
                    }
                }
            }
        }

        private static FeatureData GetFeature(IFeatureTypeHandler featureTypeHandler, string id)
        {
            List<FeatureData> list = new List<FeatureData>(TactileModules.FeatureManager.FeatureManager.Instance.GetAllCloudAvailableFeatures());
            list.AddRange(TactileModules.FeatureManager.FeatureManager.Instance.GetAllCloudUnavailableFeatures());
            foreach (FeatureData featureData in list)
            {
                if (featureData.Id == id && featureData.Type == featureTypeHandler.FeatureType)
                {
                    return featureData;
                }
            }
            return null;
        }

        private readonly IFeatureManagerAnalytics featureManagerAnalytics;
    }
}
