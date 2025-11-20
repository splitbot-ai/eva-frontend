namespace EvaComponentLibrary.Services
{
	public interface IThreadInvoker
	{
		void InvokeOnMainThread(Action action);
	}
}
