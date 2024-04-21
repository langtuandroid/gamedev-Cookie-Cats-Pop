using System;
using System.Collections.Generic;
using Spine;
using UnityEngine;

public class TestSpineAnimations : MonoBehaviour
{
	private void Initialize()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.isInitialized = true;
		List<TestSpineAnimations.AnimationButton> list = new List<TestSpineAnimations.AnimationButton>();
		ExposedList<Spine.Animation> animations = this.spine.state.Data.skeletonData.animations;
		for (int i = 0; i < animations.Count; i++)
		{
			list.Add(new TestSpineAnimations.AnimationButton
			{
				name = animations.Items[i].name
			});
		}
		this.animationButtons = list.ToArray();
	}

	private void Update()
	{
		this.Initialize();
		for (int i = 0; i < this.animationButtons.Length; i++)
		{
			if (this.animationButtons[i].shouldPlay)
			{
				this.animationButtons[i].shouldPlay = false;
				this.spine.PlayAnimation(0, this.animationButtons[i].name, true, this.forceUpdate);
			}
		}
		if (this.setToSetupPose)
		{
			this.spine.SetToSetupPose();
			this.spine.Update(0f);
		}
	}

	private void LateUpdate()
	{
		if (this.setToSetupPose)
		{
			this.spine.SetToSetupPose();
			this.spine.Update(0f);
		}
	}

	public SkeletonAnimation spine;

	public bool forceUpdate;

	public TestSpineAnimations.AnimationButton[] animationButtons;

	public bool setToSetupPose;

	private bool isInitialized;

	[Serializable]
	public class AnimationButton
	{
		public bool shouldPlay;

		public string name;
	}
}
