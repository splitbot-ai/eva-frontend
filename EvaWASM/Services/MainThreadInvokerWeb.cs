using EvaComponentLibrary.Services;

namespace EvaWASM.Services
{
	public class MainThreadInvokerWeb : IThreadInvoker
	{
		public void InvokeOnMainThread(Action action)
		{
			action();
		}
	}
}
