using System;
using UnityEngine;

public struct ComplexNumber
{
	public void Set(float _re, float _im)
	{
		this.re = _re;
		this.im = _im;
	}

	public void Set(ComplexNumber c)
	{
		this.re = c.re;
		this.im = c.im;
	}

	public static void Mul(ComplexNumber a, float b, ref ComplexNumber result)
	{
		result.re = a.re * b;
		result.im = a.im * b;
	}

	public static void Mul(ComplexNumber a, ComplexNumber b, ref ComplexNumber result)
	{
		float num = a.re * b.im + a.im * b.re;
		result.re = a.re * b.re - a.im * b.im;
		result.im = num;
	}

	public static void Add(ComplexNumber a, ComplexNumber b, ref ComplexNumber result)
	{
		result.re = a.re + b.re;
		result.im = a.im + b.im;
	}

	public static void Sub(ComplexNumber a, ComplexNumber b, ref ComplexNumber result)
	{
		result.re = a.re - b.re;
		result.im = a.im - b.im;
	}

	public static ComplexNumber operator *(ComplexNumber self, float c)
	{
		return new ComplexNumber
		{
			re = self.re * c,
			im = self.im * c
		};
	}

	public static ComplexNumber operator *(ComplexNumber self, ComplexNumber c)
	{
		return new ComplexNumber
		{
			re = self.re * c.re - self.im * c.im,
			im = self.re * c.im + self.im * c.re
		};
	}

	public static ComplexNumber operator +(ComplexNumber self, ComplexNumber c)
	{
		return new ComplexNumber
		{
			re = self.re + c.re,
			im = self.im + c.im
		};
	}

	public static ComplexNumber operator -(ComplexNumber self, ComplexNumber c)
	{
		return new ComplexNumber
		{
			re = self.re - c.re,
			im = self.im - c.im
		};
	}

	public float Magnitude()
	{
		return Mathf.Sqrt(this.re * this.re + this.im * this.im);
	}

	public float Magnitude2()
	{
		return this.re * this.re + this.im * this.im;
	}

	public float re;

	public float im;
}
