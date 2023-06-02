using System.Collections.Generic;

namespace BagManager
{
	public class GameSettings
	{
		public GameSettings(
			string name,
			uint defaultDiceSize,
			IReadOnlyDictionary<string, IReadOnlyList<string>> bags,
			IReadOnlyList<string> requiredGroups,
			IReadOnlyList<string> displayOrder,
			IReadOnlyList<DisplayFormatEntry> displayFormat,
			IReadOnlyDictionary<string, UserSelection> userSelections,
			IReadOnlyList<PieceType> pieceTypes)
		{
			Name = name;
			DefaultDiceSize = defaultDiceSize;
			Bags = bags;
			RequiredGroups = requiredGroups;
			DisplayOrder = displayOrder;
			DisplayFormat = displayFormat;
			UserSelections = userSelections;
			PieceTypes = pieceTypes;
		}

		public string Name { get; }
		public uint DefaultDiceSize { get; }
		public IReadOnlyDictionary<string, IReadOnlyList<string>> Bags { get; }
		public IReadOnlyList<string> RequiredGroups { get; }
		public IReadOnlyList<string> DisplayOrder { get; }
		public IReadOnlyList<DisplayFormatEntry> DisplayFormat { get; }
		public IReadOnlyDictionary<string, UserSelection> UserSelections { get; }
		public IReadOnlyList<PieceType> PieceTypes { get; }
	}

	public class DisplayFormatEntry
	{
		public DisplayFormatEntry(string source, string format)
		{
			Source = source;
			Format = format ?? "{0}";
		}

		public string Source { get; }
		public string Format { get; }
	}

	public class UserSelection
	{
		public UserSelection(
			uint minCount,
			uint maxCount,
			IReadOnlyList<string> options)
		{
			MinCount = minCount;
			MaxCount = maxCount;
			Options = options;
		}

		public uint MinCount { get; }
		public uint MaxCount { get; }
		public IReadOnlyList<string> Options { get; }
	}

	public class PieceType
	{
		public PieceType(
			string gameGroup,
			IReadOnlyDictionary<string, BagGroup> bagGroups)
		{
			GameGroup = gameGroup;
			BagGroups = bagGroups;
		}

		public string GameGroup { get; }
		public IReadOnlyDictionary<string, BagGroup> BagGroups;
	}

	public class BagGroup : PieceDefinition
	{
		public BagGroup(
			string name,
			string drawName,
			string type,
			uint? count,
			IReadOnlyList<PieceDefinition> pieces)
			: base(name, drawName, type, count)
		{
			Pieces = pieces;
		}

		public IReadOnlyList<PieceDefinition> Pieces { get; }
	}

	public class PieceDefinition
	{
		public PieceDefinition(
			string name,
			string drawName,
			string type,
			uint? count)
		{
			Name = name;
			DrawName = drawName;
			Type = type;
			Count = count;
		}

		public string Name { get; }
		public string DrawName { get; }
		public string Type { get; }
		public uint? Count { get; }
	}
}