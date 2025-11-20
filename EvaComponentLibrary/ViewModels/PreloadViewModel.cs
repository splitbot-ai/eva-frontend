using EvaComponentLibrary.Models;
using EvaComponentLibrary.Services;

namespace EvaComponentLibrary.ViewModels
{
	public class PreloadViewModel
	{

		public readonly WebServices _ws;

		public PreloadViewModel(WebServices service)
		{
			_ws = service ?? throw new ArgumentNullException(nameof(service));
		}
		public  void Init()
		{
			_ws.Init();
		}
	}
}
