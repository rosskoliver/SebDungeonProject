using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SebDungeon.ViewModels
{
    public class ShellViewModel : Screen
    {
        public Hero Hero { get; set; }
        public Room Room { get; set; }
        public string SelectedOption { get; set; }
        public string Message { get; set; } = "hello\r\n";
        public string AlertMessage { get; set; }
        public bool CanFight { get; set; }

        public ShellViewModel()
        {

        }

        public void Start()
        {
            ShowMessage("starting");
            Task.Run(() =>
            {
                this.Hero = new Hero() { Name = "The Hero"  };
                this.Room = new Room();
                Room.Generate();
                Room.HasExit = true;

                ShowMessage(Room.GetDescription());
                HandleOption(null);
            });

        }

        public void HandleOption(string option)
        {
            if(!Hero.IsAlive)
            {
                ShowMessage("You are dead");
                return;
            }
            if (option == "Exit")
            {
                ShowMessage("You found the exit! congratulations");
            }
            if (option == "Pickup")
            {
                ShowMessage("you pick up {0} gold pieces", Room.NumGold);
                Hero.GoldCount += Room.NumGold;
                Room.NumGold = 0;
            }
            if (option == "Fight")
            {
                string result;
                if (Room.TheEnemy.HasFought)
                    result = Hero.Fight(Room.TheEnemy);
                else
                    result = Room.TheEnemy.Fight(Hero);
                ShowMessage(result);

                var didMiss = result.Contains("miss");
                if(!didMiss)
                {
                    var mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri("Audio/Swords Clashing-SoundBible.com-912903192.mp3", UriKind.Relative));
                    mediaPlayer.Play();
                }
            }

            Room nextRoom = null;
            if (option == "North") nextRoom = Room.North;
            if (option == "East") nextRoom = Room.East;
            if (option == "South") nextRoom = Room.South;
            if (option == "West") nextRoom = Room.West;

            Room.HasExplored = true;

            if (nextRoom != null)
            {
                ShowMessage("\r\n" + nextRoom.GetDescription());
                Room = nextRoom;
            }
            CanFight = Room.TheEnemy != null && Room.TheEnemy.IsAlive;
            
        }

        private void ShowMessage(string format, params object[] args)
        {
            this.Message += string.Format(format, args) + "\r\n";
        }


    }
}
