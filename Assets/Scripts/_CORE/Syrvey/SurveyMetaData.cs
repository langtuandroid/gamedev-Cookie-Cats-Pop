using System;
using System.Collections.Generic;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

public class SurveyMetaData : FeatureMetaData
{
	[Required]
	[Description("Name of the survey. Only for internal usage")]
	[JsonSerializable("Name", null)]
	public string Name { get; set; }

	[Required]
	[Description("Text shown on the survey intro view")]
	[JsonSerializable("Intro", typeof(SurveyIntroData))]
	public SurveyIntroData Intro { get; set; }

	[Required]
	[Description("List of pages in survey")]
	[JsonSerializable("Groups", typeof(SurveyGroupData))]
	public List<SurveyGroupData> Groups { get; set; }

	[JsonSerializable("Reward", typeof(SurveyRewardData))]
	public SurveyRewardData Reward { get; set; }
}
