using System;
using UnityEngine;

public class FFT
{
	public FFT(int specSize)
	{
		this.spectrumSize = specSize;
		this.ibuffer = new float[this.spectrumSize];
		this.ispec1 = new float[this.spectrumSize / 2];
		this.window = new float[this.spectrumSize];
		this.cspec = new ComplexNumber[this.spectrumSize];
		for (int i = 0; i < this.spectrumSize; i++)
		{
			this.window[i] = 0.54f - 0.46f * Mathf.Cos((float)i * (3.14159274f / (float)this.spectrumSize));
		}
	}

	private static void UnitySwap(ref ComplexNumber a, ref ComplexNumber b)
	{
		ComplexNumber complexNumber = a;
		a = b;
		b = complexNumber;
	}

	private static void Process(ComplexNumber[] data, int numsamples, bool forward)
	{
		int num = 0;
		for (int i = 0; i < numsamples - 1; i++)
		{
			if (i < num)
			{
				ComplexNumber complexNumber = data[i];
				data[i] = data[num];
				data[num] = complexNumber;
			}
			int num2 = numsamples >> 1;
			num ^= num2;
			while ((num & num2) == 0)
			{
				num2 >>= 1;
				num ^= num2;
			}
		}
		float num3 = (!forward) ? 3.14159274f : -3.14159274f;
		for (int j = 1; j < numsamples; j <<= 1)
		{
			float f = num3 / (float)j;
			ComplexNumber b = default(ComplexNumber);
			b.Set(Mathf.Cos(f), Mathf.Sin(f));
			ComplexNumber a = default(ComplexNumber);
			a.Set(1f, 0f);
			for (int k = 0; k < j; k++)
			{
				for (int l = k; l < numsamples; l += j << 1)
				{
					ComplexNumber b2 = default(ComplexNumber);
					ComplexNumber.Mul(a, data[l + j], ref b2);
					ComplexNumber.Sub(data[l], b2, ref data[l + j]);
					ComplexNumber.Add(data[l], b2, ref data[l]);
				}
				ComplexNumber.Mul(a, b, ref a);
			}
		}
	}

	public void Forward(ComplexNumber[] data, int numsamples)
	{
		FFT.Process(data, numsamples, true);
	}

	public void AnalyzeInput(float[] data, int numchannels, int numsamples, float decaySpeed)
	{
		for (int i = 0; i < this.spectrumSize; i++)
		{
			this.ibuffer[i] = data[i];
		}
		for (int j = 0; j < this.spectrumSize; j++)
		{
			this.cspec[j].Set(this.ibuffer[j] * this.window[j], 0f);
		}
		this.Forward(this.cspec, this.spectrumSize);
		for (int k = 0; k < this.spectrumSize / 2; k++)
		{
			this.ispec1[k] = this.cspec[k].Magnitude();
		}
	}

	public void ReadBuffer(float[] buffer, int numsamples)
	{
		if (numsamples > this.spectrumSize)
		{
			numsamples = this.spectrumSize;
		}
		float num = (float)(this.spectrumSize / 2 - 2) / (float)(numsamples - 1);
		for (int i = 0; i < numsamples; i++)
		{
			float num2 = (float)i * num;
			int num3 = Mathf.FloorToInt(num2);
			buffer[i] = this.ispec1[num3] + (this.ispec1[num3 + 1] - this.ispec1[num3]) * (num2 - (float)num3);
		}
	}

	private const float kMaxSampleRate = 22050f;

	private const float kPI = 3.14159274f;

	private readonly int spectrumSize;

	private float[] ibuffer;

	private float[] ispec1;

	private float[] window;

	private ComplexNumber[] cspec;
}
