using System;

public class ExtensibleView<T> : UIView, IExtensibleVisual
{
	protected T Extension
	{
		get
		{
			if (this.cachedInstance == null)
			{
				this.cachedInstance = base.GetComponent<T>();
			}
			return this.cachedInstance;
		}
	}

	private T cachedInstance;
}
