﻿using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RWGame.Models
{
    public class RealPlayerChoiceModel : INotifyPropertyChanged
    {
        private readonly ServerWorker serverWorker;
        private readonly SystemSettings systemSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        public RealPlayerChoiceModel(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            this.serverWorker = serverWorker;
            this.systemSettings = systemSettings;
        }
        public List<Player> PlayerList { get; set; }
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }
        public GameField GameField { get; set; }
        public bool CancelGame { get; set; } = false;
        public string Login { get; set; }

        public async Task TaskUpdatePlayerList()
        {
            if (Login != "" && Login != null)
            {
                PlayerList = await serverWorker.TaskGetPlayerList(Login);
            }       
        }
        public async Task StartGame(int SelectedPlayerId)
        {
            await CreateGame(SelectedPlayerId);
            await GetCancelGame();
            if (!CancelGame)
            {
                await GetGameStateInfo();
                CreateGameField();
            }
        }
        public async Task CreateGame(int SelectedPlayerId)
        {
            Game = await GameProcesses.MakeGameWithPlayer(serverWorker, SelectedPlayerId);
        }
        public async Task GetCancelGame()
        {
            CancelGame = await GameProcesses.StartGame(serverWorker, Game);
        }
        public async Task GetGameStateInfo()
        {
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
        public void CreateGameField()
        {
            GameField = new GameField(serverWorker, systemSettings, Game, GameStateInfo);
        }
        public async Task CallCancelGame()
        {
            await serverWorker.TaskCancelGame(Game.IdGame);
        }
    }
}
