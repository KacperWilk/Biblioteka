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

    public partial class ManageShelfsWindow : Window
    {
        private DBConnect dB = new DBConnect();

        public ManageShelfsWindow()
        {
            InitializeComponent();

            ShelfsDataGrid.DataContext = dB.GetShelfs();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = false;
            AddShelfTextBox.IsEnabled = false;

            if (lastModifyToggleButton != null)
                ModifyAction();
            else
                AddAction();

            AddShelfTextBox.Clear();

            ShelfsDataGrid.DataContext = dB.GetShelfs();

            AddButton.IsEnabled = true;
            AddShelfTextBox.IsEnabled = true;
        }

        private void AddAction()
        {
            try
            {
                dB.AddShelf(AddShelfTextBox.Text);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Wystąpił błąd podczas dodawania nowej półki.",
                                "Błąd podczas dodawania półki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas dodawania nowej półki.\nUpewnij się, że półka, którą próbujesz dodać nie została już dodana.",
                                "Błąd podczas dodawania półki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nie można dodać półki bez nazwy.",
                                "Błąd podczas dodawania półki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas dodawania półki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ModifyAction()
        {
            try
            {
                dB.ModifyShelf(shelfToModify, AddShelfTextBox.Text);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas modyfikacji półki.\nUpewnij się, że nowa nazwa półki nie pokrywa się z nazwą już istniejącej.",
                                "Błąd podczas modyfikacji pólki",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nazwa pólki nie może być pusta.",
                                "Błąd podczas modyfikacji półki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas modyfikacji półki",
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
                dB.RemoveShelf((Shelf)ShelfsDataGrid.SelectedItem);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Wystąpił błąd podczas próby usunięcia półki.\nUpewnij się, że do półki, którą chcesz usunąć nie ma przypisanej żadej książki, a następnie spróbuj ponownie.", 
                                "Błąd podczas usuwania półki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + "\n\n" + ex.Message,
                                "Błąd podczas usuwania półki",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            ShelfsDataGrid.DataContext = dB.GetShelfs();
        }

        ToggleButton lastModifyToggleButton;
        Shelf shelfToModify;

        private void ModifyToggleButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleButton currentModifyToggleButton = (ToggleButton)sender;

            if (currentModifyToggleButton != lastModifyToggleButton && lastModifyToggleButton != null)
                lastModifyToggleButton.IsChecked = false;

            lastModifyToggleButton = (bool)currentModifyToggleButton.IsChecked ? currentModifyToggleButton : null;

            if (lastModifyToggleButton != null)
            {
                shelfToModify = (Shelf)ShelfsDataGrid.SelectedItem;
                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/modify.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Modyfikuj";
                AddShelfTextBox.Text = shelfToModify.ShelfName;
            }
            else
            {
                shelfToModify = null;
                AddButtonImage.Source = new BitmapImage(new Uri(@"Assets/Icons/add.png", UriKind.Relative));
                AddButtonTextBlock.Text = "Dodaj";
                AddShelfTextBox.Clear();
            }
        }

        private void ShelfsDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
