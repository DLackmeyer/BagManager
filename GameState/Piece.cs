using System;
using System.Collections.Generic;
using System.Linq;

namespace BagManager
{
	public interface IPiece
	{
		string Name { get; }
		string DrawName { get; }
	}

	public partial class GameState
	{
		private class Piece : IPiece
		{
			public string Name { get; }
			public string DrawName { get; }
			public string GameGroup { get; }
			public string BagGroup { get; }
			public string Type { get; }

			public Piece(
				string name,
				string drawName,
				string gameGroup,
				string bagGroup,
				string type)
			{
				Name = name;
				DrawName = drawName;
				GameGroup = gameGroup;
				BagGroup = bagGroup;
				Type = type;
			}
		}
	}
}