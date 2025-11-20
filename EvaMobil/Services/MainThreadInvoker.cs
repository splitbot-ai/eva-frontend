using EvaComponentLibrary.Services;

namespace EvaMobil.Services
{
	public class MainThreadInvoker : IThreadInvoker
	{
		public void InvokeOnMainThread(Action action)
		{
			MainThread.BeginInvokeOnMainThread(action);
		}
	}
}
