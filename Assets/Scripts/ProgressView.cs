using System;

public class ProgressView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length > 0)
		{
			string text = parameters[0].ToString();
			if (string.IsNullOrEmpty(text))
			{
				this.title.gameObject.SetActive(false);
			}
			else
			{
				this.title.text = text;
			}
		}
		else
		{
			this.title.gameObject.SetActive(false);
		}
	}

	public UILabel title;
}
