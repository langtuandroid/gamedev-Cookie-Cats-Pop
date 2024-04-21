using System;

public class Constants
{
	private static string Select(string localBuildValue, string adhocBuildValue, string appStoreBuildValue)
	{
		return Constants.Select(localBuildValue, adhocBuildValue, appStoreBuildValue, appStoreBuildValue);
	}

	private static string Select(string localBuildValue, string adhocBuildValue, string appStoreBuildValue, string appStoreBetaBuild)
	{
		return appStoreBuildValue;
	}

	public static string ITUNES_ID = "1159705605";

	public static string SHARED_GAME_SECRET = "bc9ae221-6914-40b6-b99e-934f0d5ca131";

	public static string FACEBOOK_LIFE_OBJECT_ID = Constants.Select("1817726915137194", "1817726915137194", "130266010793534");

	public static string FACEBOOK_KEY_OBJECT_ID = Constants.Select("1344468272264828", "1344468272264828", "947742295356757");

	public static string FACEBOOK_INVITE_OBJECT_ID = Constants.Select(string.Empty, string.Empty, string.Empty);

	public static string FACEBOOK_TOURNAMENT_LIFE_OBJECT_ID = Constants.Select(string.Empty, string.Empty, string.Empty);

	public static string FACEBOOK_APP_DISPLAY_NAME = Constants.Select("Cookie Cats Pop Dev", "Cookie Cats Pop Dev", "Cookie Cats Pop");

	public static string FACEBOOK_APP_ID = Constants.Select("1259846577366747", "1259846577366747", "1259846127366792");

	public static string FACEBOOK_APP_NAMESPACE = Constants.Select("cookie-cats-pop-dev", "cookie-cats-pop-dev", "cookie-cats-pop");

	public static string FACEBOOK_REDIRECT_URL = string.Empty;

	public static string FACEBOOK_OPENGRAPH_VERSION = "v2.8";

	public static string FACEBOOK_URL_SUFFIX = Constants.Select("local", "inhouse", string.Empty);

	public static string FACEBOOK_CANVAS_PAY_PREFIX = "http://static.tactile.dk/CookieCatsPop/Canvas/IAP/";

	public static string APS_ENVIRONMENT = Constants.Select("dev", "prod", "prod");

	public static string CROSS_PROMOTION_CAMPAIGN_CONTEXT = "CCP";

	public static string CLOUD_URL = Constants.Select("https://cookie-cats-pop-dev.tactilews.com", "https://cookie-cats-pop-dev.tactilews.com", "https://cookiecatspop.tactilews.com");

	public static string TACTILE_ANALYTICS_APP_ID = Constants.Select("cookiecatspopdev", "cookiecatspopdev", "cookiecatspop");

	public static string ONESIGNAL_APP_ID = Constants.Select("5ebf82b1-f152-48bf-8d72-1629e05a9a9b", "fdc0113d-19a4-42ed-ab5e-4c04ff46e965", "b45394f9-a169-4722-9246-e8c19080f827");

	public static string GCM_SENDER_ID = "520678004470";

	public static string APPLOVINMAX_SDK_KEY = Constants.Select("Gb5Nx6-B14-sbRht_1ysyiN80F2HkIz_t9w_W76ndggtF5c7AP2_G4ybwtnCV4IN1df1xtLi9u5BcufOgHMnBV", "Gb5Nx6-B14-sbRht_1ysyiN80F2HkIz_t9w_W76ndggtF5c7AP2_G4ybwtnCV4IN1df1xtLi9u5BcufOgHMnBV", "8xX9sXFzaDuETBSaxR5ASmzEW2BpUsdcoAneh1bHFUWofzNbPSmsDk5kmGXF79twJdT0e9HuepgIm2v9Ve0iN5");

	public static string ANDROID_TARGET_SDK = "28";

	public static string ANDROID_COMPILE_SDK = "28";

	public static string ANDROID_BUILD_TOOLS = "28.0.3";

	public static string APPLOVINMAX_INTERSTITIAL_AD_UNIT_ID = Constants.Select(string.Empty, "5872e8daed40c02f", "140e428b1beb790f");

	public static string APPLOVINMAX_REWARDED_VIDEO_AD_UNIT_ID = Constants.Select(string.Empty, "7c5ff717114e8277", "0ffdeba92616bb99");

	public static string APPLOVINMAX_GOOGLE_ADMOB_APPLICATION_IDENTIFIER = Constants.Select("ca-app-pub-6756654155219279~8935050740", "ca-app-pub-6756654155219279~8935050740", "ca-app-pub-6756654155219279~8935050740");

	public static string HOCKEYAPP_APP_ID = Constants.Select(string.Empty, "27030480a1c74277b4a40a7914e38974", "4654dd19c5884eef8c1ff33c366f6d4d", "4654dd19c5884eef8c1ff33c366f6d4d");

	public static string ANDROID_PUBLIC_KEY = Constants.Select("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAw10gjcZB0jL46cFBugEiDVaDTdAgL+Jeg37SbKjatTICva93ZweJMzuwEg+dt1hZlS0aFRLovBuDV3idi5kpCobr0nuVBGUmxDBKmB5G7GiZNB7yRYDXg3A+9fLRbxEzfr0wYt31ap+hmNj++Q5EU+8egHxUfOe4WHiFszNXGBiteBFLWWK6ssg69h4NiP2JAqQsV5ZtrQ+kre7RNFCNl+EvZtdpOl3Ba97Uo0T0JR582al/6LjHaZaNX3OEe6AnrsuF+BmLthEo2xzhssC9fEDDs39rLpL2PSIjWpc4hu7hD1lUa4l2PbJMdltJ/l5I4Md3qhMjAAFekW40UNNYIwIDAQAB", string.Empty, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAogZNqfwJGWxiglJMn+DEbmPAsUd5LeujYLs4LJFIEgyjMhrHwLGPSnJuZDzc1fnTfQaz1y8UYgCJl+Wu78+xB+0/hF12JXE8FkTLbhxT29MhijMFC2l5ntxb0wxDLeVVUTDazpTzUjtPJBfSCoc2YAKXnk0Va71fktnu4zOC/eGv7LdGsScFVx+yFkrFHIc9cxLP+UXVYQe0QPkLCgQ/rP4C+18xKysDrIO5P+0GkVcW5OrGFrrCbREtBiouCzs6v6gRXP11nEO2BKyyYSkGDpWlOch1umG/jaBybLscdq1KJ6Bv+vV0TPmzdSgfm5ke6iUf3sYWQjc0MICWxsSykwIDAQAB");

	public static string ADJUST_IO_SECRET_ID = "1";

	public static string ADJUST_IO_SECRET_PART_1 = "305852584";

	public static string ADJUST_IO_SECRET_PART_2 = "1530674887";

	public static string ADJUST_IO_SECRET_PART_3 = "1982425197";

	public static string ADJUST_IO_SECRET_PART_4 = "877085324";

	public static string GOOGLE_GAMES_ID = "705544591572";
}
