using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace SebDungeon
{
    public class Hero : PropertyChangedBase
    {
        private int _maxHitPoints = 15 + _rand.Next(10);
        public int GoldCount { get; set; } = 0;
        public int HitPoints { get; set; }
        public int PotionCount { get; set; } = _rand.Next(2);
        public string Name { get; set; }
        public bool IsAlive { get { return HitPoints > 0; } }
        public bool CanUse {  get { return PotionCount > 0; } }
        private static Random _rand = new Random();

        public Hero()
        {
            HitPoints = _maxHitPoints;
        }

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

        public string DrinkPotion()
        {
            if (PotionCount > 0)
            {
                PotionCount--;
                var curHitpoints = HitPoints;
                HitPoints += _rand.Next(5) + 3;
                if (HitPoints > _maxHitPoints)
                    HitPoints = _maxHitPoints;
                return string.Format("you gain {0} hitpoints", HitPoints - curHitpoints);
            }
            return "you have no potions!";
        }
    }

}
