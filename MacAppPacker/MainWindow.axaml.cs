using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MacAppPacker
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
        private string? _appPath;
        private List<string>? _fileList;
        private string? _infoplist;
        #endregion

        #region method
        private void BuildInfoPlist(string appName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
            sb.AppendLine("<plist version=\"1.0\">");
            sb.AppendLine("<dict>");
            sb.AppendLine("    <key>CFBundleDisplayName</key>");
            sb.AppendLine($"    <string>{appName}</string>");
            sb.AppendLine("    <key>CFBundleExecutable</key>");
            sb.AppendLine($"    <string>{appName}</string>");
            sb.AppendLine("    <key>CFBundleIdentifier</key>");
            sb.AppendLine($"    <string>com.colddaddy.{appName}</string>");
            sb.AppendLine("    <key>CFBundleInfoDictionaryVersion</key>");
            sb.AppendLine("    <string>6.0</string>");
            sb.AppendLine("    <key>CFBundlePackageType</key>");
            sb.AppendLine("    <string>APPL</string>");
            sb.AppendLine("    <key>CFBundleShortVersionString</key>");
            sb.AppendLine("    <string>1.0</string>");
            sb.AppendLine("    <key>CFBundleVersion</key>");
            sb.AppendLine("    <string>1.0</string>");
            sb.AppendLine("    <key>LSMinimumSystemVersion</key>");
            sb.AppendLine("    <string>10.12</string>");
            sb.AppendLine("</dict>");
            sb.AppendLine("</plist>");
            _infoplist = sb.ToString();
        }
        #endregion

        private async void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var folders = await iHawkAvaloniaCommonLibrary.CommonHelper.OpenFolderAsync(this);
            if (!(folders?.Count > 0)) return;
            _appPath = folders[0].Path.LocalPath;
            SelectedFolder.Text = _appPath;
            _fileList = Directory.GetFiles(folders[0].Path.LocalPath).ToList();
            FileList.ItemsSource = _fileList.Select(Path.GetFileName);
        }

        private void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (AppName.Text is null)
            {
                InfoList.Text = string.Empty;
                return;
            }
            BuildInfoPlist(AppName.Text.Trim());
            InfoList.Text = _infoplist;
        }

        private void Pack_Click(object sender, RoutedEventArgs e)
        {
            if (_appPath is null || _fileList is null || _infoplist is null || AppName.Text is null) return;
            var appName = AppName.Text.Trim();
            var path4Contents = Path.Combine(_appPath, $"{appName}.app", "Contents");
            var path4MacOS = Path.Combine(path4Contents, "MacOS");
            var path4Resources = Path.Combine(path4Contents, "Resources");
            Directory.CreateDirectory(path4MacOS);
            Directory.CreateDirectory(path4Resources);
            foreach (var file in _fileList)
            {
                File.Move(file, Path.Combine(path4MacOS, Path.GetFileName(file)));
            }
            File.WriteAllText(Path.Combine(path4Contents, "Info.plist"), _infoplist, Encoding.UTF8);
        }
    }
}