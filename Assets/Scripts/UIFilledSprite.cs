using System;
using UnityEngine;

[ExecuteInEditMode]
public class UIFilledSprite : UISprite
{
	public UIFilledSprite.FillDirection fillDirection
	{
		get
		{
			return this.mFillDirection;
		}
		set
		{
			if (this.mFillDirection != value)
			{
				this.mFillDirection = value;
				this.changeFlags = UIChangeFlags.All;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return this.mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (this.mFillAmount != num)
			{
				this.mFillAmount = num;
				this.changeFlags = UIChangeFlags.All;
			}
		}
	}

	public bool invert
	{
		get
		{
			return this.mInvert;
		}
		set
		{
			if (this.mInvert != value)
			{
				this.mInvert = value;
				this.changeFlags = UIChangeFlags.All;
			}
		}
	}

	private bool AdjustRadial(Vector2[] xy, Vector2[] uv, float fill, bool invert)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (!invert)
		{
			num = 1f - num;
		}
		num *= 1.57079637f;
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Cos(num);
		if (num2 > num3)
		{
			num3 *= 1f / num2;
			num2 = 1f;
			if (!invert)
			{
				xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
				xy[3].y = xy[0].y;
				uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
				uv[3].y = uv[0].y;
			}
		}
		else if (num3 > num2)
		{
			num2 *= 1f / num3;
			num3 = 1f;
			if (invert)
			{
				xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
				xy[1].x = xy[0].x;
				uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
				uv[1].x = uv[0].x;
			}
		}
		else
		{
			num2 = 1f;
			num3 = 1f;
		}
		if (invert)
		{
			xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
			uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
		}
		else
		{
			xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
			uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
		}
		return true;
	}

	private void Rotate(Vector2[] v, int offset)
	{
		for (int i = 0; i < offset; i++)
		{
			Vector2 vector = new Vector2(v[3].x, v[3].y);
			v[3].x = v[2].y;
			v[3].y = v[2].x;
			v[2].x = v[1].y;
			v[2].y = v[1].x;
			v[1].x = v[0].y;
			v[1].y = v[0].x;
			v[0].x = vector.y;
			v[0].y = vector.x;
		}
	}

