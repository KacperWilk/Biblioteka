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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BibliotekaLib;
using BibliotekaLib.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaApp
{
    public partial class MainWindow : Window
    {
        private DBConnect dB = new DBConnect();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                BooksDataGrid.DataContext = dB.GetBooks();
            }
            catch (SqlException)
            {
                //Pusty MessageBox zapobigający zamknięciu komunikatu przez splashscreen.
                MessageBox.Show("");

                var databaseError = MessageBox.Show("Wystąpił problem z bazą danych programu Biblioteka.\n\n" + 
                                                    "Upewnij się, że silnik bazy danych działa poprawnie oraz istnieje tam baza danych o nazwie \"Biblioteka\", a następnie spróbuj ponownie uruchomić program.\n\n" +
                                                    "Program Biblioteka może również utworzyć nową, pustą bazę danych. Czy chcesz aby utworzył ją teraz?", 
                                                    "Problem z bazą dnaych programu Biblioteka",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Error);

                if (databaseError == MessageBoxResult.Yes)
                {
                    try
                    {
                        dB.CreateDatabase();

                        MessageBox.Show("Program Biblioteka pomyślnie utworzył nową bazę danych.", 
                                        "Tworzenie bazy danych programu Biblioteka",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                        "Problem z bazą dnaych programu Biblioteka",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);

                        Environment.Exit(0);
                    }
                }
                else
                {
                    MessageBox.Show("Program Biblioteka zostanie zamknięty.", 
                                    "Problem z bazą dnaych programu Biblioteka",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    Environment.Exit(0);
                }
            }
        }

        private void AddBookButtonClick(object sender, RoutedEventArgs e)
        {
            AddModifyBookWindow addBookWindow = new AddModifyBookWindow();
            addBookWindow.Closing += AnyWindowClosing;
            addBookWindow.ShowDialog();
        }

        private void ManageShelfsButtonClick(object sender, RoutedEventArgs e)
        {
            ManageShelfsWindow manageShelfsWindow = new ManageShelfsWindow();
            manageShelfsWindow.Closing += AnyWindowClosing;
            manageShelfsWindow.ShowDialog();
        }

        private void AnyWindowClosing(object sender, EventArgs e)
        {
            BooksDataGrid.DataContext = dB.GetBooks();
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                dB.RemoveBook((Book)BooksDataGrid.SelectedItem);
                BooksDataGrid.DataContext = dB.GetBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Problem z bazą dnaych programu Biblioteka",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ManageAuthorsButtonClick(object sender, RoutedEventArgs e)
        {
            ManageAuthorsWindow manageAuthorsWindow = new ManageAuthorsWindow();
            manageAuthorsWindow.Closing += AnyWindowClosing;
            manageAuthorsWindow.ShowDialog();
        }

        private void ReadButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                dB.IncrementBookReadCount((Book)BooksDataGrid.SelectedItem);
                BooksDataGrid.DataContext = dB.GetBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Problem podczas inkrementacji wskaźnika przeczytań",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ModifyButtonClick(object sender, RoutedEventArgs e)
        {
            AddModifyBookWindow addBookWindow = new AddModifyBookWindow((Book)BooksDataGrid.SelectedItem);
            addBookWindow.Closing += AnyWindowClosing;
            addBookWindow.ShowDialog();
        }

        private void BooksDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
