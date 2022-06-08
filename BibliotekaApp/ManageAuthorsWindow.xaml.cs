using BibliotekaLib;
using BibliotekaLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BibliotekaApp
{

    /// Logika interakcji dla klasy ManageAuthorsWindow.xaml

    public partial class ManageAuthorsWindow : Window
    {
        private DBConnect dB = new DBConnect();

        public ManageAuthorsWindow()
        {
            InitializeComponent();

            AuthorsDataGrid.DataContext = dB.GetAuthors();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = false;
            FirstNameTextBox.IsEnabled = false;
            LastNameTextBox.IsEnabled = false;

            if (lastModifyToggleButton != null)
                ModifyAction();
            else
                AddAction();

            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();

            AuthorsDataGrid.DataContext = dB.GetAuthors();

            AddButton.IsEnabled = true;
            FirstNameTextBox.IsEnabled = true;
            LastNameTextBox.IsEnabled = true;
        }

        private void AddAction()
        {
            try
            {
                dB.AddAuthor(FirstNameTextBox.Text, LastNameTextBox.Text);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Wystąpił błąd podczas dodawania nowego autora.",
                                "Błąd podczas dodawania autora",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas dodawania nowego autora.\nUpewnij się, że autor, którego próbujesz dodać nie został już dodany.",
                                "Błąd podczas dodawania autora",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nie można dodać autora bez imienia lub nazwiska.",
                                "Błąd podczas dodawania autora",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas dodawania autora",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ModifyAction()
        {
            try
            {
                dB.ModifyAuthor(authorToModify, FirstNameTextBox.Text, LastNameTextBox.Text);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas modyfikacji autora.\nUpewnij się, że nowe dane autora nie pokrywają się z danymi istniejącego już autora.",
                                "Błąd podczas modyfikacji autora",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Imię lub nazwisko nie mogą być puste.",
                                "Błąd podczas modyfikacji autora",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas modyfikacji autora",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            lastModifyToggleButton = null;
            AddButton.Content = "Dodaj";
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                dB.RemoveAuthor((Author)AuthorsDataGrid.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas usuwania autora",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            AuthorsDataGrid.DataContext = dB.GetAuthors();
        }

        ToggleButton lastModifyToggleButton;
        Author authorToModify;

        private void ModifyToggleButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleButton currentModifyToggleButton = (ToggleButton)sender;

            if (currentModifyToggleButton != lastModifyToggleButton && lastModifyToggleButton != null)
                lastModifyToggleButton.IsChecked = false;

            lastModifyToggleButton = (bool)currentModifyToggleButton.IsChecked ? currentModifyToggleButton : null;

            if (lastModifyToggleButton != null)
            {
                authorToModify = (Author)AuthorsDataGrid.SelectedItem;
                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/modify.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Modyfikuj";
                FirstNameTextBox.Text = authorToModify.FirstName;
                LastNameTextBox.Text = authorToModify.LastName;
            }
            else
            {
                authorToModify = null;
                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/add.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Dodaj";
                FirstNameTextBox.Clear();
                LastNameTextBox.Clear();
            }
        }

        private void AuthorsDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
