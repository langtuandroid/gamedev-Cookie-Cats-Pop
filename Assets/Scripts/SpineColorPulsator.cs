using System;
using Spine;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpineColorizer))]
public class SpineColorPulsator : MonoBehaviour
{
	public Color ColorA
	{
		get
		{
			return this.colorA;
		}
		set
		{
			this.colorA = value;
		}
	}

	public Color ColorB
	{
		get
		{
			return this.colorB;
		}
		set
		{
			this.colorB = value;
		}
	}

	private void OnEnable()
	{
		this.spineColorizer = base.GetComponent<SpineColorizer>();
		this.startColor = this.spineColorizer.Color;
		this.colorA = this.startColor;
		this.elapsed = 0f;
	}

	private void OnDisable()
	{
		this.spineColorizer.SetSlotsColor(Color.white, null);
		this.spineColorizer.SetColor(this.startColor);
	}

	public void SetSlotFilter(Predicate<Slot> slotFilter)
	{
		this.slotFilter = slotFilter;
	}

	private void Update()
	{
		float num = Mathf.Sin((this.elapsed * this.speed + this.phase) * 3.14159274f * 2f) * 0.5f + 0.5f;
		this.elapsed += Time.deltaTime;
		num = Mathf.Pow(num, this.bias);
		this.spineColorizer.SetSlotsColor(Color.Lerp(this.colorA, this.colorB, num), this.slotFilter);
	}

	[SerializeField]
	private float speed = 1f;

	[SerializeField]
	private float bias = 1f;

	[SerializeField]
	private float phase;

	private float elapsed;

	[SerializeField]
	private Color colorA = Color.white;

	[SerializeField]
	private Color colorB = new Color(1f, 1f, 1f, 0f);

	private SpineColorizer spineColorizer;

	private Color startColor;

	private Predicate<Slot> slotFilter;
}
