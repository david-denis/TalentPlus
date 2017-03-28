namespace TalentPlus.Shared
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Xamarin;
	using Xamarin.Forms;
	using XLabs.Forms.Mvvm;
	using XLabs.Ioc;
	using XLabs.Platform.Device;
	using XLabs.Platform.Services.Media;

	/// <summary>
	/// Class CameraViewModel.
	/// </summary>
	[ViewType(typeof(VideoPage))]
	public class VideoViewModel : ViewModel
	{
		/// <summary>
		/// The _scheduler.
		/// </summary>
		private readonly TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		/// <summary>
		/// The picture chooser.
		/// </summary>
		private IMediaPicker _mediaPicker;

		/// <summary>
		/// The image source.
		/// </summary>
		private ImageSource _imageSource;

		/// <summary>
		/// The video info.
		/// </summary>
		private string _videoInfo;

		/// <summary>
		/// The take picture command.
		/// </summary>
		private Command _takeVideoCommand;

		/// <summary>
		/// The select video command.
		/// </summary>
		private Command _selectVideoCommand;

		private string _status;


		////private CancellationTokenSource cancelSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="CameraViewModel" /> class.
		/// </summary>
		public VideoViewModel()
		{
			Setup();
		}

		public byte[] ImageBytes { get; set; }

		/// <summary>
		/// Gets or sets the image source.
		/// </summary>
		/// <value>The image source.</value>
		public ImageSource ImageSource
		{
			get
			{
				return _imageSource;
			}
			set
			{
				SetProperty(ref _imageSource, value);
			}
		}

		/// <summary>
		/// Gets or sets the video info.
		/// </summary>
		/// <value>The video info.</value>
		public string VideoInfo
		{
			get
			{
				return _videoInfo;
			}
			set
			{
				SetProperty(ref _videoInfo, value);
			}
		}

		/// <summary>
		/// Gets the take picture command.
		/// </summary>
		/// <value>The take picture command.</value>
		public Command TakeVideoCommand
		{
			get
			{
				return _takeVideoCommand ?? (_takeVideoCommand = new Command(
																	   async () => await TakeVideo(),
																	   () => true));
			}
		}

		/// <summary>
		/// Gets the select video command.
		/// </summary>
		/// <value>The select picture command.</value>
		public Command SelectVideoCommand 
		{
			get
			{
				return _selectVideoCommand ?? (_selectVideoCommand = new Command(
																	   async () => await SelectVideo(),
																	   () => true)); 
			}
		}

		/// <summary>
		/// Gets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		public string Status
		{
			get { return _status; }
			private set { SetProperty(ref _status, value); }
		}

		/// <summary>
		/// Setups this instance.
		/// </summary>
		private void Setup()
		{
			var device = Resolver.Resolve<IDevice>();

			////RM: hack for working on windows phone? 
			//_mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
			#if __ANDROID__
			_mediaPicker = Resolver.Resolve<IMediaPicker>() ?? device.MediaPicker;
			#else
			_mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
			#endif
		}

		/// <summary>
		/// Takes the picture.
		/// </summary>
		/// <returns>Take Picture Task.</returns>
		private async Task<MediaFile> TakeVideo()
		{
			Setup();

			ImageSource = null;

			return await _mediaPicker.TakeVideoAsync(new VideoMediaStorageOptions { DefaultCamera = CameraDevice.Front, Quality = VideoQuality.Medium, DesiredLength = System.TimeSpan.FromSeconds(30) }).ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					Status = t.Exception.InnerException.ToString();
				}
				else if (t.IsCanceled)
				{
					Status = "Canceled";
				}
				else
				{
					var mediaFile = t.Result;
					
					byte[] bytes = new byte[mediaFile.Source.Length];
					mediaFile.Source.Read(bytes, 0, System.Convert.ToInt32(mediaFile.Source.Length));
					ImageBytes = bytes;

					ImageSource = ImageSource.FromStream(() => mediaFile.Source);

					return mediaFile;
				}

				return null;
			}, _scheduler);
		}

		/// <summary>
		/// Selects the video.
		/// </summary>
		/// <returns>Select Video Task.</returns>
		private async Task SelectVideo()
		{
			Setup();

			//TODO Localize
			VideoInfo = "Selecting video";
			ImageSource = null;

			try
			{
				var mediaFile = await _mediaPicker.SelectVideoAsync(new VideoMediaStorageOptions());

				byte[] bytes = new byte[mediaFile.Source.Length];
				mediaFile.Source.Read(bytes, 0, System.Convert.ToInt32(mediaFile.Source.Length));
				ImageBytes = bytes;

				//TODO Localize
				VideoInfo = mediaFile != null
					            ? string.Format("Your video size {0} MB", ConvertBytesToMegabytes(mediaFile.Source.Length))
					            : "No video was selected";

				ImageSource = ImageSource.FromStream(() => mediaFile.Source);
			}
			catch (System.Exception ex) 
			{
				if (ex is TaskCanceledException)
				{
					//TODO Localize
					VideoInfo = "Selecting video canceled";
				}
				else
				{
					VideoInfo = ex.Message;
					Insights.Report(ex, new Dictionary<string, string>
					{
						{ "Where", "VideoViewModel.SelectVideo()" }
					});
				}
			}
		}

		private static double ConvertBytesToMegabytes(long bytes)
		{
			return (bytes / 1024f) / 1024f;
		}
	}
}

