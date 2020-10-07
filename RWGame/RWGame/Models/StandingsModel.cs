﻿using RWGame.Classes.ResponseClases;
using RWGame.Classes;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RWGame.Models
{
    class StandingsModel : INotifyPropertyChanged
    {
        private ServerWorker serverWorker;
        public Standings standings { get; set; }
        public string UserLogin { get { return serverWorker.UserLogin; } }
        public StandingsModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
            _ = UpdateModelStandings();
        }
        public async Task UpdateModelStandings()
        {
            standings = await serverWorker.TaskGetStandings();
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}