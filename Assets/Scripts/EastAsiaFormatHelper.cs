using System;
using System.Linq;

public class EastAsiaFormatHelper
{
	public static bool IsEastAsiaLanguage(LanguageCode code)
	{
		return code == LanguageCode.ZH || code == LanguageCode.KO || code == LanguageCode.JA;
	}

	public static string ApplyEastAsiaWordWrapping(string value, LanguageCode code)
	{
		string text = string.Empty;
		int num = 0;
		if (code == LanguageCode.ZH)
		{
			foreach (char c in value)
			{
				text += c;
				if (++num >= 4)
				{
					if (!EastAsiaFormatHelper.ChineseForbiddenLineEndChars.Contains(c))
					{
						if (c != value.Last<char>())
						{
							char value2 = value[value.IndexOf(c) + 1];
							if (EastAsiaFormatHelper.ChineseForbiddenLineStartChars.Contains(value2))
							{
								goto IL_B9;
							}
						}
						if (!EastAsiaFormatHelper.IsEnglishCharacter(c))
						{
							if (char.IsWhiteSpace(c))
							{
								num = 0;
							}
							else
							{
								text += '\r';
								num = 0;
							}
						}
					}
				}
				IL_B9:;
			}
		}
		else if (code == LanguageCode.JA)
		{
			foreach (char c2 in value)
			{
				text += c2;
				if (++num >= 4)
				{
					if (!EastAsiaFormatHelper.JapaneseForbiddenLineEndChars.Contains(c2))
					{
						if (c2 != value.Last<char>())
						{
							char value3 = value[value.IndexOf(c2) + 1];
							if (EastAsiaFormatHelper.JapaneseForbiddenLineStartChars.Contains(value3))
							{
								goto IL_1AC;
							}
							if (EastAsiaFormatHelper.JapaneseForbiddenSplitChars.Contains(c2) || EastAsiaFormatHelper.JapaneseForbiddenSplitChars.Contains(value3))
							{
								goto IL_1AC;
							}
						}
						if (!EastAsiaFormatHelper.IsEnglishCharacter(c2))
						{
							if (char.IsWhiteSpace(c2))
							{
								num = 0;
							}
							else
							{
								text += '\r';
								num = 0;
							}
						}
					}
				}
				IL_1AC:;
			}
		}
		else
		{
			text = value;
		}
		return text;
	}

	private static bool IsEnglishCharacter(char c)
	{
		return c < '\u0080';
	}

	private const int MinEastAsiaCharacterLineLength = 4;

	private static readonly char[] ChineseForbiddenLineStartChars = new char[]
	{
		'!',
		'%',
		')',
		',',
		'.',
		':',
		';',
		'?',
		']',
		'}',
		'¢',
		'°',
		'·',
		'’',
		'\'',
		'"',
		'"',
		'†',
		'‡',
		'›',
		'℃',
		'∶',
		'、',
		'。',
		'〃',
		'〆',
		'〕',
		'〗',
		'〞',
		'﹚',
		'﹜',
		'！',
		'＂',
		'％',
		'＇',
		'）',
		'，',
		'．',
		'：',
		'；',
		'？',
		'！',
		'］',
		'｝',
		'～'
	};

	private static readonly char[] ChineseForbiddenLineEndChars = new char[]
	{
		'$',
		'(',
		'£',
		'¥',
		'·',
		'‘',
		'"',
		'〈',
		'《',
		'「',
		'『',
		'【',
		'〔',
		'〖',
		'〝',
		'﹙',
		'﹛',
		'＄',
		'（',
		'．',
		'［',
		'｛',
		'￡',
		'￥'
	};

	private static readonly char[] JapaneseForbiddenLineStartChars = new char[]
	{
		')',
		']',
		'｝',
		'〕',
		'〉',
		'》',
		'」',
		'』',
		'】',
		'〙',
		'〗',
		'〟',
		'’',
		'"',
		'｠',
		'»',
		'ヽ',
		'ヾ',
		'ー',
		'ァ',
		'ィ',
		'ゥ',
		'ェ',
		'ォ',
		'ッ',
		'ャ',
		'ュ',
		'ョ',
		'ヮ',
		'ヵ',
		'ヶ',
		'ぁ',
		'ぃ',
		'ぅ',
		'ぇ',
		'ぉ',
		'っ',
		'ゃ',
		'ゅ',
		'ょ',
		'ゎ',
		'ゕ',
		'ゖ',
		'ㇰ',
		'ㇱ',
		'ㇲ',
		'ㇳ',
		'ㇴ',
		'ㇵ',
		'ㇶ',
		'ㇷ',
		'ㇸ',
		'ㇹ',
		'ㇺ',
		'ㇻ',
		'ㇼ',
		'ㇽ',
		'ㇾ',
		'ㇿ',
		'々',
		'〻',
		'‐',
		'゠',
		'–',
		'〜',
		'?',
		'？',
		'!',
		'！',
		'‼',
		'⁇',
		'⁈',
		'⁉',
		'・',
		'、',
		':',
		';',
		',',
		'。',
		'.'
	};

	private static readonly char[] JapaneseForbiddenLineEndChars = new char[]
	{
		'(',
		'[',
		'｛',
		'〔',
		'〈',
		'《',
		'「',
		'『',
		'【',
		'〘',
		'〖',
		'〝',
		'‘',
		'"',
		'｟',
		'«'
	};

	private static readonly char[] JapaneseForbiddenSplitChars = new char[]
	{
		'—',
		'ー',
		'…',
		'‥',
		'〳',
		'〴',
		'〵'
	};
}
