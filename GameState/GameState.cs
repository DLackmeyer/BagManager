using System;
using System.Collections.Generic;
using System.Linq;

namespace BagManager
{
	public partial class GameState
	{
		private IReadOnlyDictionary<string, string> _displayStrings { get; }
		private IReadOnlyDictionary<string, Bag> _bags;
		private RemainingPieces _bagContents;

		public IReadOnlyList<string> GameGroups { get; }
		public IReadOnlyList<string> BagList { get; }
		public IRemainingPieces BagContents => _bagContents;

		private GameState(
			IReadOnlyDictionary<string, string> displayStrings,
			IReadOnlyList<string> gameGroups,
			IReadOnlyDictionary<string, Bag> bags,
			RemainingPieces remainingPieces)
		{
			_displayStrings = displayStrings;
			_bags = bags;
			GameGroups = gameGroups;
			_bagContents = remainingPieces;
			BagList = _bags.Keys.ToArray();
		}

		public IPiece Draw(string bag)
		{
			Piece p = _bags[bag].Draw();
			if (p == null)
			{
				return null;
			}

			_bagContents.RemovePiece(p);
			return p;
		}

		public string GetDisplayString(string gameGroup) => _displayStrings[gameGroup];
	}

	public class RemainingPieces
	{
		private IReadOnlyDictionary<string, Dictionary<string, uint>> _remainingPieces;

		public RemainingPieces(
			IReadOnlyDictionary<string, Dictionary<string, uint>> initialPieces)
		{
			_remainingPieces = initialPieces;
		}

		public void RemovePiece(Piece p)
		{
			_remainingPieces[p.GameGroup][p.Type]--;
		}

		public IReadOnlyDictionary<string, uint> this[string group] => _remainingPieces[group];
	}

	public class Piece
	{
		public string Name { get; }
		public string GameGroup { get; }
		public string BagGroup { get; }
		public string Type { get; }

		public Piece(
			string name,
			string gameGroup,
			string bagGroup,
			string type)
		{
			Name = name;
			GameGroup = gameGroup;
			BagGroup = bagGroup;
			Type = type;
		}
	}
}