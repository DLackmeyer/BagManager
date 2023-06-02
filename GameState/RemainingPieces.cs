using System;
using System.Collections.Generic;
using System.Linq;

namespace BagManager
{
	public interface IRemainingPieces
	{
		IReadOnlyDictionary<string, uint> this[string group] { get; }
	}

	public partial class GameState
	{
		private class RemainingPieces : IRemainingPieces
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
	}
}