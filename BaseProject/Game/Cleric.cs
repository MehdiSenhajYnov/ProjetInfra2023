using System;
namespace CSharpDiscovery.Examen
{
    public class Cleric : Character, IHealer
    {
        public float Mana = 100;
        public int HealPower {get; set;}
        public override void Special() {
            Mana += 10;
            if (Mana > 100) Mana = 100;

        }

        public Cleric() : base() {
            HealPower = 15;
        }

        public Cleric(string _Name, float _MaxHealth) : base(_Name, _MaxHealth)
        {
            HealPower = 15;
        }

        public override void CibledSpecial(Character cible) {
            cible.Health += HealPower;
            if (cible.Health > cible.MaxHealth) cible.Health = cible.MaxHealth; 
        }

        public override string ToString()
        {
            return base.ToString() + " | Classe : Clerc | Mana : " + Mana;
        }
        
        public void DoubleHeal(Character cibleOne, Character cibleTwo) {
            CibledSpecial(cibleOne);
            CibledSpecial(cibleTwo);
        }

        public override void AlternatifAtk(Character cible) {
            cible.TakeDamage((int)Math.Round(Mana/2));
            Mana -= 80;
            if (Mana < 0) Mana = 0;
        }
    }

}