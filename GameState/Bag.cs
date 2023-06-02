using System;
using System.Collections.Generic;
using System.Linq;

namespace BagManager
{
	public partial class GameState
	{
		private class Bag
		{
			private Random _rand;
			private List<Piece> _pieces;

			public Bag(IReadOnlyList<Piece> pieces, Random rand)
			{
				_rand = rand;
				_pieces = new List<Piece>(pieces);
			}

			public Piece Draw()
			{
				if (_pieces.Count == 0)
				{
					return null;
				}

				int index = _rand.Next(0, _pieces.Count);
				Piece p = _pieces[index];
				_pieces.RemoveAt(index);
				return p;
			}

		}
	}
}