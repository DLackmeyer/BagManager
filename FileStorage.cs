using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BagManager
{
	public static class FileStorage
	{
		public static void SaveGameSettings(GameSettings gameSettings, Form parent)
		{
			SaveFileDialog saveDlg = new SaveFileDialog();
			saveDlg.Filter = "BagManager Games (*.bmg)|*.bmg";

			DialogResult result = saveDlg.ShowDialog(parent);
			if (result != DialogResult.OK)
			{
				return;
			}

			try
			{
				JsonSerializer serializer = new JsonSerializer();
				using (StreamWriter sw = new StreamWriter(saveDlg.FileName))
				using (JsonWriter jw = new JsonTextWriter(sw))
				{
					serializer.Serialize(jw, gameSettings);
				}

				Properties.Settings.Default.lastGameLocation = saveDlg.FileName;
				Properties.Settings.Default.Save();
			}
			catch
			{
				MessageBox.Show(parent,
					"Failed to write the profile data",
					"Save Failed",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		public static GameSettings LoadGameSettings(Form parent)
		{
			OpenFileDialog openDlg = new OpenFileDialog();
			openDlg.Filter = "BagManager Games (*.bmg)|*.bmg";

			DialogResult result = openDlg.ShowDialog(parent);
			if (result != DialogResult.OK)
			{
				return null;
			}

			return LoadGameSettingsFromFile(openDlg.FileName);
		}

		public static GameSettings LoadGameSettingsFromFile(string filePath)
		{
			try
			{
				JsonSerializer serializer = new JsonSerializer();
				using (StreamReader sr = new StreamReader(filePath))
				using (JsonReader jr = new JsonTextReader(sr))
				{
					return serializer.Deserialize<GameSettings>(jr);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error loading game: {e.Message}");
				return null;
			}
		}
	}
}