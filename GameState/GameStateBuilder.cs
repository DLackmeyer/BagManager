using System;
using System.Collections.Generic;
using System.Linq;

namespace BagManager
{
	public partial class GameState
	{
		public class Builder
		{
			private IReadOnlyList<string> _requiredGroups { get; }
			private IReadOnlyList<DisplayFormatEntry> _displayFormat { get; }

			private IReadOnlyList<string> _bags;
			private Dictionary<string, string> _pieceTypeBagMap = new Dictionary<string, string>();
			private Dictionary<string, List<string>> _gameGroupPieceMap = new Dictionary<string, List<String>>();

			private Dictionary<string, Piece> _pieces = new Dictionary<string, Piece>();

			private Dictionary<string, uint> _pieceCounts = new Dictionary<string, uint>();
			private Dictionary<string, uint> _bagGroupCounts = new Dictionary<string, uint>();

			public Builder(GameSettings settings)
			{
				_displayFormat = settings.DisplayFormat;
				_requiredGroups = settings.RequiredGroups;
				_bags = settings.Bags.Keys.ToArray();

				foreach (KeyValuePair<string, IReadOnlyList<string>> bag in settings.Bags)
				{
					foreach (string pieceType in bag.Value)
					{
						_pieceTypeBagMap[pieceType] = bag.Key;
					}
				}

				foreach (PieceType pt in settings.PieceTypes)
				{
					foreach (KeyValuePair<string, BagGroup> bg in pt.BagGroups)
					{
						if (bg.Value.Pieces != null)
						{
							foreach (PieceDefinition pd in bg.Value.Pieces)
							{
								ProcessPiece(pd, pd.Type ?? bg.Value.Type, pt.GameGroup, bg.Key);
							}
						}
						else
						{
							ProcessPiece(bg.Value, bg.Value.Type, pt.GameGroup, bg.Key);
						}
					}
				}
			}

			private void ProcessPiece(PieceDefinition p, string pieceType, string gameGroup, string bagGroup)
			{
				string name = p.Name ?? bagGroup;
				_pieces.Add(name, new Piece(name, p.DrawName ?? name, gameGroup, bagGroup, pieceType));

				if (!_gameGroupPieceMap.TryGetValue(gameGroup, out List<string> gameGroupPieces))
				{
					gameGroupPieces = new List<string>();
					_gameGroupPieceMap[gameGroup] = gameGroupPieces;
				}
				gameGroupPieces.Add(name);

				_bagGroupCounts.TryGetValue(bagGroup, out uint currentCount);
				_bagGroupCounts[bagGroup] = currentCount + p.Count.Value;

				_pieceCounts[name] = p.Count.Value;
			}

			public GameState CreateGame(IReadOnlyList<string> userChoices, int? seed = null)
			{
				var bagContents = _bags.ToDictionary(k => k, v => new List<Piece>());
				var initialPieces = new Dictionary<string, Dictionary<string, uint>>();
				var displayStrings = new Dictionary<string, string>();

				IReadOnlyList<string> allGroups = userChoices.Union(_requiredGroups).ToArray();
				foreach (string g in allGroups)
				{
					var bagPieces = new List<Piece>();
					var pieceCountsByType = new Dictionary<string, uint>();
					var pieceBagGroupsByType = new Dictionary<string, string>();

					foreach (string pieceName in _gameGroupPieceMap[g])
					{
						Piece p = _pieces[pieceName];
						List<Piece> bag = bagContents[_pieceTypeBagMap[p.Type]];
						for (int i = 0; i < _pieceCounts[pieceName]; i++)
						{
							bag.Add(p);
						}

						pieceBagGroupsByType[p.Type] = p.BagGroup;
						pieceCountsByType.TryGetValue(p.Type, out uint currentCount);
						pieceCountsByType[p.Type] = currentCount + _pieceCounts[p.Name];
					}

					var segments = new List<string>();
					foreach (DisplayFormatEntry dfe in _displayFormat)
					{
						if (pieceBagGroupsByType.TryGetValue(dfe.Source, out string bagGroup))
						{
							segments.Add(String.Format(dfe.Format, bagGroup));
						}
					}
					displayStrings.Add(g, String.Join(" ", segments));

					initialPieces.Add(g, pieceCountsByType);
				}

				Random r = seed.HasValue ? new Random(seed.Value) : new Random();
				var bags = bagContents.ToDictionary(
					k => k.Key,
					k => new Bag(k.Value, r));

				return new GameState(displayStrings, allGroups, bags, new RemainingPieces(initialPieces));
			}
		}
	}
}