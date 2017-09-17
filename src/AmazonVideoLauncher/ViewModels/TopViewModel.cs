using AmazonVideoLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AmazonVideoLauncher.ViewModels
{
    class TopViewModel : ObservableObject
    {
        private ObservableCollection<VideoTitle> _Items;
        public ObservableCollection<VideoTitle> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }

        public TopViewModel()
        {
            this._Items = new ObservableCollection<VideoTitle>();
        }
    }
}
