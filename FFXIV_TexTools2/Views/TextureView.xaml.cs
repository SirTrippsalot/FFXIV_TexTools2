    // FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Windows.Controls;
using FFXIV_TexTools2;
using FFXIV_TexTools2.Helpers;
using FFXIV_TexTools2.Resources;
using FFXIV_TexTools2.ViewModel;
using FFXIV_TexTools2.Views;
using System.Windows;
using Xceed.Wpf;

namespace FFXIV_TexTools2.Views
{

    /// <summary>
    /// Interaction logic for TextureView.xaml
    /// </summary>
    public partial class TextureView : UserControl
    {
        public TextureView()
        {
            InitializeComponent();
        }

        public void CenterContents()
        {
            zoomBox.CenterContent();
            zoomBox.FitToBounds();
        }

        private void fullPathLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            CenterContents();
            e.Handled = true;

        }

        
            MainViewModel mViewModel;
            private void Save_All_DDS_Click(object sender, RoutedEventArgs e)
            {
                mViewModel.TextureVM.SaveAllDDS();
            }
        
    }
}
