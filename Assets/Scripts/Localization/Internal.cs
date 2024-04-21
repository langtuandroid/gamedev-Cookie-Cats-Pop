using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Localization
{
	internal class Internal
	{
		private Internal()
		{
			this.UpdateSettings(null);
		}

		public LanguageCode CurrentLanguageCode { get; private set; }

		public CultureInfo CurrentCultureInfo { get; private set; }

		public char CurrentThousandSeperator { get; private set; }

		public void UpdateSettings(AssetBundle assetBundle)
		{
			this.CurrentLanguageCode = LanguageCode.N;
			this.CurrentCultureInfo = null;
			this.CurrentThousandSeperator = ',';
			this.currentLanguage = new Hashtable();
			LanguageCode item = Internal.LanguageNameToCode(Application.systemLanguage);
			LanguageCode languageCode = LanguageCode.N;
			List<LanguageCode> list = new List<LanguageCode>();
			string value = item.ToString() + "_" + SystemInfoHelper.GetLocaleCountryCode();
			if (Enum.IsDefined(typeof(LanguageCode), value))
			{
				list.Add((LanguageCode)Enum.Parse(typeof(LanguageCode), value));
			}
			list.Add(item);
			list.Add(LanguageCode.EN);
			TextAsset textAsset = null;
			for (int i = 0; i < list.Count; i++)
			{
				textAsset = this.GetLanguageAssetForCode(list[i], assetBundle);
				if (textAsset != null)
				{
					languageCode = list[i];
					break;
				}
			}
			if (languageCode == LanguageCode.N)
			{
			}
			if (textAsset != null)
			{
				this.CurrentLanguageCode = (LanguageCode)Enum.Parse(typeof(LanguageCode), textAsset.name);
				this.CurrentCultureInfo = this.GetCultureInfoFromLanguageCode(this.CurrentLanguageCode);
				if (this.CurrentCultureInfo != null && this.CurrentCultureInfo.NumberFormat.NumberGroupSeparator.Length == 1)
				{
					this.CurrentThousandSeperator = this.CurrentCultureInfo.NumberFormat.NumberGroupSeparator[0];
				}
				this.currentLanguage = textAsset.text.hashtableFromJson();
			}
		}

		public static Internal Instance
		{
			get
			{
				if (Internal.instance == null)
				{
					Internal.instance = new Internal();
				}
				return Internal.instance;
			}
		}

		public string FormatNumber(int number)
		{
			return number.ToString("N0", this.CurrentCultureInfo);
		}

		public string FormatNumber(long number)
		{
			return number.ToString("N0", this.CurrentCultureInfo);
		}

		public string Get(string key)
		{
			if (!this.currentLanguage.ContainsKey(key))
			{
				return key;
			}
			if ((string)this.currentLanguage[key] == "[EMPTY]")
			{
				return string.Empty;
			}
			string text = (string)this.currentLanguage[key];
			if (EastAsiaFormatHelper.IsEastAsiaLanguage(this.CurrentLanguageCode))
			{
				text = EastAsiaFormatHelper.ApplyEastAsiaWordWrapping(text, this.CurrentLanguageCode);
			}
			return text;
		}

		private TextAsset GetLanguageAssetForCode(LanguageCode languageCode, AssetBundle assetBundle)
		{
			string text = languageCode.ToString();
			if (assetBundle != null && assetBundle.Contains(text))
			{
				TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(text);
				if (textAsset != null)
				{
					return textAsset;
				}
			}
			return Resources.Load("LocalizedLanguages/" + text) as TextAsset;
		}

		private CultureInfo GetCultureInfoFromLanguageCode(LanguageCode languageCode)
		{
			CultureInfo cultureInfo = null;
			switch (languageCode)
			{
			case LanguageCode.DA:
				cultureInfo = Internal.GetCultureInfoFromIetfTag("da-DK");
				break;
			case LanguageCode.DE:
				cultureInfo = Internal.GetCultureInfoFromIetfTag("de-DE");
				break;
			default:
				if (languageCode != LanguageCode.NL)
				{
					if (languageCode != LanguageCode.NO)
					{
						if (languageCode != LanguageCode.FR)
						{
							if (languageCode != LanguageCode.IT)
							{
								if (languageCode != LanguageCode.PL)
								{
									if (languageCode != LanguageCode.SV)
									{
										if (languageCode == LanguageCode.TR)
										{
											cultureInfo = Internal.GetCultureInfoFromIetfTag("tr-TR");
										}
									}
									else
									{
										cultureInfo = Internal.GetCultureInfoFromIetfTag("sv-SE");
									}
								}
								else
								{
									cultureInfo = Internal.GetCultureInfoFromIetfTag("pl-PL");
								}
							}
							else
							{
								cultureInfo = Internal.GetCultureInfoFromIetfTag("it-IT");
							}
						}
						else
						{
							cultureInfo = Internal.GetCultureInfoFromIetfTag("fr-FR");
						}
					}
					else
					{
						cultureInfo = Internal.GetCultureInfoFromIetfTag("nb-NO");
					}
				}
				else
				{
					cultureInfo = Internal.GetCultureInfoFromIetfTag("nl-NL");
				}
				break;
			case LanguageCode.EN:
				cultureInfo = Internal.GetCultureInfoFromIetfTag("en-US");
				break;
			case LanguageCode.ES:
				cultureInfo = Internal.GetCultureInfoFromIetfTag("es-ES");
				break;
			}
			if (cultureInfo == null)
			{
				cultureInfo = Internal.GetCultureInfoFromIetfTag("en-US");
				if (cultureInfo == null)
				{
					cultureInfo = Internal.GetCultureInfoFromIetfTag("en-GB");
					if (cultureInfo == null)
					{
						cultureInfo = CultureInfo.InvariantCulture;
					}
				}
			}
			return cultureInfo;
		}

		private static CultureInfo GetCultureInfoFromIetfTag(string tag)
		{
			foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				if (cultureInfo.Name.Equals(tag))
				{
					return cultureInfo;
				}
			}
			return CultureInfo.InvariantCulture;
		}

		public static LanguageCode LanguageNameToCode(SystemLanguage name)
		{
			if (name == SystemLanguage.Afrikaans)
			{
				return LanguageCode.AF;
			}
			if (name == SystemLanguage.Arabic)
			{
				return LanguageCode.AR;
			}
			if (name == SystemLanguage.Basque)
			{
				return LanguageCode.BA;
			}
			if (name == SystemLanguage.Belarusian)
			{
				return LanguageCode.BE;
			}
			if (name == SystemLanguage.Bulgarian)
			{
				return LanguageCode.BG;
			}
			if (name == SystemLanguage.Catalan)
			{
				return LanguageCode.CA;
			}
			if (name == SystemLanguage.Chinese)
			{
				return LanguageCode.ZH;
			}
			if (name == SystemLanguage.ChineseSimplified)
			{
				return LanguageCode.ZH;
			}
			if (name == SystemLanguage.Czech)
			{
				return LanguageCode.CS;
			}
			if (name == SystemLanguage.Danish)
			{
				return LanguageCode.DA;
			}
			if (name == SystemLanguage.Dutch)
			{
				return LanguageCode.NL;
			}
			if (name == SystemLanguage.English)
			{
				return LanguageCode.EN;
			}
			if (name == SystemLanguage.Estonian)
			{
				return LanguageCode.ET;
			}
			if (name == SystemLanguage.Faroese)
			{
				return LanguageCode.FA;
			}
			if (name == SystemLanguage.Finnish)
			{
				return LanguageCode.FI;
			}
			if (name == SystemLanguage.French)
			{
				return LanguageCode.FR;
			}
			if (name == SystemLanguage.German)
			{
				return LanguageCode.DE;
			}
			if (name == SystemLanguage.Greek)
			{
				return LanguageCode.EL;
			}
			if (name == SystemLanguage.Hebrew)
			{
				return LanguageCode.HE;
			}
			if (name == SystemLanguage.Hungarian)
			{
				return LanguageCode.HU;
			}
			if (name == SystemLanguage.Icelandic)
			{
				return LanguageCode.IS;
			}
			if (name == SystemLanguage.Indonesian)
			{
				return LanguageCode.ID;
			}
			if (name == SystemLanguage.Italian)
			{
				return LanguageCode.IT;
			}
			if (name == SystemLanguage.Japanese)
			{
				return LanguageCode.JA;
			}
			if (name == SystemLanguage.Korean)
			{
				return LanguageCode.KO;
			}
			if (name == SystemLanguage.Latvian)
			{
				return LanguageCode.LA;
			}
			if (name == SystemLanguage.Lithuanian)
			{
				return LanguageCode.LT;
			}
			if (name == SystemLanguage.Norwegian)
			{
				return LanguageCode.NO;
			}
			if (name == SystemLanguage.Polish)
			{
				return LanguageCode.PL;
			}
			if (name == SystemLanguage.Portuguese)
			{
				return LanguageCode.PT;
			}
			if (name == SystemLanguage.Romanian)
			{
				return LanguageCode.RO;
			}
			if (name == SystemLanguage.Russian)
			{
				return LanguageCode.RU;
			}
			if (name == SystemLanguage.SerboCroatian)
			{
				return LanguageCode.SH;
			}
			if (name == SystemLanguage.Slovak)
			{
				return LanguageCode.SK;
			}
			if (name == SystemLanguage.Slovenian)
			{
				return LanguageCode.SL;
			}
			if (name == SystemLanguage.Spanish)
			{
				return LanguageCode.ES;
			}
			if (name == SystemLanguage.Swedish)
			{
				return LanguageCode.SV;
			}
			if (name == SystemLanguage.Thai)
			{
				return LanguageCode.TH;
			}
			if (name == SystemLanguage.Turkish)
			{
				return LanguageCode.TR;
			}
			if (name == SystemLanguage.Ukrainian)
			{
				return LanguageCode.UK;
			}
			if (name == SystemLanguage.Vietnamese)
			{
				return LanguageCode.VI;
			}
			if (name == SystemLanguage.Hungarian)
			{
				return LanguageCode.HU;
			}
			if (name == SystemLanguage.Unknown)
			{
				return LanguageCode.N;
			}
			return LanguageCode.N;
		}

		private static Internal instance;

		private Hashtable currentLanguage;
	}
}
