using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.Analytics.EventVerification.Packaging;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public class PackageUploader : IPackageUploader
	{
		public PackageUploader(IRequestFactory requestFactory)
		{
			this.requestFactory = requestFactory;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EventCountPackage> PackageUploaded;



		public bool IsUploading()
		{
			return !this.fiber.IsTerminated;
		}

		public void UpLoadPackages(List<EventCountPackage> packages)
		{
			this.packagesToUpload.AddRange(packages);
			if (this.IsUploading())
			{
				return;
			}
			this.fiber.Start(this.UpLoadPackagesInternal());
		}

		private IEnumerator UpLoadPackagesInternal()
		{
			EnumeratorResult<bool> hasErrors = new EnumeratorResult<bool>();
			while (this.packagesToUpload.Count > 0 && !hasErrors)
			{
				EventCountPackage package = this.packagesToUpload[0];
				this.packagesToUpload.RemoveAt(0);
				yield return this.UploadPackage(package, hasErrors);
			}
			yield break;
		}

		private IEnumerator UploadPackage(EventCountPackage package, EnumeratorResult<bool> hasErrors)
		{
			IUploadRequest request = this.GetRequest(package);
			yield return request.Run();
			string errorMessage = request.GetErrorMessage();
			if (!string.IsNullOrEmpty(errorMessage))
			{
				hasErrors.value = true;
				yield break;
			}
			IUploadResponse response = request.GetResponse();
			if (response.ResponseCode <= ReturnCode.RetryLater)
			{
				this.requestFactory.FailoverToNextCollector();
				hasErrors.value = true;
				yield break;
			}
			this.DispatchPackageUploaded(package);
			yield break;
		}

		private void DispatchPackageUploaded(EventCountPackage package)
		{
			this.PackageUploaded(package);
		}

		private IUploadRequest GetRequest(EventCountPackage package)
		{
			return this.requestFactory.CreateRequest(package);
		}

		private readonly Fiber fiber = new Fiber();

		private readonly List<EventCountPackage> packagesToUpload = new List<EventCountPackage>();

		private readonly IRequestFactory requestFactory;
	}
}
