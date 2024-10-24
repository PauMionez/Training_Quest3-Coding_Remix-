using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Training_Quest3.Abstract
{
    internal class ViewModelBase : RaisePropertyChanged
    {
        #region Version
        /// <summary>
        /// Set the application name
        /// </summary>
        public string ApplicationName { get { return "Coding Program Remix"; } }

        /// <summary>
        /// Set the application version here.
        /// </summary>
        public string Title
        {
            get
            {
                return $"{ApplicationName} v1";
            }
        }

        #endregion

        #region Dialog Functions
        /// <summary>
        /// Display an error message
        /// </summary>
        /// <param name="ex">Your exception</param>
        /// <param name="message">Title text. Typically, put here your method name</param>
        public static void ErrorMessage(Exception ex)
        {
            try
            {
                StackTrace stackTrace = new StackTrace(ex);
                System.Reflection.MethodBase method = stackTrace.GetFrame(stackTrace.FrameCount - 1).GetMethod();
                string titleText = method.Name;

                MessageBox.Show(string.Format(ex.Message + "\n\n" + ex.StackTrace + "\n\n{0}", "Please screenshot and send procedures on how this error occured. Thank you."), titleText, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex2)
            {
                ErrorMessage(ex2);
            }
        }

        /// <summary>
        /// Displays an information messagebox
        /// </summary>
        /// <param name="message">Text</param>
        /// <param name="title">Title text</param>
        public static void InformationMessage(string message, string title)
        {
            try
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex2)
            {
                ErrorMessage(ex2);
            }
        }

        /// <summary>
        /// Displays a warning messagebox
        /// </summary>
        /// <param name="message">Text</param>
        /// <param name="title">Title text</param>
        public static void WarningMessage(string message, string title)
        {
            try
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex2)
            {
                ErrorMessage(ex2);
            }
        }
        /// <summary>
        /// Displays a warning messagebox
        /// </summary>
        /// <param name="message">Text</param>
        /// <param name="title">Title text</param>
        public static void WarningMessage(string message)
        {
            try
            {
                WarningMessage(message, "Warning");
            }
            catch (Exception ex2)
            {
                ErrorMessage(ex2);
            }
        }

        /// <summary>
        /// Displays a yes/no/cancel messagebox. Returns a MessageBoxResult. Either true or false.
        /// </summary>
        /// <param name="message">Text to ask</param>
        /// <returns>MessageBoxResult result. Either true or false.</returns>
        public static MessageBoxResult YesNoCancelDialog(string message)
        {
            MessageBoxResult res = MessageBoxResult.None;

            try
            {
                res = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
            }
            catch (Exception ex2)
            {
                ErrorMessage(ex2);
            }

            return res;
        }
        #endregion

        #region Shared
        /// <summary>
        /// Open the file chooser and return the selected files
        /// </summary>
        /// <returns></returns>
        public List<string> GetFilePaths(string DisplayName, string ExtensionList, string Title)
        {
            List<string> pathList = null;

            try
            {
                Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
                {
                    Title = Title,
                    Multiselect = true
                };

                dialog.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter(DisplayName, ExtensionList));

                if (dialog.ShowDialog() != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                {
                    return null;
                }

                pathList = dialog.FileNames.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }

            return pathList;
        }
        /// <summary>
        /// Open the file chooser and return the selected file or folder
        /// </summary>
        /// <returns></returns>
        public string GetFilePath(string DisplayName, string ExtensionList, string Title)
        {
            string path = null;

            try
            {
                Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
                {
                    Title = Title,
                };

                dialog.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter(DisplayName, ExtensionList));

                if (dialog.ShowDialog() != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                {
                    return null;
                }

                path = dialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }

            return path;
        }

        /// <summary>
        /// Open the folder chooser and return the selected paths
        /// </summary>
        /// <param name="IsMultiSelect">Default is false. True = Select 2 or more files</param>
        /// <returns></returns>
        public IEnumerable<string> GetFolderPaths(string Title)
        {
            IEnumerable<string> path = null;

            try
            {
                CommonOpenFileDialog folderSelectorDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    AllowNonFileSystemItems = false,
                    Multiselect = true,
                    Title = Title
                };

                if (folderSelectorDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = folderSelectorDialog.FileNames;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }

            return path;
        }

        /// <summary>
        /// Open the folder chooser and return the path
        /// </summary>
        /// <param name="IsMultiSelect">Default is false. True = Select 2 or more files</param>
        /// <returns></returns>
        public string GetFolderPath(string Title, bool isMultiSelect = false)
        {
            string path = null;

            try
            {
                CommonOpenFileDialog folderSelectorDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    AllowNonFileSystemItems = false,
                    Multiselect = isMultiSelect,
                    Title = Title
                };

                if (folderSelectorDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = folderSelectorDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }

            return path;
        }




        public static MessageBoxResult YesNoDialog(string message)
        {
            MessageBoxResult res = MessageBoxResult.None;

            try
            {
                res = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }

            return res;
        }

        #endregion
    }

    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
