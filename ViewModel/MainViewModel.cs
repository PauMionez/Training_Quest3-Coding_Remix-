using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Training_Quest3.DataBase;
using Training_Quest3.Model;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Training_Quest3.ViewModel
{
    internal class MainViewModel : Abstract.ViewModelBase, INotifyDataErrorInfo
    {
        #region Coding Commands
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand LoadUserCommand { get; private set; }
        public AsyncCommand UpdateCommand { get; private set; }
        public AsyncCommand DeleteCommand { get; private set; }
        public AsyncCommand<GridCellDoubleTappedEventArgs> DoubleClickGridCommand { get; }
        public AsyncCommand ExportCommand { get; private set; }
        public DelegateCommand ImportSuggestCommand { get; private set; }
        #endregion

        #region Image Command
        public AsyncCommand SelectImageFolderCommand { get; private set; }
        public AsyncCommand AddImageCommand { get; private set; }
        public AsyncCommand DeleteImageCommand { get; private set; }
        public AsyncCommand ImageListSelectionChangedCommand { get; private set; }
        public DelegateCommand<MouseWheelEventArgs> ImageScaleMouseWheelCommand { get; private set; }


        #endregion


        public MainViewModel()
        {
            #region Coding Initial

            #region Coding Commands Initial
            LoadUserCommand = new AsyncCommand(GetUserAsync);
            SaveCommand = new AsyncCommand(AddNewRecordAsync);
            UpdateCommand = new AsyncCommand(UpdateUserAsync);
            DeleteCommand = new AsyncCommand(DeleteUserAsync);
            DoubleClickGridCommand = new AsyncCommand<GridCellDoubleTappedEventArgs>(DoubleClickGrid);
            ImportSuggestCommand = new DelegateCommand(ImportSuggest);
            ExportCommand = new AsyncCommand(ExportToExcelAsync);
            #endregion

            // Initialize Output Directories
            OutputDirectory = Directory.CreateDirectory(Path.Combine(DesktopDirectory, OutputFolderName));
            SaveDirectory = Directory.CreateDirectory(Path.Combine(OutputDirectory.FullName, ApplicationName));

            // Initialize Collection
            Suggestions = new ObservableCollection<SuggestionModel>();
            #endregion

            #region Image Initial

            #region Image Command Initial
            SelectImageFolderCommand = new AsyncCommand(SelectImageFolderAsync);
            ImageListSelectionChangedCommand = new AsyncCommand(ImageListSelectionChangedAsync);
            AddImageCommand = new AsyncCommand(AddImageAsync);
            DeleteImageCommand = new AsyncCommand(DeleteImageAsync);
            ImageScaleMouseWheelCommand = new DelegateCommand<MouseWheelEventArgs>(ImageScaleMouseWheelChanged);
            #endregion

            DefaultZoom = 0.5;
            ImageViewerScale = DefaultZoom;
            MultiCheckedVisibility = Visibility.Collapsed;
            CropExtensionCollection = new List<string> { "jpg", "jpeg", "png", "tif", "tiff" };
            SelectedCropExtension = CropExtensionCollection.First();
            ImageListCollection = new ObservableCollection<ImageListModel>();

            LoadPlaceholderImage();
            #endregion

        }

        #region Image Propertie and Fields
        #region ImageViewer Properties

        private ObservableCollection<ImageListModel> _ImageListCollection;

        public ObservableCollection<ImageListModel> ImageListCollection
        {
            get { return _ImageListCollection; }
            set { _ImageListCollection = value; OnPropertyChanged(); }
        }

        private ImageListModel _SelectedImageItem;

        public ImageListModel SelectedImageItem
        {
            get { return _SelectedImageItem; }
            set { _SelectedImageItem = value; OnPropertyChanged(); }
        }

        private string _SelectedImageFolder;
        public string SelectedImageFolder
        {
            get { return _SelectedImageFolder; }
            set { _SelectedImageFolder = value; OnPropertyChanged(); }
        }
        private int _SelectedImageIndex;

        public int SelectedImageIndex
        {
            get { return _SelectedImageIndex; }
            set { _SelectedImageIndex = value; OnPropertyChanged(); }
        }

        private string _SelectedImageInputText;

        public string SelectedImageInputText
        {
            get { return _SelectedImageInputText; }
            set { _SelectedImageInputText = value; OnPropertyChanged(); }
        }

        private BitmapImage _SelectedImageSource;

        public BitmapImage SelectedImageSource
        {
            get { return _SelectedImageSource; }
            set { _SelectedImageSource = value; OnPropertyChanged(); }
        }

        private double _DefaultZoom;

        public double DefaultZoom
        {
            get { return _DefaultZoom; }
            set { _DefaultZoom = value; OnPropertyChanged(); }
        }

        private double _ImageHeight;

        public double ImageHeight
        {
            get { return _ImageHeight; }
            set { _ImageHeight = value; OnPropertyChanged(); CanvasHeight = _ImageHeight; }
        }

        private double _ImageWidth;

        public double ImageWidth
        {
            get { return _ImageWidth; }
            set { _ImageWidth = value; OnPropertyChanged(); CanvasWidth = _ImageWidth; }
        }

        private double _CanvasHeight;

        public double CanvasHeight
        {
            get { return _CanvasHeight; }
            set { _CanvasHeight = value; OnPropertyChanged(); }
        }

        private double _CanvasWidth;

        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set { _CanvasWidth = value; OnPropertyChanged(); }
        }


        private List<string> _CropExtensionCollection;

        public List<string> CropExtensionCollection
        {
            get { return _CropExtensionCollection; }
            set { _CropExtensionCollection = value; OnPropertyChanged(); }
        }

        private string _SelectedCropExtension;

        public string SelectedCropExtension
        {
            get { return _SelectedCropExtension; }
            set { _SelectedCropExtension = value; OnPropertyChanged(); }
        }

        private string _CropThumbsColor;

        public string CropThumbsColor
        {
            get { return _CropThumbsColor; }
            set { _CropThumbsColor = value; OnPropertyChanged(); }
        }

        private double _ImageViewerScale;

        public double ImageViewerScale
        {
            get { return _ImageViewerScale; }
            set { _ImageViewerScale = value; OnPropertyChanged(); }
        }

        private Visibility _MultiCheckedVisibility;

        public Visibility MultiCheckedVisibility
        {
            get { return _MultiCheckedVisibility; }
            set { _MultiCheckedVisibility = value; OnPropertyChanged(); }
        }

        #endregion

        #region ImageViewer Fields

        Canvas CanvasControl;
        private double ImageDPI = 0;
        private const string Pdf_Color = "Red";
        private const string Image_Color = "LightBlue";


        #endregion
        #endregion


        #region Coding Properties and Fields

        private ObservableCollection<UserInfoModel> _CodingRecordsCollection;
        public ObservableCollection<UserInfoModel> CodingRecordsCollection
        {
            get { return _CodingRecordsCollection; }
            set { _CodingRecordsCollection = value; OnPropertyChanged(); }
        }

        private UserInfoModel _selectedUserItem;
        public UserInfoModel SelectedUserItem
        {
            get { return _selectedUserItem; }
            set { _selectedUserItem = value; OnPropertyChanged(); }
        }

        private string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set { _StatusText = value; OnPropertyChanged(); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _middleName;
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; OnPropertyChanged(); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _birth_day;
        public string Birth_day
        {
            get { return _birth_day; }
            set { _birth_day = value; OnPropertyChanged(); }
        }
        private string _birth_month;
        public string Birth_month
        {
            get { return _birth_month; }
            set { _birth_month = value; OnPropertyChanged(); }
        }
        private string _birth_year;
        public string Birth_year
        {
            get { return _birth_year; }
            set { _birth_year = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        private string _favAnimal;
        public string FavAnimal
        {
            get { return _favAnimal; }
            set { _favAnimal = value; OnPropertyChanged(); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        //private string _password;
        //public string Password
        //{
        //    get { return _password; }
        //    set
        //    {
        //        _password = value;

        //        ClearErrors(nameof(Password));
        //        if (_password.Length < 8)
        //        {
        //            AddError(nameof(Password), "Your Password is Weak!.");
        //        }

        //        OnPropertyChanged();
        //    }
        //}

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        #region Coding Fields 
        public ObservableCollection<SuggestionModel> Suggestions { get; set; }
        private DirectoryInfo SaveDirectory { get; set; }
        private string _selectedSuggestfile;
        public string JoinedPasswordError { get; private set; }
        public bool IsUpdating = false;

        // Directory Fields
        private readonly string DesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private readonly DirectoryInfo OutputDirectory;
        private readonly string OutputFolderName = "Training Progam Remix";
        private readonly string ExcelFileName = $"Coding_Program_Remix";
        #endregion
        #endregion


        #region Coding Functions
        /// <summary>
        /// Insert statement and Update statements
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private async Task AddNewRecordAsync()
        {
            try
            {
                #region validations

                var errorMessage = ValidateInputs();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    WarningMessage($"{errorMessage}", "Validation Errors");
                    return;
                }

                if (IsUpdating)
                {
                    WarningMessage($"You can Add new Record while updating", "Validation Errors");
                    ResetAddRecordInputs();
                    LoadPlaceholderImage();
                    IsUpdating = false;
                    return;
                }
                if (SelectedImageFolder == null)
                {
                    WarningMessage("Please Select Image Folder.", "Validation Error");
                    return;
                }

                if (SelectedImageItem == null)
                {
                    WarningMessage("Please Select Image first before add new record.", "Validation Error");
                    return;
                }
                #endregion

                // Collect data
                UserInfoModel user = new UserInfoModel
                {
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
                    BD_day = Birth_day,
                    BD_month = Birth_month,
                    BD_year = Birth_year,
                    Address = Address,
                    FavAnimal = FavAnimal,
                    UserName = UserName,
                    Password = Password,
                    ImageName = SelectedImageItem.FileName,

                };

                //Confirmation 
                if (YesNoDialog("Do you want to add this new record?") != MessageBoxResult.Yes) { return; }

                //Add the Favorite Animal if the input is not in suggestion list
                if (!Suggestions.Any(s => s.FavanimalModel.Equals(FavAnimal, StringComparison.OrdinalIgnoreCase)))
                {
                    Suggestions.Add(new SuggestionModel { FavanimalModel = FavAnimal });
                }

                using (DBContext db = new DBContext())
                {
                    db.UserInfoRecord.Add(user);
                    await db.SaveChangesAsync();
                }

                CodingRecordsCollection.Add(user);

                //SelectedImageListItem.BackgroundColor = "LightGreen";
                //if (SelectedImageListIndex < ImageCollection.Count) SelectedImageListIndex++;

                ResetAddRecordInputs();
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Populate Collection from the DB Data
        /// </summary>
        /// <returns></returns>
        private async Task GetUserAsync()
        {
            CodingRecordsCollection = new ObservableCollection<UserInfoModel>();

            await Task.Run(async () =>
            {
                try
                {
                    using (DBContext db = new DBContext())
                    {
                        List<UserInfoModel> userRecord = await db.UserInfoRecord.ToListAsync();

                        foreach (UserInfoModel item in userRecord)
                        {
                            CodingRecordsCollection.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage(ex);
                }
            });
        }

        /// <summary>
        /// Import Suggestion file(.txt or .xlxs)
        /// Get and Adding to the Suggestion Collection
        /// </summary>
        public void ImportSuggest()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Text files|*.txt;*.xlsx",
                    Title = "Select Suggestions File"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedSuggestfile = openFileDialog.FileName;

                    Suggestions.Clear();

                    if (File.Exists(_selectedSuggestfile))
                    {
                        //Get & add data from .txt file
                        if (_selectedSuggestfile.EndsWith(".txt"))
                        {
                            string[] lines = File.ReadAllLines(_selectedSuggestfile);

                            foreach (string line in lines)
                            {
                                Suggestions.Add(new SuggestionModel { FavanimalModel = line });
                            }
                        }
                        //Get & add data from .xlsx file to collection
                        else if (_selectedSuggestfile.EndsWith(".xlsx"))
                        {
                            using (ExcelEngine excelEngine = new ExcelEngine())
                            {
                                IApplication application = excelEngine.Excel;
                                IWorkbook wb = application.Workbooks.Open(_selectedSuggestfile);
                                IWorksheet ws = wb.Worksheets[0];

                                int rowCount = ws.Rows.Count();

                                //Assuming data is in the first row and Column / no header
                                for (int row = 1; row <= rowCount; row++)
                                {
                                    string suggestion = ws[row, 1].Value.ToString();
                                    if (!string.IsNullOrEmpty(suggestion))
                                    {
                                        Suggestions.Add(new SuggestionModel { FavanimalModel = suggestion });
                                    }
                                }
                            }
                        }


                        // Sort the collection
                        SortCollection(Suggestions, e => e.FavanimalModel);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Reset All
        /// </summary>
        private void ResetAddRecordInputs()
        {
            FirstName = MiddleName = LastName = Birth_day = Birth_month = Birth_year = Address = FavAnimal = UserName = Password = string.Empty;
            ClearErrors(nameof(Password));
        }

        /// <summary>
        /// Update The Data from Selected Retrieve Data 
        /// </summary>
        /// <returns></returns>
        private async Task UpdateUserAsync()
        {
            try
            {
                #region validations

                var errorMessage = ValidateInputs();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    WarningMessage($"{errorMessage}", "Validation Errors");
                    return;
                }

                if (SelectedUserItem == null)
                {
                    WarningMessage("No user selected for update.");
                    return;
                }
                #endregion


                using (DBContext db = new DBContext())
                {
                    // Find the existing user in the database
                    var user = await db.UserInfoRecord.FindAsync(SelectedUserItem.Id); // Assuming Id is the primary key

                    if (user != null)
                    {
                        // Update the properties
                        user.FirstName = FirstName;
                        user.MiddleName = MiddleName;
                        user.LastName = LastName;
                        user.BD_day = Birth_day;
                        user.BD_month = Birth_month;
                        user.BD_year = Birth_year;
                        user.Address = Address;
                        user.FavAnimal = FavAnimal;
                        user.UserName = UserName;
                        user.Password = Password;
                        user.ImageName = SelectedImageItem.FileName;

                        // Save changes
                        await db.SaveChangesAsync();
                        InformationMessage($"User {user.FullName} Updated successfully.", "Update Succesful");

                        // Update the collection from changes 
                        var index = CodingRecordsCollection.IndexOf(SelectedUserItem);
                        if (index >= 0)
                        {
                            CodingRecordsCollection[index] = user;
                        }

                        ResetAddRecordInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Delete Selected Data from DataGrid
        /// </summary>
        /// <returns></returns>
        private async Task DeleteUserAsync()
        {
            try
            {
                if (SelectedUserItem == null)
                {
                    WarningMessage("No user selected for deletion.");
                    return;
                }

                using (DBContext db = new DBContext())
                {
                    //Get the user in the database
                    var user = await db.UserInfoRecord.FindAsync(SelectedUserItem.Id);

                    if (user != null)
                    {
                        db.UserInfoRecord.Remove(user);
                        await db.SaveChangesAsync();
                        CodingRecordsCollection.Remove(SelectedUserItem);
                        InformationMessage($"User {user.FullName} Record Deleted successfully.", "Delete Succesful");
                    }
                    ResetAddRecordInputs();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Double Click Commands 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task DoubleClickGrid(GridCellDoubleTappedEventArgs args)
        {
            try
            {
                if (args.Record is UserInfoModel data)
                {
                    await LoadDataAgain(data);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Load the Selected row data into input fields
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<Task> LoadDataAgain(UserInfoModel data)
        {
            try
            {
                FirstName = data.FirstName;
                MiddleName = data.MiddleName;
                LastName = data.LastName;
                Birth_month = data.BD_month;
                Birth_day = data.BD_day;
                Birth_year = data.BD_year;
                Address = data.Address;
                FavAnimal = data.FavAnimal;
                UserName = data.UserName;
                Password = data.Password;


                //Find the corresponding image item in ImageListCollection
                SelectedImageItem = ImageListCollection.FirstOrDefault(item => item.FileName == data.ImageName);

                //Show the image frome selected retrieved data
                if (SelectedImageItem != null)
                {
                    await ImageListSelectionChangedAsync();
                }
                else
                {
                    WarningMessage($"Image From The Record Not Exist in Folder {SelectedImageFolder}.");
                    ResetAddRecordInputs();
                    LoadPlaceholderImage();
                }

                IsUpdating = true;
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Get data from database and export to .xlsx file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task ExportToExcelAsync()
        {
            try
            {
                // Validations
                if (!Directory.Exists(SaveDirectory.FullName))
                {
                    SaveDirectory = Directory.CreateDirectory(Path.Combine(OutputDirectory.FullName, ApplicationName));
                }

                string datetimeMark = DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss");

                #region Excel
                var outExcelFile = Path.Combine(SaveDirectory.FullName, $"Report {Path.GetFileName(ExcelFileName)} {datetimeMark}.xlsx");


                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    IWorkbook wb = application.Workbooks.Create();
                    IWorksheet ws = wb.Worksheets[0];

                    #region Column headers

                    ws.Range[1, 1].Text = "ID";
                    ws.Range[1, 2].Text = "First Name";
                    ws.Range[1, 3].Text = "Middle Name";
                    ws.Range[1, 4].Text = "Last Name";
                    ws.Range[1, 5].Text = "Birth Day";
                    ws.Range[1, 6].Text = "Birth Month";
                    ws.Range[1, 7].Text = "Birth Year";
                    ws.Range[1, 8].Text = "Address";
                    ws.Range[1, 9].Text = "Favorite Animal";
                    ws.Range[1, 10].Text = "User Name";
                    ws.Range[1, 11].Text = "Password";

                    int lastCol = ws.UsedRange.LastColumn;

                    //Header excel design
                    ws.Range[1, 1, 1, lastCol].CellStyle.Font.Bold = true;
                    ws.Range[1, 1, 1, lastCol].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    ws.Range[1, 1, 1, lastCol].CellStyle.Font.Color = ExcelKnownColors.White;
                    ws.Range[1, 1, 1, lastCol].CellStyle.Color = System.Drawing.Color.FromArgb(42, 118, 189);
                    #endregion

                    #region Data

                    TextInfo textInfo = new CultureInfo("en-US").TextInfo;

                    for (int row = 0; row < CodingRecordsCollection.Count; row++)
                    {
                        UserInfoModel record = CodingRecordsCollection[row];
                        ws[row + 2, 1].Number = record.Id;
                        ws[row + 2, 2].Text = textInfo.ToTitleCase(record.FirstName.ToLower());
                        ws[row + 2, 4].Text = textInfo.ToTitleCase(record.MiddleName.ToLower());
                        ws[row + 2, 3].Text = textInfo.ToTitleCase(record.LastName.ToLower());
                        ws[row + 2, 5].Text = record.BD_day;
                        ws[row + 2, 6].Text = record.BD_month;
                        ws[row + 2, 7].Text = record.BD_year;
                        ws[row + 2, 8].Text = textInfo.ToTitleCase(record.Address.ToLower());
                        ws[row + 2, 9].Text = record.FavAnimal;
                        ws[row + 2, 10].Text = record.UserName;
                        ws[row + 2, 11].Text = record.Password;

                        //center align the data
                        for (int column = 1; column <= 100; column++)
                        {
                            ws[1, column].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                            ws[row + 2, column].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        }
                    }
                    #endregion

                    ws.UsedRange.AutofitColumns();

                    await Task.Run(() =>
                    {
                        using (FileStream filestream = new FileStream(outExcelFile, FileMode.Create, FileAccess.ReadWrite))
                        {
                            wb.SaveAs(filestream);
                        }
                    });
                }

                #endregion

                Process.Start(outExcelFile);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }

        }

        #region Validation

        /// <summary>
        /// Gather all Errors Message
        /// </summary>
        /// <returns></returns>
        public string ValidateInputs()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstName)) errors.Add("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(MiddleName)) errors.Add("Middle name cannot be empty.");
            if (string.IsNullOrWhiteSpace(LastName)) errors.Add("Last name cannot be empty.");
            if (string.IsNullOrWhiteSpace(Address)) errors.Add("Address cannot be empty.");
            if (string.IsNullOrWhiteSpace(FavAnimal)) errors.Add("Favorite Animal cannot be empty.");
            if (string.IsNullOrWhiteSpace(UserName)) errors.Add("User name cannot be empty.");
            if (!IsValidateDay(Birth_day) || string.IsNullOrWhiteSpace(Birth_day)) errors.Add("Invalid Birth Day.");
            if (!IsValidateMonth(Birth_month) || string.IsNullOrWhiteSpace(Birth_month)) errors.Add("Invalid Birth Month.");
            if (!IsValidateYear(Birth_year) || string.IsNullOrWhiteSpace(Birth_year)) errors.Add("Invalid Birth Year.");
            if (string.IsNullOrWhiteSpace(Password)) errors.Add("Password cannot be empty");
            if (!IsValidatePassword(Password)) errors.Add($"Invalid Password: Password must contain at least {JoinedPasswordError}");

            return errors.Any() ? string.Join("\n", errors) : null;
        }

        #region Birth Date Validation
        /// <summary>
        /// Validation for Day, Month, Year Input
        /// </summary>
        /// <param name="day"></param>
        /// <param name="TitleName"></param>
        /// <returns></returns>
        private bool IsValidateDay(string day)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(day)) return true;
                if (day.Contains("?")) return true;

                if (int.TryParse(day, out int _day))
                {
                    if (_day < 1 || _day > 31)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }

            return true;
        }
        private bool IsValidateMonth(string month)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(month)) return true;
                if (month.Contains("?")) return true;

                if (int.TryParse(month, out int _month))
                {
                    if (_month < 1 || _month > 12)
                    {

                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }

            return true;
        }
        private bool IsValidateYear(string year)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(year)) return true;
                if (year.Contains("?")) return true;
                if (year.Length != 4) return false;
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Validation Password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool IsValidatePassword(string password)
        {
            try
            {
                List<string> PasswordError = new List<string>();

                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    PasswordError.Add("1 uppercase letter");
                }

                if (!Regex.IsMatch(password, @"\d"))
                {
                    PasswordError.Add("1 digit");
                }

                if (!Regex.IsMatch(password, @"[@#$!%*?&]"))
                {
                    PasswordError.Add("1 special character");
                }

                if (password.Length < 8)
                {
                    PasswordError.Add("8 characters long");
                }


                JoinedPasswordError = PasswordError.Count > 0 ? string.Join(", ", PasswordError) : null;

                if (JoinedPasswordError == null) { return true; }
                else { return false; }


            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }

            return true;
        }


        #endregion

        #region ErrorNorify

        private readonly Dictionary<string, List<string>> _PropertyErrors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public bool HasErrors => _PropertyErrors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            return _PropertyErrors.GetValueOrDefault(propertyName, null);
        }


        public void AddError(string propertyName, string errorMessage)
        {
            if (!_PropertyErrors.ContainsKey(propertyName))
            {
                _PropertyErrors.Add(propertyName, new List<string>());
            }

            _PropertyErrors[propertyName].Add(errorMessage);
            OnErrorChanged(propertyName);
        }

        public void ClearErrors(string propertyName)
        {
            if (_PropertyErrors.Remove(propertyName))
            {
                OnErrorChanged(propertyName);
            }
        }

        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion
        #endregion 

        #region Image Function/Method

        /// <summary>
        /// Add a selected image or pdf 
        /// </summary>
        /// <returns></returns>
        private async Task AddImageAsync()
        {
            try
            {
                if (ImageListCollection == null)
                {
                    WarningMessage("Please Select Image Folder first before Add other Image.", "Validation Error");
                    LoadPlaceholderImage();
                    return;
                }

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.jfif;*.pdf",
                    Title = "Select an Image"
                };


                //validate the action
                if (YesNoDialog($"Do you want to Add this Image {openFileDialog.FileName}?") != MessageBoxResult.Yes) { return; }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    // Add the selected image to the collection
                    ImageListModel newImageItem = new ImageListModel
                    {
                        FullPath = selectedFilePath,
                    };

                    //Set the color based on file type
                    newImageItem.Color = newImageItem.FullPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? Pdf_Color : Image_Color;

                    ImageListCollection.Add(newImageItem);

                    // Sort the collection
                    SortCollection(ImageListCollection, e => e.FullPath);

                    await Task.CompletedTask;


                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Delete the selected image
        /// </summary>
        /// <returns></returns>
        private async Task DeleteImageAsync()
        {
            try
            {
                if (SelectedImageItem == null)
                {
                    WarningMessage("Please Select Image first before delete.", "Validation Error");
                    LoadPlaceholderImage();
                    return;
                }

                string imagename = SelectedImageItem.FileName;

                //validate the action
                if (YesNoDialog($"Do you want to delete this image {imagename}?") != MessageBoxResult.Yes) { return; }

                await Task.Run(() =>
                {
                    ImageListCollection.Remove(SelectedImageItem);
                    SelectedImageItem = null;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Selecting Folder Image that contain image and pdf
        /// </summary>
        /// <returns></returns>
        private async Task SelectImageFolderAsync()
        {
            try
            {
                SelectedImageFolder = GetFolderPath("Select Input Image Folder");
                if (string.IsNullOrWhiteSpace(SelectedImageFolder)) { LoadPlaceholderImage(); return; }


                ImageListCollection.Clear();
                SelectedImageItem = null;
                SelectedImageSource = null;

                List<string> ExtensionCollection = new List<string> { "jpg", "jpeg", "png", "tif", "tiff", "jfif", "pdf" };
                SelectedCropExtension = ExtensionCollection.First();

                List<string> imageFileCollection = await Task.Run(() => ExtensionCollection.SelectMany(ext => Directory.GetFiles(SelectedImageFolder, $"*.{ext}")).ToList());

                if (imageFileCollection.Count != 0)
                {
                    ImageListCollection.Clear();
                    foreach (string file in imageFileCollection)
                    {
                        ImageListCollection.Add(new ImageListModel
                        {
                            FullPath = file
                        });
                    }


                    // Sort the collection
                    SortCollection(ImageListCollection, e => e.FullPath);

                    List<ImageListModel> pdfFiles = ImageListCollection.Where(file => file.FullPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)).ToList();
                    foreach (ImageListModel file in ImageListCollection)
                    {
                        file.Color = file.FullPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? Pdf_Color : Image_Color;
                    }
                }
                else
                {
                    WarningMessage($"No Images Found in This Folder {SelectedImageFolder}");
                    LoadPlaceholderImage();
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }




        /// <summary>
        /// Image View Change base on selected in list collection
        /// Load images and pdf in image View
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task ImageListSelectionChangedAsync()
        {
            try
            {
                // Avoid auto select first item on set itemsource
                if (ImageListCollection == null || SelectedImageItem == null) { LoadPlaceholderImage(); return; }

                // Set primary canvas control
                GetCanvas(CanvasControl);

                string filePath = SelectedImageItem.FullPath;

                //Handle PDF files
                if (Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadSelectedPDFFile(filePath);
                }
                else
                {
                    await LoadSelectedImageFile(filePath);
                }
            }
            catch (IOException)
            {
                // Do not show error
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Load Pdf file in image viewer using pdfiumviewer 
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        private async Task LoadSelectedPDFFile(string pdfFilePath)
        {
            if (string.IsNullOrWhiteSpace(pdfFilePath)) return;

            await Task.Run(() =>
            {
                using (FileStream fs = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var document = PdfiumViewer.PdfDocument.Load(fs))
                    {
                        if (document.PageCount > 0)
                        {
                            //Render the first page at 300 DPI
                            var image = document.Render(0, 300, 300, true);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Seek(0, SeekOrigin.Begin);

                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = ms;
                                bitmap.EndInit();
                                bitmap.Freeze();
                                SelectedImageSource = bitmap;

                                SetImageProperties(image);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Load Image file in image viewer
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <returns></returns>
        private async Task LoadSelectedImageFile(string imageFilePath)
        {
            if (string.IsNullOrWhiteSpace(imageFilePath)) return;

            byte[] imageData;
            int bufferSize = 4096;

            await Task.Run(async () =>
            {
                // Open the file with read access and shared read access for safe access
                using (FileStream fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize * 3, useAsync: true))
                {
                    imageData = new byte[fs.Length];
                    await fs.ReadAsync(imageData, 0, (int)fs.Length);
                }

                //Load the image from the byte array into a MemoryStream
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    SelectedImageSource = bitmap;
                }
                using (Bitmap image = new Bitmap(imageFilePath))
                {
                    SetImageProperties(image);
                }
            });
        }

        /// <summary>
        /// Propertie Image base on image properties
        /// </summary>
        /// <param name="image"></param>
        private void SetImageProperties(System.Drawing.Image image)
        {
            ImageHeight = image.Height;
            ImageWidth = image.Width;
            ImageDPI = image.HorizontalResolution;
        }

        /// <summary>
        /// Sorting Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="pathSelector"></param>
        private void SortCollection<T>(IList<T> collection, Func<T, string> pathSelector)
        {
            var sortedList = collection.OrderBy(pathSelector).ToList();
            collection.Clear();

            foreach (var item in sortedList)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Loading Canva
        /// </summary>
        /// <param name="originalSource"></param>
        private void GetCanvas(object originalSource)
        {
            try
            {
                if (originalSource == null)
                {
                    CanvasControl = new Canvas();

                    if (SelectedImageItem == null) return;

                    System.Drawing.Image image = System.Drawing.Image.FromFile(SelectedImageItem.FullPath);

                    CanvasControl.Width = image.Width;
                    CanvasControl.Height = image.Height;
                    return;
                }

                UIElement element = (UIElement)originalSource;

                if (element is Canvas)
                {
                    CanvasControl = (System.Windows.Controls.Canvas)element;
                }
                else if (element is System.Windows.Shapes.Line)
                {
                    System.Windows.Shapes.Line uiElementControl = (System.Windows.Shapes.Line)element;
                    CanvasControl = (Canvas)uiElementControl.Parent;
                }
                else if (element is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle uiElementControl = (System.Windows.Shapes.Rectangle)element;

                    SolidColorBrush fillBrush = (SolidColorBrush)uiElementControl.Fill;
                    if (fillBrush.Color == (System.Windows.Media.Color)ColorConverter.ConvertFromString(CropThumbsColor))
                        return;
                    else
                        CanvasControl = (Canvas)uiElementControl.Parent;
                }

            }
            catch (OutOfMemoryException)
            {
                // Do not show error
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Zoom in and out using mousewheel
        /// </summary>
        /// <param name="args"></param>
        private void ImageScaleMouseWheelChanged(MouseWheelEventArgs args)
        {
            try
            {
                if (args == null)
                {
                    return;
                }

                int delta = args.Delta;
                double increment = 0;

                if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                {
                    increment = delta > 0 ? 0.1 : -0.1;
                }
                else
                {
                    increment = delta > 0 ? 0.01 : -0.01;
                }

                double tempScale = ImageViewerScale + increment;

                // Larger value

                if (tempScale < 0.01)
                {
                    return;
                }

                ImageViewerScale = tempScale;
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        /// <summary>
        /// Load Default placeholder image 
        /// </summary>
        private void LoadPlaceholderImage()
        {
            try
            {
                string placeholderPath = @"Asset/ImagePlaceHolder.png";
                using (FileStream fs = new FileStream(placeholderPath, FileMode.Open, FileAccess.Read))
                {

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = fs;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    SelectedImageSource = bitmap;

                    ImageHeight = 2100;
                    ImageWidth = 2100;
                    ImageDPI = 96;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }

        #endregion
    }
}
