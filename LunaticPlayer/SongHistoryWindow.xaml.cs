﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LunaticPlayer.Classes;
using LunaticPlayer.Player;
using Newtonsoft.Json;

namespace LunaticPlayer
{
    /// <summary>
    /// Interaktionslogik für SongHistoryWindow.xaml
    /// </summary>
    public partial class SongHistoryWindow : Window
    {
        private readonly SongHistoryManager _songHistory;

        private List<Song> _allSongs;

        public SongHistoryWindow(SongHistoryManager shManager)
        {
            InitializeComponent();
            _songHistory = shManager;
            _allSongs = _songHistory.SongHistory;

            PopulateList();
        }

        /// <summary>
        /// Fills the ListBox with <see cref="Classes.Song"/> items.
        /// </summary>
        private void PopulateList()
        {
            SongList.ItemsSource = _songHistory.SongHistory;
        }

        private void CopyItem_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.CopyToClipboard);
        }

        private void SearchOnGoogle_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.SearchOnGoogle);
        }

        private void SearchOnTouhouWiki_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.SearchOnTw);
        }

        private void CopyJson_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.CopyJsonToClipboard);
        }

        /// <summary>
        /// Handles the context menu click action.
        /// </summary>
        /// <param name="action">Which action should be performed.</param>
        private void HandleClick(CMenuAction action)
        {
            if (SongList.SelectedIndex == -1) return;

            var song = (Song)SongList.Items[SongList.SelectedIndex];

            switch (action)
            {
                case CMenuAction.CopyToClipboard:
                    Clipboard.SetText($"Artist: {song.ArtistName}, Circle: {song.CircleName}, Title: {song.Title}");
                    break;
                case CMenuAction.CopyJsonToClipboard:
                    Clipboard.SetText(JsonConvert.SerializeObject(song));
                    break;
                case CMenuAction.SearchOnGoogle:
                    System.Diagnostics.Process.Start($"https://www.google.com/search?q={song.ArtistName}+{song.Title}");
                    break;
                case CMenuAction.SearchOnTw:
                    System.Diagnostics.Process.Start($"https://en.touhouwiki.net/index.php?search={song.CircleName}");
                    break;
                case CMenuAction.ShowDetails:
                    var details = new SongDetailsWindow(song);
                    details.Show();
                    break;
            }

#if DEBUG
            Console.WriteLine($"[HistoryWindow]: Action Performed - {action}");
#endif
        }

        private void ShowDetails_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.ShowDetails);
        }

        private void SearchQueryBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var liveSearch = true;

            if (e.Key == Key.Enter || liveSearch)
            {
                var box = sender as TextBox;

                if (!string.IsNullOrWhiteSpace(box.Text))
                {
                    var normalized = box.Text.ToLower();

                    SongList.ItemsSource = _allSongs.Where(s => s.Title.ToLower().Contains(normalized) || s.CircleArtist.ToLower().Contains(normalized));
                }
                else
                {
                    PopulateList();
                }
            }
        }
    }

    internal enum CMenuAction
    {
        CopyToClipboard,
        CopyJsonToClipboard,
        SearchOnGoogle,
        SearchOnTw,
        ShowDetails
    }
}
