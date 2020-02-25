using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLibrary.Message
{
	public sealed class ConfiguratorActorMessage
	{
		public sealed class UpdateRecipe
		{
			public UpdateRecipe(string recipe)
			{
				Recipe = recipe;
			}

			public string Recipe
			{
				get; private set;
			}
		}
	}
}
