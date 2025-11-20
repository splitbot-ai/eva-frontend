
namespace EvaComponentLibrary.Models
{
	public class ObservableMessage
	{
		public event Action<string>? OnChange;

		private string _property = "";
		public string Property
		{
			get => _property;
			set
			{
				if (_property != value)
				{
					_property = value;
					OnChange?.Invoke(_property); // Notify subscribers
				}
			}
		}
	}
}
