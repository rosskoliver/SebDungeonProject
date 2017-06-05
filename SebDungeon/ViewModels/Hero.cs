using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace SebDungeon
{
    public class Hero : PropertyChangedBase
    {
        public int GoldCount { get; set; } = 0;
        public int HitPoints { get; set; } = 15 + _rand.Next(10);
        public string Name { get; set; }
        public bool IsAlive { get { return HitPoints > 0; } }

        private static Random _rand = new Random();

        public string GetDescription()
        {
            var list = new List<String>();
            if (GoldCount > 0)
                list.Add(string.Format("You have {0} gold pieces and {1} hit points", GoldCount, HitPoints));
            return string.Join("\r\n", list);
        }

        public string Fight(Enemy enemy)
        {
            if (enemy == null) return null;
            var list = new List<string>();
            if (_rand.Next(3) <= 1)
            {
                var damage = _rand.Next(3) + 1;
                list.Add(string.Format("you hit the enemy for {0} points of damage", damage));
                enemy.HitPoints -= damage;
                if (enemy.HitPoints <= 0)
                    list.Add(string.Format("the enemy is killed"));
            }
            else
            {
                list.Add("your attack misses!");
            }
            enemy.HasFought = false;
            return string.Join("\r\n", list);
        }
    }

}
