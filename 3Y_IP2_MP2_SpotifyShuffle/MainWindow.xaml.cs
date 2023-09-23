using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SongDatabaseApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Song> allSongs;
        private ObservableCollection<Song> displayedSongs;
        private bool isTextBoxFocused = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataGrid();
            SubscribeToKeyPressEvents();
            SubscribeToTextBoxFocusEvents();
        }

        private void InitializeDataGrid()
        {
            allSongs = new ObservableCollection<Song>();
            displayedSongs = allSongs;
            songListBox.ItemsSource = displayedSongs;
        }

        private void LoadCSV(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var data = lines.Skip(1).Select(line => line.Split(',')).ToList();

                allSongs.Clear();
                foreach (var lineData in data)
                {
                    if (lineData.Length >= 3)
                    {
                        allSongs.Add(new Song
                        {
                            Title = lineData[0],
                            Artist = lineData[1],
                            Year = lineData[2]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void ShuffleSongs()
        {
            var random = new Random();
            displayedSongs = new ObservableCollection<Song>(displayedSongs.OrderBy(song => random.Next()));
            songListBox.ItemsSource = displayedSongs;
        }

        private void SaveSongsToCSV(string filePath)
        {
            try
            {
                var lines = new List<string> { "Title,Artist,Year" };
                lines.AddRange(displayedSongs.Select(song => $"{song.Title},{song.Artist},{song.Year}"));
                File.WriteAllLines(filePath, lines);
                MessageBox.Show("Songs saved to CSV successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void songListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (songListBox.SelectedItem != null)
            {
                string artist = ((Song)songListBox.SelectedItem).Artist;
                displayedSongs = new ObservableCollection<Song>(allSongs.Where(song => song.Artist == artist));
                songListBox.ItemsSource = displayedSongs;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            displayedSongs = allSongs;
            songListBox.ItemsSource = displayedSongs;
        }

        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            displayedSongs = new ObservableCollection<Song>(allSongs
                .Where(song => song.Title.ToLower().Contains(searchText) ||
                               song.Artist.ToLower().Contains(searchText) ||
                               song.Year.ToLower().Contains(searchText)));
            songListBox.ItemsSource = displayedSongs;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            ShuffleSongs();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "CSV Files|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveSongsToCSV(saveFileDialog.FileName);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "CSV Files|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadCSV(openFileDialog.FileName);
            }
        }

        private void SubscribeToKeyPressEvents()
        {
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!isTextBoxFocused)
            {
                if (e.Key == Key.S)
                {
                    ShuffleSongs();
                }
                else if (e.Key == Key.P)
                {
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                    saveFileDialog.Filter = "CSV Files|*.csv";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        SaveSongsToCSV(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void SubscribeToTextBoxFocusEvents()
        {
            searchTextBox.GotFocus += (sender, e) => isTextBoxFocused = true;
            searchTextBox.LostFocus += (sender, e) => isTextBoxFocused = false;
        }
    }
}
