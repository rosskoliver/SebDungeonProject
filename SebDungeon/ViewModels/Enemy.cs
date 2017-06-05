using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace SebDungeon
{
    public class Enemy : PropertyChangedBase
    {
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public bool IsAlive { get { return HitPoints > 0; } }
        public bool HasFought { get; set; } = true;
        private static Random _rand = new Random();

        public string GetDescription()
        {
            var list = new List<string>();
            if (IsAlive)
                list.Add(string.Format("{0} has {1} hit points remaining", Name, HitPoints));
            else
                list.Add(string.Format("The {0} is dead", Name));
            return string.Join("\r\n", list);
        }

        public string Fight(Hero hero)
        {
            if (hero == null) return null;
            var list = new List<string>();
            if (_rand.Next(5) <= 1)
            {
                var damage = _rand.Next(3) + 1;
                list.Add(string.Format("you get hit for {0} points of damage", damage));
                hero.HitPoints -= damage;
                if (hero.HitPoints <= 0)
                    list.Add(string.Format("you are KILLED by the {0}!", Name));
            }
            else
            {
                list.Add("the enemy misses!");
            }
            HasFought = true;
            return string.Join("\r\n", list);
        }

    }

}
