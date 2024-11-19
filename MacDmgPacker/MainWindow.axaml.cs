using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MacDmgPacker
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region property
        private string? _srcFolder;
        private string? _dmgFile;
        private string? _volName;
        private List<string>? _fileList;
        #endregion

        #region method
        private void CreateDmgFile(string volName, string sourceDir, string dmgPath)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "hdiutil",
                    Arguments = $"create -volname {volName} -srcfolder \"{sourceDir}\" -ov -format UDZO \"{dmgPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
        #endregion

        #region event handler
        private async void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var folders = await iHawkAvaloniaCommonLibrary.CommonHelper.OpenFolderAsync(this);
            if (!(folders?.Count > 0)) return;
            _srcFolder = folders[0].Path.LocalPath;
            SelectedFolder.Text = _srcFolder;
            _fileList = Directory.GetFiles(folders[0].Path.LocalPath).ToList();
            FileList.ItemsSource = _fileList;
        }

        private async void DmgFile_Click(object? sender, RoutedEventArgs e)
        {
            var file = await iHawkAvaloniaCommonLibrary.CommonHelper.SaveFileAsync(this, "dmg");
            if (file == null) return;
            _dmgFile = file.Path.LocalPath;
            DmgFile.Text = _dmgFile;
            _volName = Path.GetFileNameWithoutExtension(_dmgFile);
            VolName.Text = _volName;
        }

        private void PackDmg_Click(object sender, RoutedEventArgs e)
        {
            if (!(_fileList?.Count > 0) || string.IsNullOrWhiteSpace(_volName) || string.IsNullOrWhiteSpace(_dmgFile)) return;

            var tmpDir = Path.Combine(Path.GetTempPath(), _volName);
            if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true);
            Directory.CreateDirectory(tmpDir);

            foreach (var file in _fileList)
            {
                File.Copy(file, Path.Combine(tmpDir, Path.GetFileName(file)));
            }

            CreateDmgFile(_volName, tmpDir, _dmgFile);
            Directory.Delete(tmpDir, true);
        }
        #endregion
    }
}