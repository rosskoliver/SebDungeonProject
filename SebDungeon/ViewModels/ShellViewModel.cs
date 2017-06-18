using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public bool CanPickup { get; set; }
        public string AsciiDiagram { get; set; }
        public bool IsStarted { get; set; }
        public ImageSource RoomImage { get; set; }

        public ShellViewModel()
        {

        }

        public void Start()
        {
            IsStarted = true;
            ShowMessage("starting");

            this.Hero = new Hero() { Name = "The Hero" };
            this.Room = new Room();
            Room.Generate();
            Room.HasExit = true;
            ShowMessage(Room.GetDescription());

            HandleOption(null);


        }

        public void HandleOption(string option)
        {
            string soundFile = null;

            if (!Hero.IsAlive)
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
                if (Room.HasGold)
                {
                    soundFile = "Audio/cash-register-05.wav";
                    ShowMessage("you pick up {0} gold pieces", Room.NumGold);
                    Hero.GoldCount += Room.NumGold;
                    Room.NumGold = 0;
                }
                if (Room.HasPotion)
                {
                    ShowMessage("You Pickup the Potion!");
                    Hero.PotionCount += 1;
                    Room.HasPotion = false;
                }
            }
            if (option == "Use")
            {
                if (Hero.PotionCount > 0)
                {
                    var result = Hero.DrinkPotion();
                    ShowMessage(result);
                }
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
                if(didMiss)
                    soundFile = "Audio/Swoosh 3-SoundBible.com-1573211927.mp3"; // sebbie to do
                else
                    soundFile = "Audio/Swords Clashing-SoundBible.com-912903192.mp3";
             }

            if (soundFile != null)
            {
                var mediaPlayer = new MediaPlayer();
                mediaPlayer.Open(new Uri(soundFile, UriKind.Relative));
                mediaPlayer.Play();
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
            AsciiDiagram = Room.GetAsciiDiagram();
            RoomImage = CreateRoomImage(Room);
            CanPickup = Room.HasGold || Room.HasPotion;
        }

        private ImageSource CreateRoomImage(Room room)
        {
            var asciiDiagram = room.GetAsciiDiagram();
            var walls = BitmapFactory.FromContent(@"Graphics\Wilkituska_s_A4.png");
            var gold = BitmapFactory.FromContent(@"Graphics\Yiingolds.png");
            var potion = BitmapFactory.FromContent(@"Graphics\pixel_potions_set_2_by_linaivelle.png");
            var enemy = BitmapFactory.FromContent(@"Graphics\goblin-hi[1].png");
            var exit = BitmapFactory.FromContent(@"Graphics\stairs.png");

            var writeableBmp = BitmapFactory.New(13 * 64, 10 * 64);
            using (var context = writeableBmp.GetBitmapContext())
            {
                writeableBmp.Clear(Colors.LightGray);
               for (int pass = 0; pass < 2; pass++)
                {
                    var x = 1;
                    var y = 0;
                    foreach (var c in asciiDiagram)
                    {

                        if (c == '\n') { y++; x = 1; continue; }
                        if (c == '\r') continue;

                        var sourceImage = walls;
                        var x1 = 15.0;
                        var y1 = 0.0;
                        var w1 = 32;
                        var h1 = 32;
                        if (pass == 0)
                        {
                            x1 = 15; y1 = 0;
                        }
                        if(pass == 1)
                        {
                            if (c == '╔') { x1 = 0; y1 = 1; }
                            if (c == '║') { x1 = 0; y1 = 1.5; }
                            if (c == '╚') { x1 = 0; y1 = 2; }
                            if (c == '╝') { x1 = 1; y1 = 2; }
                            if (c == '═') { x1 = 0.5; y1 = 1; }
                            if (c == '╗') { x1 = 1; y1 = 1; }
                            if (c == '=') { x1 = 0.5; y1 = 2; }
                            if (c == '|') { x1 = 1; y1 = 1.5; }
                            if (c == '°') { x1 = 160 / 32.0; y1 = 80 / 32.0; sourceImage = gold; w1 = 42; h1 = 42;  }
                            if (c == '♥') { x1 = 120 / 32.0; y1 = 40 / 32.0; sourceImage = potion; w1 = 45; h1 = 71; }
                            if (c == '§') { x1 = 0; y1 = 0; sourceImage = enemy; w1 = 479; h1 = 588; }
                            if (c == '▓') { x1 = 0; y1 = 0; sourceImage = exit; w1 = 48; h1 = 68; }
                        }
                        writeableBmp.Blit(new System.Windows.Rect(x * 64, y * 64, 64, 64), sourceImage, new System.Windows.Rect(x1 * 32, y1 * 32, w1, h1));

                        x++;
                    }
                }
            }
            return writeableBmp;
        }


        private void ShowMessage(string format, params object[] args)
        {
            this.Message += string.Format(format, args) + "\r\n";
        }


    }
}
