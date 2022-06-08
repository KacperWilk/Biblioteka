using BibliotekaLib;
using BibliotekaLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BibliotekaApp
{

    /// Logika interakcji dla klasy ManageBookAuthorsWindow.xaml

    public partial class ManageBookAuthorsWindow : Window
    {
        private DBConnect dB = new DBConnect();
        private List<Author> tmpAuthors = new List<Author>();

        public List<Author> Result => tmpAuthors;

        public ManageBookAuthorsWindow(List<Author> bookAuthorsToModify)
        {
            InitializeComponent();

            AllAuthorsDataGrid.DataContext = dB.GetAuthors();

            tmpAuthors = bookAuthorsToModify;

            CurrentAuthorsDataGrid.DataContext = tmpAuthors;
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            tmpAuthors.Remove((Author)CurrentAuthorsDataGrid.SelectedItem);
            CurrentAuthorsDataGrid.Items.Refresh();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            Author selectedAuthor = (Author)AllAuthorsDataGrid.SelectedItem;

            if (tmpAuthors.Where(x => x.FirstName == selectedAuthor.FirstName && x.LastName == selectedAuthor.LastName).Count() == 0)
                tmpAuthors.Add(selectedAuthor);
            else
                MessageBox.Show("Wystąpił błąd podczas dodawania autora.\nUpewnij się, że autor nie jest już na liście.",
                                "Błąd podczas dodawania autora",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

            CurrentAuthorsDataGrid.Items.Refresh();
        }

        private void AuthorsDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
