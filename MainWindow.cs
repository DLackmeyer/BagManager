using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BagManager
{
	public partial class MainWindow : Form
	{
		private GameState.Builder _builder = null;
		private GameSettings _gameSettings = null;
		private GameState _gameState = null;

		private IReadOnlyDictionary<Keys, Action> _buttonHotkeys = null;

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();

			if (File.Exists(Properties.Settings.Default.lastGameLocation))
			{
				_gameSettings = FileStorage.LoadGameSettingsFromFile(Properties.Settings.Default.lastGameLocation);
				LoadGameSettings(_gameSettings);

				_builder = new GameState.Builder(_gameSettings);
				StartNewGame();
			}
		}

		#endregion

		#region Profile Loading / Saving

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FileStorage.SaveGameSettings(_gameSettings, this);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GameSettings game = FileStorage.LoadGameSettings(this);
			if (game == null)
			{
				MessageBox.Show(this,
					"The given settings could not be loaded. The file was inaccessible or corrupt.",
					"Unable to load settings",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			LoadGameSettings(game);
		}

		private void LoadGameSettings(GameSettings game)
		{
			_gameSettings = game;
			UpdateDrawButtons();
		}

		#endregion

		#region Game Start

		private void StartNewGame()
		{
			_gameState = _builder.CreateGame(GetRandomPieces(_gameSettings.UserSelections.Values));
			this.historyTextBox.Text = $"Started new game: {_gameSettings.Name}";
			UpdateBagContents();
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show(this,
				"Are you sure you want to reset? Bag state and history will be lost.",
				"Reset Game?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if (result == DialogResult.Yes)
			{
				StartNewGame();
			}
		}

		private IReadOnlyList<string> GetRandomPieces(IEnumerable<UserSelection> userSelections)
		{
			Random r = new Random();
			var result = new List<string>();
			foreach (UserSelection selection in userSelections)
			{
				var selectionResult = new List<string>(selection.Options);
				int count = r.Next((int)selection.MinCount, (int)selection.MaxCount + 1);
				while (selectionResult.Count > count)
				{
					selectionResult.RemoveAt(r.Next(0, selectionResult.Count));
				}

				result.AddRange(selectionResult);
			}

			return result;
		}

		private void UpdateDrawButtons()
		{
			SuspendLayout();
			flowLayoutPanel1.Controls.Clear();

			int index = 0;
			var buttonHotKeys = new Dictionary<Keys, Action>();

			foreach (string bag in _gameSettings.Bags.Keys)
			{
				var button = new Button();
				button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
				button.Location = new System.Drawing.Point(3, 3);
				button.Name = $"draw{bag}Button";
				button.Size = new System.Drawing.Size(75, 23);
				button.TabIndex = 0;
				button.Text = bag;
				button.UseVisualStyleBackColor = true;
				button.Click += new System.EventHandler((e, args) => DrawTile(bag));
				flowLayoutPanel1.Controls.Add(button);

				buttonHotKeys.Add((Keys)(Keys.D1 + index++), () => DrawTile(bag));
			}

			_buttonHotkeys = buttonHotKeys;
			ResumeLayout(true);
		}

		#endregion

		#region Tile Drawing

		private void drawTileButton_Click(object sender, EventArgs e)
		{
			DrawTile(_gameState.BagList[0]);
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && _buttonHotkeys.TryGetValue(e.KeyCode, out Action callback))
			{
				callback();
				e.Handled = true;
			}
		}

		private void DrawTile(string bag)
		{
			IPiece drawn = _gameState.Draw(bag);

			if (drawn == null)
			{
				AddHistory($"{bag} bag is empty!");
			}
			else
			{
				AddHistory($"{bag} drawn: {drawn.DrawName}");
				UpdateBagContents();
			}
		}

		private void UpdateBagContents()
		{
			SuspendLayout();

			DataTable table = new DataTable();

			table.Columns.Add("Piece");
			foreach (string pt in _gameSettings.DisplayOrder)
			{
				table.Columns.Add(pt);
			}

			foreach (string group in _gameState.GameGroups)
			{
				IReadOnlyDictionary<string, uint> remaining = _gameState.BagContents[group];
				DataRow row = table.NewRow();
				row["Piece"] = _gameState.GetDisplayString(group);
				foreach (string pt in _gameSettings.DisplayOrder)
				{
					row[pt] = remaining.TryGetValue(pt, out uint left) ? left.ToString() : "--";
				}

				table.Rows.Add(row);
			}

			bagStatusGrid.DataSource = table;
			bagStatusGrid.Columns["Piece"].Width = 140;
			foreach (string pt in _gameSettings.DisplayOrder)
			{
				bagStatusGrid.Columns[pt].Width = 50;
			}

			ResumeLayout(true);
		}

		#endregion

		#region About Dialog

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this,
				"Dennis wrote this, but he didn't make an about dialog yet...",
				"Not Implemented",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		#endregion

		#region Dice Rolling

		private void rollButton_Click(object sender, EventArgs e)
		{
			AddHistory($"Rolled Die (d{_gameSettings.DefaultDiceSize}): {new Random().Next(0, (int)_gameSettings.DefaultDiceSize) + 1}");
		}

		private void AddHistory(string record)
		{
			this.historyTextBox.Text += $"\n{record}";
			this.historyTextBox.SelectionStart = this.historyTextBox.Text.Length;
			this.historyTextBox.ScrollToCaret();
		}

		#endregion
	}
}