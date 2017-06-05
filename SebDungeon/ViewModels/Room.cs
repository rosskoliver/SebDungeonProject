using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SebDungeon
{

    public class Room : PropertyChangedBase
    {
        public string Name { get; set; }
        public string Environment { get; set; }
        public int RoomNo { get; set; }
        public Room East { get; set; }
        public Room North { get; set; }
        public Room South { get; set; }
        public Room West { get; set; }
        public int NumGold { get; set; }
        public bool HasExit { get; set; }
        public bool HasExplored { get; set; }
        public bool CanMoveNorth { get { return North != null; } }
        public bool CanMoveSouth { get { return South != null; } }
        public bool CanMoveEast { get { return East != null; } }
        public bool CanMoveWest { get { return West != null; } }
        public bool HasGold { get { return NumGold > 0; } }
        public bool HasEnemy { get { return TheEnemy != null; } }
        public Enemy TheEnemy { get; set; }
        public bool HasPotion { get; set; }

        public static int NumRooms { get; private set; }
        private static Random _rand = new Random();

        public Room()
        {
            NumRooms++;
            RoomNo = NumRooms;
        }

        public List<string> GetDirections()
        {
            var options = new List<string>();
            if (CanMoveNorth) options.Add("North");
            if (CanMoveSouth) options.Add("South");
            if (CanMoveWest) options.Add("West");
            if (CanMoveEast) options.Add("East");

            return options;
        }

        public void Generate()
        {
            var names = new[] { "Surgery", "Workshop", "Barracks", "Magic", "Throne", "Candy", "Gold Mine", "Teleportation", "Magic Fountain", "Crypt", "Kings Coffin" };
            this.Name = names[_rand.Next(names.Length)];

            var environments = new[] { "Spooky", "Dusty", "Cobwebbed", "Cold", "Hot", "Frosty", "Misty", "Foggy", "Damp", "Slippery", "Normal", "Empty" };
            this.Environment = environments[_rand.Next(environments.Length)];

            if (_rand.Next(3) == 0) this.NumGold = _rand.Next(100) + 50;

            if (_rand.Next(2) == 0 && North == null && NumRooms < 20) (North = new Room() { South = this }).Generate();
            if (_rand.Next(3) == 0 && South == null && NumRooms < 20) (South = new Room() { North = this }).Generate();
            if (_rand.Next(3) == 0 && West == null && NumRooms < 20) (West = new Room() { East = this }).Generate();
            if (_rand.Next(5) == 0 && East == null && NumRooms < 20) (East = new Room() { West = this }).Generate();

            if (_rand.Next(10) == 0) HasExit = true;
            if (_rand.Next(4) == 0)
            {
                var enemyNames = new[] { "Goblin", "Orc", "Drow Elf", "Dragon", "Minotaur", "Howling Hag", "Vampire", "Sebbie the epic coder" };
                TheEnemy = new Enemy() { HitPoints = _rand.Next(8) + 5, Name = enemyNames[_rand.Next(enemyNames.Length)] };
            }
            if (_rand.Next(4) == 0) HasPotion = true;
        }

        public string GetDescription()
        {
            var list = new List<string>();
            list.Add($"The {Environment} {Name} Room (#{RoomNo}/{NumRooms})");
            if (HasExplored) list.Add("You have already explored this area");
            if (this.NumGold > 0) list.Add($"There is {NumGold} gold pieces on the floor");
            if (North == null && South == null && West == null && East == null)
                list.Add("There are no areas to move to in any direction!");
            else
                list.Add(string.Format("There are areas to the {0}", string.Join(", ", GetDirections())));
            if (HasExit)
                list.Add("This room has an EXIT!");
            if (HasEnemy)
            {
                list.Add(string.Format("The room has a {0}!", TheEnemy.Name));
                list.Add(string.Format(TheEnemy.GetDescription()));
            }
            if (HasPotion)
                list.Add(string.Format("This Room has a potion!"));
            return string.Join("\r\n", list);
        }

        public string GetAsciiDiagram()
        {

            var diagram = @"
╔═══NNN═══╗
║    1 ggg║
W g    g  E
║X   p  g ║
╚═══SSS═══╝
";
            diagram = diagram.Replace("N", this.North == null ? "═" : " ");
            diagram = diagram.Replace("S", this.South == null ? "═" : " ");
            diagram = diagram.Replace("W", this.West == null ? "║" : " ");
            diagram = diagram.Replace("E", this.East == null ? "║" : " ");
            diagram = diagram.Replace("g", this.NumGold > 0 ? "°" : " ");
            diagram = diagram.Replace("X", this.HasExit ? "▓" : " ");
            diagram = diagram.Replace("1", this.TheEnemy != null && this.TheEnemy.IsAlive ? "§" : " ");
            diagram = diagram.Replace("p", this.HasPotion ? "♥" : " ");
            return diagram;
        }
    }

}
