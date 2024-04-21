using System;

namespace Spine
{
	public class AttachmentTimeline : Timeline
	{
		public AttachmentTimeline(int frameCount)
		{
			this.frames = new float[frameCount];
			this.attachmentNames = new string[frameCount];
		}

		public int SlotIndex
		{
			get
			{
				return this.slotIndex;
			}
			set
			{
				this.slotIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return this.frames;
			}
			set
			{
				this.frames = value;
			}
		}

		public string[] AttachmentNames
		{
			get
			{
				return this.attachmentNames;
			}
			set
			{
				this.attachmentNames = value;
			}
		}

		public int FrameCount
		{
			get
			{
				return this.frames.Length;
			}
		}

		public void SetFrame(int frameIndex, float time, string attachmentName)
		{
			this.frames[frameIndex] = time;
			this.attachmentNames[frameIndex] = attachmentName;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
		{
			float[] array = this.frames;
			string text;
			if (time < array[0])
			{
				text = skeleton.data.slots[this.slotIndex].attachmentName;
			}
			else if (time >= array[array.Length - 1])
			{
				text = this.attachmentNames[array.Length - 1];
			}
			else
			{
				int num = Animation.binarySearch(array, time, 1) - 1;
				text = this.attachmentNames[num];
			}
			skeleton.slots.Items[this.slotIndex].Attachment = ((text != null) ? skeleton.GetAttachment(this.slotIndex, text) : null);
		}

		internal int slotIndex;

		internal float[] frames;

		internal string[] attachmentNames;
	}
}
