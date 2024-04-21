using System;
using System.Collections;
using System.IO;
using System.Text;

namespace TactileModules.Analytics.EventStorage
{
	public class EventsPool
	{
		public EventsPool(string filePath)
		{
			this.filePath = filePath;
			this.Filename = Path.GetFileName(filePath);
			if (!File.Exists(this.filePath))
			{
				FileStream fileStream = File.Create(this.filePath);
				fileStream.Close();
			}
		}

		public string Filename { get; private set; }

		public string Filepath
		{
			get
			{
				return this.filePath;
			}
		}

		public long Size
		{
			get
			{
				return new FileInfo(this.filePath).Length;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				if (this.creationTime == DateTime.MinValue)
				{
					this.creationTime = this.GetFileTimeFromName();
				}
				return this.creationTime;
			}
		}

		public bool IsLocked { get; set; }

		private DateTime GetFileTimeFromName()
		{
			string s = this.Filename.Substring(0, this.Filename.LastIndexOf(".", StringComparison.Ordinal));
			long fileTime = 0L;
			long.TryParse(s, out fileTime);
			return DateTime.FromFileTimeUtc(fileTime);
		}

		public void Add(string eventData)
		{
			using (StreamWriter streamWriter = File.AppendText(this.filePath))
			{
				streamWriter.Write(eventData);
				streamWriter.Write(",");
			}
		}

		public string ReadText()
		{
			return File.ReadAllText(this.filePath);
		}

		public IEnumerator ReadTextAsync(EventsReadResult result)
		{
			MemoryStream readStream = new MemoryStream();
			byte[] buffer = new byte[1024];
			FileStream fileStream = File.OpenRead(this.filePath);
			for (;;)
			{
				int numBytesRead = fileStream.Read(buffer, 0, buffer.Length);
				if (numBytesRead == 0)
				{
					break;
				}
				readStream.Write(buffer, 0, numBytesRead);
				yield return null;
			}
			result.Data = Encoding.UTF8.GetString(readStream.ToArray());
			fileStream.Close();
			yield break;
		}

		public void Append(string eventsData)
		{
			File.AppendAllText(this.filePath, eventsData);
		}

		public void Delete()
		{
			File.Delete(this.filePath);
		}

		public bool IsEmpty()
		{
			return new FileInfo(this.Filepath).Length == 0L;
		}

		public const int MAX_SIZE = 16000;

		private string filePath;

		private DateTime creationTime;
	}
}
