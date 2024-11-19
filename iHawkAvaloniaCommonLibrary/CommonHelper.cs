using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace iHawkAvaloniaCommonLibrary
{
    public static class CommonHelper
    {
        public static async Task<IReadOnlyList<IStorageFile>?> OpenPdfFileAsync(Visual visual)
        {
            if (TopLevel.GetTopLevel(visual) is not TopLevel topLevel) return null;
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("pdf文件") { Patterns = new List<string> { "*.pdf" } },
                    new("所有文件") { Patterns = new List<string> { "*.*" } }
                }
            });
            return files;
        }

        public static async Task<IReadOnlyList<IStorageFile>?> OpenFontFileAsync(Visual? visual)
        {
            if (TopLevel.GetTopLevel(visual) is not TopLevel topLevel) return null;
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("字库文件") { Patterns = new List<string> { "*.ttf", "*.otf" } },
                    new("ttf文件") { Patterns = new List<string> { "*.ttf" } },
                    new("otf文件") { Patterns = new List<string> { "*.otf" } },
                    new("所有文件") { Patterns = new List<string> { "*.*" } }
                }
            });
            return files;
        }

        public static async Task<IReadOnlyList<IStorageFolder>?> OpenFolderAsync(Visual? visual)
        {
            if (TopLevel.GetTopLevel(visual) is not TopLevel topLevel) return null;
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = true
            });
            return folders;
        }

        public static async Task<IStorageFile?> SaveFileAsync(Visual visual, string defaultExt)
        {
            if (TopLevel.GetTopLevel(visual) is not TopLevel topLevel) return null;
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                DefaultExtension = defaultExt,
                SuggestedFileName = $"未命名.{defaultExt}",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new FilePickerFileType($"{defaultExt}文件") { Patterns = new List<string> { $"*.{defaultExt}" } },
                    new FilePickerFileType("所有文件") { Patterns = new List<string> { "*.*" } }
                }
            });
            return file;
        }
    }
}