	protected override void OnFill(MeshData meshData)
	{
		float x = 0f;
		float y = 0f;
		float num = 1f;
		float num2 = -1f;
		float num3 = this.mOuterUV.xMin;
		float num4 = this.mOuterUV.yMin;
		float num5 = this.mOuterUV.xMax;
		float num6 = this.mOuterUV.yMax;
		if (this.mFillDirection == UIFilledSprite.FillDirection.Horizontal || this.mFillDirection == UIFilledSprite.FillDirection.Vertical)
		{
			float num7 = (num5 - num3) * this.mFillAmount;
			float num8 = (num6 - num4) * this.mFillAmount;
			if (this.fillDirection == UIFilledSprite.FillDirection.Horizontal)
			{
				if (this.mInvert)
				{
					x = 1f - this.mFillAmount;
					num3 = num5 - num7;
				}
				else
				{
					num *= this.mFillAmount;
					num5 = num3 + num7;
				}
			}
			else if (this.fillDirection == UIFilledSprite.FillDirection.Vertical)
			{
				if (this.mInvert)
				{
					num2 *= this.mFillAmount;
					num4 = num6 - num8;
				}
				else
				{
					y = -(1f - this.mFillAmount);
					num6 = num4 + num8;
				}
			}
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		array[0] = new Vector2(num, y);
		array[1] = new Vector2(num, num2);
		array[2] = new Vector2(x, num2);
		array[3] = new Vector2(x, y);
		array2[0] = new Vector2(num5, num6);
		array2[1] = new Vector2(num5, num4);
		array2[2] = new Vector2(num3, num4);
		array2[3] = new Vector2(num3, num6);
		if (this.fillDirection == UIFilledSprite.FillDirection.Radial90)
		{
			if (!this.AdjustRadial(array, array2, this.mFillAmount, this.mInvert))
			{
				return;
			}
		}
		else
		{
			if (this.fillDirection == UIFilledSprite.FillDirection.Radial180)
			{
				Vector2[] array3 = new Vector2[4];
				Vector2[] array4 = new Vector2[4];
				for (int i = 0; i < 2; i++)
				{
					array3[0] = new Vector2(0f, 0f);
					array3[1] = new Vector2(0f, 1f);
					array3[2] = new Vector2(1f, 1f);
					array3[3] = new Vector2(1f, 0f);
					array4[0] = new Vector2(0f, 0f);
					array4[1] = new Vector2(0f, 1f);
					array4[2] = new Vector2(1f, 1f);
					array4[3] = new Vector2(1f, 0f);
					if (this.mInvert)
					{
						if (i > 0)
						{
							this.Rotate(array3, i);
							this.Rotate(array4, i);
						}
					}
					else if (i < 1)
					{
						this.Rotate(array3, 1 - i);
						this.Rotate(array4, 1 - i);
					}
					float num9;
					float num10;
					if (i == 1)
					{
						num9 = ((!this.mInvert) ? 1f : 0.5f);
						num10 = ((!this.mInvert) ? 0.5f : 1f);
					}
					else
					{
						num9 = ((!this.mInvert) ? 0.5f : 1f);
						num10 = ((!this.mInvert) ? 1f : 0.5f);
					}
					array3[1].y = Mathf.Lerp(num9, num10, array3[1].y);
					array3[2].y = Mathf.Lerp(num9, num10, array3[2].y);
					array4[1].y = Mathf.Lerp(num9, num10, array4[1].y);
					array4[2].y = Mathf.Lerp(num9, num10, array4[2].y);
					float fill = this.mFillAmount * 2f - (float)i;
					bool flag = i % 2 == 1;
					if (this.AdjustRadial(array3, array4, fill, !flag))
					{
						if (this.mInvert)
						{
							flag = !flag;
						}
						if (flag)
						{
							for (int j = 0; j < 4; j++)
							{
								num9 = Mathf.Lerp(array[0].x, array[2].x, array3[j].x);
								num10 = Mathf.Lerp(array[0].y, array[2].y, array3[j].y);
								float x2 = Mathf.Lerp(array2[0].x, array2[2].x, array4[j].x);
								float y2 = Mathf.Lerp(array2[0].y, array2[2].y, array4[j].y);
								meshData.verts.Add(new Vector3(num9 * base.Size.x, num10 * base.Size.y, 0f));
								meshData.uvs.Add(new Vector2(x2, y2));
								meshData.colors.Add(base.Color);
							}
						}
						else
						{
							for (int k = 3; k > -1; k--)
							{
								num9 = Mathf.Lerp(array[0].x, array[2].x, array3[k].x);
								num10 = Mathf.Lerp(array[0].y, array[2].y, array3[k].y);
								float x3 = Mathf.Lerp(array2[0].x, array2[2].x, array4[k].x);
								float y3 = Mathf.Lerp(array2[0].y, array2[2].y, array4[k].y);
								meshData.verts.Add(new Vector3(num9 * base.Size.x, num10 * base.Size.y, 0f));
								meshData.uvs.Add(new Vector2(x3, y3));
								meshData.colors.Add(base.Color);
							}
						}
					}
				}
				return;
			}
			if (this.fillDirection == UIFilledSprite.FillDirection.Radial360)
			{
				float[] array5 = new float[]
				{
					0.5f,
					1f,
					0f,
					0.5f,
					0.5f,
					1f,
					0.5f,
					1f,
					0f,
					0.5f,
					0.5f,
					1f,
					0f,
					0.5f,
					0f,
					0.5f
				};
				Vector2[] array6 = new Vector2[4];
				Vector2[] array7 = new Vector2[4];
				for (int l = 0; l < 4; l++)
				{
					array6[0] = new Vector2(0f, 0f);
					array6[1] = new Vector2(0f, 1f);
					array6[2] = new Vector2(1f, 1f);
					array6[3] = new Vector2(1f, 0f);
					array7[0] = new Vector2(0f, 0f);
					array7[1] = new Vector2(0f, 1f);
					array7[2] = new Vector2(1f, 1f);
					array7[3] = new Vector2(1f, 0f);
					if (this.mInvert)
					{
						if (l > 0)
						{
							this.Rotate(array6, l);
							this.Rotate(array7, l);
						}
					}
					else if (l < 3)
					{
						this.Rotate(array6, 3 - l);
						this.Rotate(array7, 3 - l);
					}
					for (int m = 0; m < 4; m++)
					{
						int num11 = (!this.mInvert) ? (l * 4) : ((3 - l) * 4);
						float a = array5[num11];
						float b = array5[num11 + 1];
						float a2 = array5[num11 + 2];
						float b2 = array5[num11 + 3];
						array6[m].x = Mathf.Lerp(a, b, array6[m].x);
						array6[m].y = Mathf.Lerp(a2, b2, array6[m].y);
						array7[m].x = Mathf.Lerp(a, b, array7[m].x);
						array7[m].y = Mathf.Lerp(a2, b2, array7[m].y);
					}
					float fill2 = this.mFillAmount * 4f - (float)l;
					bool flag2 = l % 2 == 1;
					if (this.AdjustRadial(array6, array7, fill2, !flag2))
					{
						if (this.mInvert)
						{
							flag2 = !flag2;
						}
						if (flag2)
						{
							for (int n = 0; n < 4; n++)
							{
								float num12 = Mathf.Lerp(array[0].x, array[2].x, array6[n].x);
								float num13 = Mathf.Lerp(array[0].y, array[2].y, array6[n].y);
								float x4 = Mathf.Lerp(array2[0].x, array2[2].x, array7[n].x);
								float y4 = Mathf.Lerp(array2[0].y, array2[2].y, array7[n].y);
								meshData.verts.Add(new Vector3(num12 * base.Size.x, num13 * base.Size.y, 0f));
								meshData.uvs.Add(new Vector2(x4, y4));
								meshData.colors.Add(base.Color);
							}
						}
						else
						{
							for (int num14 = 3; num14 > -1; num14--)
							{
								float num15 = Mathf.Lerp(array[0].x, array[2].x, array6[num14].x);
								float num16 = Mathf.Lerp(array[0].y, array[2].y, array6[num14].y);
								float x5 = Mathf.Lerp(array2[0].x, array2[2].x, array7[num14].x);
								float y5 = Mathf.Lerp(array2[0].y, array2[2].y, array7[num14].y);
								meshData.verts.Add(new Vector3(num15 * base.Size.x, num16 * base.Size.y, 0f));
								meshData.uvs.Add(new Vector2(x5, y5));
								meshData.colors.Add(base.Color);
							}
						}
					}
				}
				return;
			}
		}
		for (int num17 = 0; num17 < 4; num17++)
		{
			meshData.verts.Add(Vector2.Scale(array[num17], base.Size));
			meshData.uvs.Add(array2[num17]);
			meshData.colors.Add(base.Color);
		}
	}

	[HideInInspector]
	[SerializeField]
	private UIFilledSprite.FillDirection mFillDirection = UIFilledSprite.FillDirection.Radial360;

	[HideInInspector]
	[SerializeField]
	private float mFillAmount = 1f;

	[HideInInspector]
	[SerializeField]
	private bool mInvert;

	public enum FillDirection
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360
	}
}
