using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
	public class Source
	{
		public string Name {  get; set; }
		public string Id { get; set; }
		public bool IsChecked { get; set; }=false;
		public string Category { get; set; }
		public FileTypes FileType { get; set; } = FileTypes.NONE;
	}
}
