﻿using System.Collections.Generic;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using RWGame.Classes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RWGame.ViewModels
{
    public class GameHistoryDisplayData : INotifyPropertyChanged
    {
        public GameHistoryDisplayData(ServerWorker serverWorker, SystemSettings systemSettings, INavigation Navigation)
        {
            this.Navigation = Navigation;
            GameHistoryModel = new GameHistoryModel(serverWorker, systemSettings);
        }
        public string Title { get { return "Games History"; } }
        public string GameListViewEmptyMessageText { get { return "Here we place your finished games.\nThanks for playing =)"; } }
        private GameHistoryModel GameHistoryModel { get; set; }
        public INavigation Navigation { get; set; }
        public ObservableCollection<ElementsOfViewCell> CustomListViewRecords { get; } = new ObservableCollection<ElementsOfViewCell>();
        public List<Game> GamesList { get; set; }
        public bool IsCustomListViewVisible { get; set; }
        public bool IsGameListViewEmptyMessageVisible { get; set; }
        public bool IsCustomListViewRefreshing { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void UpdateGameList()
        {
            await TaskUpdateGameList();
            IsCustomListViewRefreshing = false;
        }
        public async Task TaskUpdateGameList()
        {
            await GameHistoryModel.UpdateGameList();
            GamesList = GameHistoryModel.GamesList;
            CustomListViewRecords.Clear();
            if (GamesList != null && GamesList.Count > 0)
            {
                for (int i = 0; i < GamesList.Count; i++)
                {
                    if (GamesList[i].GameState == GameStateEnum.END)
                    {
                        CustomListViewRecords.Add(new ElementsOfViewCell(GamesList[i]));
                    }
                }
            }
            if (CustomListViewRecords.Count == 0)
            {
                IsCustomListViewVisible = false;
                IsGameListViewEmptyMessageVisible = true;
            }
            else
            {
                IsCustomListViewVisible = true;
                IsGameListViewEmptyMessageVisible = false;
            }
        }
        public async void LoadSelectedGame(ElementsOfViewCell selectedItem)
        {
            await GameHistoryModel.GetSelectedGameData(selectedItem.game.IdGame);
            await Navigation.PushAsync(GameHistoryModel.GameField);
            GameHistoryModel.IsGameStarted = false;
            UpdateGameList();
        }

        public void OnGameHistoryPageAppearing()
        {
            _ = TaskUpdateGameList();
        }
    }

    public class GameHistoryViewModel : INotifyPropertyChanged
    {
        public GameHistoryDisplayData GameHistoryDisplayData { get; set; }

        public GameHistoryViewModel(ServerWorker serverWorker, SystemSettings systemSettings, INavigation Navigation)
        {
            GameHistoryDisplayData = new GameHistoryDisplayData(serverWorker, systemSettings, Navigation);
            RefreshGamesListCommand = new Command(GameHistoryDisplayData.UpdateGameList);
            OnGameHistoryPageAppearingCommand = new Command(GameHistoryDisplayData.OnGameHistoryPageAppearing);
        }

        public Command RefreshGamesListCommand { get; set; }
        public Command OnGameHistoryPageAppearingCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
