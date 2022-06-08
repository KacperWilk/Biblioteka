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
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls.Primitives;

namespace BibliotekaApp
{

    /// Logika interakcji dla klasy ManageShelfsWindow.xaml

    public partial class ManageGeneresWindow : Window
    {
        private DBConnect dB = new DBConnect();

        public ManageGeneresWindow()
        {
            InitializeComponent();

            GeneresDataGrid.DataContext = dB.GetGeneres();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = false;
            AddGenereTextBox.IsEnabled = false;

            if (lastModifyToggleButton != null)
                ModifyAction();
            else
                AddAction();

            AddGenereTextBox.Clear();

            GeneresDataGrid.DataContext = dB.GetGeneres();

            AddButton.IsEnabled = true;
            AddGenereTextBox.IsEnabled = true;
        }

        private void AddAction()
        {
            try
            {
                dB.AddGenere(AddGenereTextBox.Text);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Wystąpił błąd podczas dodawania nowego gatunku.", "Błąd podczas dodawania gatunku",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas dodawania nowego gatunku.\nUpewnij się, że gatunek, który próbujesz dodać nie został już dodany.", "Błąd podczas dodawania gatunku",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nie można dodać gatunku bez nazwy.", "Błąd podczas dodawania gatunku",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas dodawania gatunku",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ModifyAction()
        {
            try
            {
                dB.ModifyGenere(genereToModify, AddGenereTextBox.Text);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas modyfikacji gatunku.\nUpewnij się, że nowa nazwa gatunku nie pokrywa się z nazwą już istniejącego.",
                                "Błąd podczas modyfikacji gatunku",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nazwa gatunku nie może być pusta.",
                                "Błąd podczas modyfikacji gatunku",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas modyfikacji gatunku",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            lastModifyToggleButton = null;
            AddButton.Content = "Dodaj";
        }

        private void RemoveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                dB.RemoveGenere((Genere)GeneresDataGrid.SelectedItem);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas próby usunięcia gatunku.\nUpewnij się, że do gatunku, który chcesz usunąć nie ma przypisanej żadej książki, a następnie spróbuj ponownie.", 
                                "Błąd podczas usuwania gatunku",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas usuwania gatunku",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            GeneresDataGrid.DataContext = dB.GetGeneres();
        }

        ToggleButton lastModifyToggleButton;
        Genere genereToModify;

        private void ModifyToggleButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleButton currentModifyToggleButton = (ToggleButton)sender;

            if (currentModifyToggleButton != lastModifyToggleButton && lastModifyToggleButton != null)
                lastModifyToggleButton.IsChecked = false;

            lastModifyToggleButton = (bool)currentModifyToggleButton.IsChecked ? currentModifyToggleButton : null;

            if (lastModifyToggleButton != null)
            {
                genereToModify = (Genere)GeneresDataGrid.SelectedItem;
                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/modify.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Modyfikuj";
                AddGenereTextBox.Text = genereToModify.GenereName;
            }
            else
            {
                genereToModify = null;
                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/add.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Dodaj";
                AddGenereTextBox.Clear();
            }
        }

        private void GeneresDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
