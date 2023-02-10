using System;
namespace GameNetClient
{
    public abstract class Character 
    {
        public string Name = "";
        public float Health;
        public float MaxHealth;
        public DateTime CreationDate;

        public Character() {
            Name = "NPC";
            Health = 100;
            MaxHealth = 100;
        }
        public Character(string _Name, float _MaxHealth)
        {
            this.Name = _Name;
            this.MaxHealth = _MaxHealth;
            this.Health = _MaxHealth;
        }
        public void TakeDamage(int damage) {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public string GetCreationDate() {
            return CreationDate.ToString("dd/MM h/mm");
        }
        public override string ToString()
        {
            return Name + " : " + Health + "/" + MaxHealth;
        }

        public abstract void Special();

        public abstract void CibledSpecial(Character cible);

        public abstract void AlternatifAtk(Character cible);

        public abstract void SetUniqueValue(byte newValue);
    }
}