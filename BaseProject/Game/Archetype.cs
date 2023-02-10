using System;
namespace CSharpDiscovery.Examen
{
    public interface IHealer
    {
        public int HealPower {get; set;}
        public void DoubleHeal(Character cibleOne, Character cibleTwo);
        public int GetHeal()
        {
            return HealPower;
        }
    }

    public interface ITank
    {
        public int AttackPower {get; set;}
        public void DoubleHit(Character cible);
        public int GetPower()
        {
            return AttackPower;
        }
    }

}