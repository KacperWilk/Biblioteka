using BibliotekaLib;
using BibliotekaLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class AddModifyBookWindow : Window
    {
        private DBConnect dB = new DBConnect();
        private Book bookToModify = null;
        private List<Author> authorsList = new List<Author>();

        public AddModifyBookWindow(Book book = null)
        {
            bookToModify = book;

            InitializeComponent();

            ShelfsComboBox.ItemsSource = dB.GetShelfs();
            GeneresComboBox.ItemsSource = dB.GetGeneres();

            if (bookToModify != null)
            {
                authorsList = dB.GetBookAuthors(bookToModify);
                TitleTextBox.Text = bookToModify.Title;
                PurchaseDateDatePicker.SelectedDate = bookToModify.PurchaseDate;

                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/modify.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Modyfikuj";
                this.Title = "Modyfikuj książkę";
                ReadCountResetButton.Visibility = Visibility.Visible;

                if (book.ReadCount == 0)
                    ReadCountResetButton.IsEnabled = false;
            }
        }

        private void ManageShelfsButtonClick(object sender, RoutedEventArgs e)
        {
            ManageShelfsWindow manageShelfsWindow = new ManageShelfsWindow();
            manageShelfsWindow.Closing += ManageShelfsWindowClosing;
            manageShelfsWindow.ShowDialog();
        }

        private void ManageShelfsWindowClosing(object sender, EventArgs e)
        {
            ShelfsComboBox.ItemsSource = dB.GetShelfs();
        }

        private void ManageGeneresButtonClick(object sender, RoutedEventArgs e)
        {
            ManageGeneresWindow manageGeneresWindow = new ManageGeneresWindow();
            manageGeneresWindow.Closing += ManageGeneresWindowClosing;
            manageGeneresWindow.ShowDialog();
        }

        private void ManageGeneresWindowClosing(object sender, EventArgs e)
        {
            GeneresComboBox.ItemsSource = dB.GetGeneres();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            TitleTextBox.IsEnabled = false;
            PurchaseDateDatePicker.IsEnabled = false;
            GeneresComboBox.IsEnabled = false;
            ManageGeneresButton.IsEnabled = false;
            ShelfsComboBox.IsEnabled = false;
            ManageShelfsButton.IsEnabled = false;

            if (bookToModify != null)
                ModifyAction();
            else
                AddAction();

            TitleTextBox.IsEnabled = true;
            PurchaseDateDatePicker.IsEnabled = true;
            GeneresComboBox.IsEnabled = true;
            ManageGeneresButton.IsEnabled = true;
            ShelfsComboBox.IsEnabled = true;
            ManageShelfsButton.IsEnabled = true;
        }

        private void AddAction()
        {
            bool bookAddSuccess = true;

            try
            {
                dB.AddBook(
                    TitleTextBox.Text,
                    (DateTime)PurchaseDateDatePicker.SelectedDate,
                    (Genere)GeneresComboBox.SelectedItem,
                    (Shelf)ShelfsComboBox.SelectedItem,
                    authorsList
                    );
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nie wszystkie dane zotsały uzupełnione.",
                                "Błąd podczas dodawania książki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);

                bookAddSuccess = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Data zakupu książki nie może być z przyszłości.\nPopraw datę i spróbuj ponownie.",
                                "Błąd podczas dodawania książki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);

                bookAddSuccess = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas dodawania książki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                bookAddSuccess = false;
            }

            if (bookAddSuccess)
                this.Close();
        }

        private void ModifyAction()
        {
            bool bookModifySuccess = true;

            try
            {
                dB.ModifyBook(
                    bookToModify,
                    TitleTextBox.Text,
                    (DateTime)PurchaseDateDatePicker.SelectedDate,
                    (Genere)GeneresComboBox.SelectedItem,
                    (Shelf)ShelfsComboBox.SelectedItem,
                    authorsList
                    );
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nie wszystkie dane zotsały uzupełnione.",
                                "Błąd podczas modyfikacji książki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);

                bookModifySuccess = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Data zakupu książki nie może być z przyszłości.\nPopraw datę i spróbuj ponownie.",
                                "Błąd podczas modyfikacji książki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);

                bookModifySuccess = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas modyfikacji książki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                bookModifySuccess = false;
            }

            if (bookModifySuccess)
                this.Close();
        }

        private void ReadCountResetButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                dB.ResetBookReadCount(bookToModify);
                ReadCountResetButton.IsEnabled = false;
                ReadCountResetButton.Content = "Licznik przeczytań zresetowany";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Problem podczas resetowania licznika przeczytań",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ManageBookAuthorsButtonClick(object sender, RoutedEventArgs e)
        {
            ManageAuthorsWindow manageAuthorsWindow = new ManageAuthorsWindow();
            manageAuthorsWindow.ShowDialog();
        }

        private void AddRemoveAuthorsButtonClick(object sender, RoutedEventArgs e)
        {
            ManageBookAuthorsWindow manageBookAuthorsWindow = new ManageBookAuthorsWindow(authorsList);
            manageBookAuthorsWindow.ShowDialog();
            authorsList = manageBookAuthorsWindow.Result;
        }
    }
}