using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training_Quest3.Model
{
    class ImageListModel : Abstract.RaisePropertyChanged
    {
        public string FileName
        {
            get { return string.IsNullOrWhiteSpace(FullPath) ? "n/a" : System.IO.Path.GetFileName(FullPath); }
        }

        private string _FullPath;

        public string FullPath
        {
            get { return _FullPath; }
            set { _FullPath = value; OnPropertyChanged(); }
        }

        private string _Color;

        public string Color
        {
            get { return _Color ?? "Transparent"; }
            set { _Color = value; OnPropertyChanged(); }
        }


        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; OnPropertyChanged(); }
        }
    }
}
