using System;
namespace GameNetClient
{
    public class Paladin: Character, ITank, IHealer
    {
        public int HealPower {get; set;}
        public int AttackPower {get; set;}

        public int Buff{get; set;} = 0;

        public Paladin() : base() {
            HealPower = 15;
            AttackPower = 25;
        }

        public Paladin(string _Name, float _MaxHealth) : base(_Name, _MaxHealth)
        {
            HealPower = 15;
            AttackPower = 25;
        }
        
        public override void Special()
        {
            Health += HealPower + Buff;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public override void CibledSpecial(Character cible)
        {
            cible.TakeDamage(AttackPower + Buff);
            Buff += 3;
            if (Buff > 15) Buff = 15;
        }

        public void DoubleHeal(Character cibleOne, Character cibleTwo) {
            cibleOne.Health += HealPower;
            if (cibleOne.Health > cibleOne.MaxHealth) cibleOne.Health = cibleOne.MaxHealth; 
            cibleTwo.Health += HealPower;
            if (cibleTwo.Health > cibleTwo.MaxHealth) cibleTwo.Health = cibleTwo.MaxHealth; 
        }


        public void DoubleHit(Character cible)
        {
            Health -= 10;
            if (Health < 0) Health = 0;
            cible.TakeDamage(AttackPower*2);
        }

        public override string ToString()
        {
            return base.ToString() + " | Classe : Paladin | Buff : " + Buff;
        }

        public override void AlternatifAtk(Character cible) {
            DoubleHit(cible);
        }

        public override void SetUniqueValue(byte newValue) {
            Buff = (int)newValue;
        }

    }
}