using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private List<string>? _fileList;//TODO:待删
        #endregion

        #region method
        private void Pack()
        {
            //if (_appPath is null || _fileList is null || _infoplist is null || AppName.Text is null) return;
            var appName = "AppName";//AppName.Text.Trim();//TODO:临时数据，需要替换为实际的应用名称
            var dmgName = $"{appName}.dmg";
            var tempDir = Path.Combine(Path.GetTempPath(), appName);
            var path4Contents = Path.Combine(tempDir, $"{appName}.app", "Contents");
            var path4MacOS = Path.Combine(path4Contents, "MacOS");
            var path4Resources = Path.Combine(path4Contents, "Resources");
            var path4Dmg = Path.Combine(tempDir, "DMG");

            Directory.CreateDirectory(path4MacOS);
            Directory.CreateDirectory(path4Resources);
            Directory.CreateDirectory(path4Dmg);

            foreach (var file in _fileList)
            {
                File.Move(file, Path.Combine(path4MacOS, Path.GetFileName(file)));
            }
            File.WriteAllText(Path.Combine(path4Contents, "Info.plist"), "_infoplist", Encoding.UTF8);

            // 复制背景图到DMG目录
            var backgroundImagePath = Path.Combine("Resources", "background.png");
            File.Copy(backgroundImagePath, Path.Combine(path4Dmg, "background.png"));

            // 创建应用程序快捷方式
            CreateApplicationsLink(path4Dmg);
        }

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

            // 配置DMG背景和快捷方式
            //ConfigureDmg(dmgPath, "AppName");//TODO:AppName为临时数据，需要替换为实际的应用名称
        }

        private void ConfigureDmg(string dmgPath, string appName)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "hdiutil",
                    Arguments = $"attach \"{dmgPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();

            var sb = new StringBuilder();
            sb.AppendLine("tell application \"Finder\"");
            sb.AppendLine($"\ttell disk \"{Path.GetFileNameWithoutExtension(dmgPath)}\"");
            sb.AppendLine("\t\topen");
            sb.AppendLine("\t\tset current view of container window to icon view");
            sb.AppendLine("\t\tset toolbar visible of container window to false");
            sb.AppendLine("\t\tset statusbar visible of container window to false");
            sb.AppendLine("\t\tset the bounds of container window to {100, 100, 600, 400}");
            sb.AppendLine("\t\tset viewOptions to the icon view options of container window");
            sb.AppendLine("\t\tset arrangement of viewOptions to not arranged");
            sb.AppendLine("\t\tset icon size of viewOptions to 72");
            sb.AppendLine("\t\tset background picture of viewOptions to file \".background:background.png\"");
            sb.AppendLine("\t\tmake new alias file at container window to POSIX file \"/Applications\" with properties {name:\"Applications\"}");
            sb.AppendLine($"\t\tset position of item \"{appName}.app\" of container window to {{100, 100}}");
            sb.AppendLine("\t\tset position of item \"Applications\" of container window to {400, 100}");
            sb.AppendLine("\t\tclose");
            sb.AppendLine("\t\topen");
            sb.AppendLine("\t\tupdate without registering applications");
            sb.AppendLine("\t\tdelay 5");
            sb.AppendLine("\tend tell");
            sb.AppendLine("end tell");
            var script = sb.ToString();
            var scriptPath = Path.Combine(Path.GetTempPath(), "configure_dmg.scpt");
            File.WriteAllText(scriptPath, script);

            process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "osascript",
                    Arguments = $"\"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();

            process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "hdiutil",
                    Arguments = $"detach \"/Volumes/{Path.GetFileNameWithoutExtension(dmgPath)}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Items.Add(CreateDirectoryNode(directory));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name });
            }
            return directoryNode;
        }

        /// <summary>
        /// 复制文件夹用于创建DMG
        /// </summary>
        private void CopyDirectory(string srcDir, string desDir)
        {
            var dir = new DirectoryInfo(srcDir);
            if (!Directory.Exists(desDir)) Directory.CreateDirectory(desDir);

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var targetFilePath = Path.Combine(desDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            var dirs = dir.GetDirectories();
            foreach (var subDir in dirs)
            {
                var newDesDir = Path.Combine(desDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDesDir);
            }
        }

        /// <summary>
        /// 创建应用程序快捷方式
        /// </summary>
        /// <param name="targetPath">快捷方式存放目录</param>
        private void CreateApplicationsLink(string targetPath)
        {
            var applicationsLink = Path.Combine(targetPath, "Applications");
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ln",
                    Arguments = $"-s /Applications \"{applicationsLink}\"",
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
            FileTree.ItemsSource = new[] { CreateDirectoryNode(new DirectoryInfo(_srcFolder)) };
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
            if (string.IsNullOrWhiteSpace(_srcFolder) || string.IsNullOrWhiteSpace(_volName) || string.IsNullOrWhiteSpace(_dmgFile)) return;

            var tmpDir = Path.Combine(Path.GetTempPath(), _volName);
            if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true);
            Directory.CreateDirectory(tmpDir);
            CopyDirectory(_srcFolder, tmpDir);
            CreateApplicationsLink(tmpDir);
            CreateDmgFile(_volName, tmpDir, _dmgFile);
            Directory.Delete(tmpDir, true);
        }
        #endregion
    }
}