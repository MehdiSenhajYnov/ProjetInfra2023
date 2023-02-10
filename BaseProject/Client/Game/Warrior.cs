using System;
namespace GameNetClient
{
    public class Warrior : Character, ITank
    {
        public bool Bravery = false; 
        public int AttackPower {get; set;}

        public Warrior() : base() {
            AttackPower = 25;
        }

        public Warrior(string _Name, float _MaxHealth) : base(_Name, _MaxHealth)
        {
            AttackPower = 25;
        }
        
        public override void Special() {
            if (Health <= 30) {
                Bravery = true;
            }
        }

        public override void CibledSpecial(Character cible) {
            int damageInflicted = AttackPower;
            if (Bravery) damageInflicted = AttackPower + 15;
            cible.TakeDamage(damageInflicted);
        }

        public override string ToString()
        {
            return base.ToString() + " | Classe : Guerrier | Bravoure : " + Bravery;
        }
        public void DoubleHit(Character cible)
        {
            Health -= 10;
            if (Health < 0) Health = 0;
            cible.TakeDamage(AttackPower*2);
        }

        public override void AlternatifAtk(Character cible) {
            DoubleHit(cible);
        }

        public override void SetUniqueValue(byte newValue) {
            Bravery = newValue == 1 ? true : false;
        }
    }

}