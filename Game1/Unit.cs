using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Game1
{
	public enum UnitRace
	{
		None,
		Human,
		Elf,
		Demon
	}

	public enum UnitType
	{
		None,
		Infantry,
		Archer,
		Healer,
		Wizard,
		Cavalry,
		Flyer,
		Support,
	}

	public enum Element
	{
		None,
		Light,
		Dark,
		Water,
		Fire,
		Air,
		Earth,
	}

	public enum AiType
	{
		Default
	}

	public class Unit
	{
		//public const int MAX_VALUE = 999;
		public const int MAX_SKILLS = 3;
		public const int RANGER_RANGE = 3;

		private const string FILE_PATH = "Xml\\Unit\\";
		private const string FILE_FORMAT = "Unit{0:D4}.xml";
		private const string DEFAULT_FILE = "UnitDefault.xml";

		// Basic
		public int Id;
		public string Name;
		public UnitRace Race;
		public UnitType Type;
		public int Rank;
		public Element Element;
		public Bitmap Avatar;
		public AiType Ai;

		public int Level;
		public int Experience;

		// Ability
		public int HitPoint;
		public int SkillPoint;
		public int Attack;
		public int Defence;
		public int MinDamage;
		public int MaxDamage;
		public int MagicAttack;
		public int MagicDefence;
		public int Initiative;
		public int Speed;
		public int Range;

		public int HpGrow;
		public int SpGrow;
		public int AttGrow;
		public int DefGrow;
		public int MinDmgGrow;
		public int MaxDmgGrow;
		public int MattGrow;
		public int MdefGrow;
		public int IntGrow;
		public int SpdGrow;

		// Skill
		public int[] Skills = new int[MAX_SKILLS];
		public int[] SkillLevels = new int[MAX_SKILLS];
		//public Dictionary<int, int> Skills;		// XmlSerializer cannot apply to Dictionary directly.
		public List<int> SkillLearning = new List<int>();
		public List<int> SkillLearningLevel = new List<int>();

		public Unit()
		{
			Id = 0;
			Name = "Default Unit";
			Race = UnitRace.Human;
			Type = UnitType.Infantry;
			Rank = 0;
			Element = Element.Light;
			Avatar = null;
			Ai = AiType.Default;

			Level = 0;
			Experience = 0;

			HitPoint = 500;
			SkillPoint = 50;
			Attack = 50;
			Defence = 50;
			MinDamage = 50;
			MaxDamage = 100;
			MagicAttack = 50;
			MagicDefence = 50;
			Initiative = 50;
			Speed = 50;
			Range = 1;

			HpGrow = 25;
			SpGrow = 5;
			AttGrow = 1;
			DefGrow = 1;
			MinDmgGrow = 1;
			MaxDmgGrow = 2;
			MattGrow = 1;
			MdefGrow = 1;
			IntGrow = 1;
			SpdGrow = 1;

			for (int i = 0; i < MAX_SKILLS; i++)
			{
				Skills[i] = -1;
				SkillLevels[i] = -1;
			}

			//LoadFromFile(DEFAULT_FILE);
		}

		public static Unit LoadWithId(int unitId)
		{
			string file = DEFAULT_FILE;
			if (unitId > 0)
				file = string.Format(FILE_FORMAT, unitId);
			//if (unitId > 0)
			//	file = string.Format("Unit{0:D4}.xml", unitId);

			Unit temp = null;
			file = FILE_PATH + file;
			using (var sr = new StreamReader(file))
			{
				XmlSerializer xs = new XmlSerializer(typeof(Unit));
				temp = (Unit)xs.Deserialize(sr);

				//this.Id = temp.Id;
				//this.Name = temp.Name;
				//this.Race = temp.Race;
				//this.Type = temp.Type;
				//this.Rank = temp.Rank;
				//this.Element = temp.Element;
				//this.Avatar = temp.Avatar;
				//this.Ai = temp.Ai;

				//this.HitPoint = temp.HitPoint;
				//this.SkillPoint = temp.SkillPoint;
				//this.Attack = temp.Attack;
				//this.Defence = temp.Defence;
				//this.MinDamage = temp.MinDamage;
				//this.MaxDamage = temp.MaxDamage;
				//this.MagicAttack = temp.MagicAttack;
				//this.MagicDefence = temp.MagicDefence;
				//this.Initiative = temp.Initiative;
				//this.Speed = temp.Speed;
				//this.Range = temp.Range;

				//this.Skills = temp.Skills;
				//this.UpgradeSkills = temp.UpgradeSkills;
			}

			//temp.Skills = new Dictionary<int, int>();
			//for (int i = 0; i < MAX_SKILLS; i++)
			//{
			//	temp.Skills[i] = -1;
			//	temp.SkillLevels[i] = -1;
			//}

			//for (int i = 0; i < temp.SkillLearningLevel.Count; i++)
			//{
			//	if (temp.SkillLearningLevel[i] == 0)
			//	{
			//		int skillId = temp.SkillLearning[i];
			//		int j = 0;
			//		int slot = -1;
			//		for (; j < MAX_SKILLS; j++)
			//		{
			//			if (temp.Skills[j] == -1 && slot == -1)
			//				slot = j;
			//			if (temp.Skills[j] == skillId)
			//			{
			//				temp.SkillLevels[j]++;
			//				break;
			//			}
			//		}
			//		if (j == MAX_SKILLS)
			//		{
			//			temp.Skills[slot] = skillId;
			//			temp.SkillLevels[slot] = 0;
			//		}
			//	}
			//	else
			//		break;
			//}

			return temp;
		}

		public int LevelUp(int level)
		{
			int startLv = Level;
			Level += level;

			HitPoint += level * HpGrow;
			SkillPoint += level * SpGrow;
			Attack += level * AttGrow;
			Defence += level * DefGrow;
			MinDamage += level * MinDmgGrow;
			MaxDamage += level * MaxDmgGrow;
			MagicAttack += level * MattGrow;
			MagicDefence += level * MdefGrow;
			Initiative += level * IntGrow;
			Speed += level * SpdGrow;

			if (MaxDamage < MinDamage)
				MaxDamage = MinDamage;

			//TODO: Check skills.
			if (SkillLearning != null)
			{
				for (int i = 0; i < SkillLearning.Count; i++)
				{
					//if (SkillLearningLevel[i] > Level)
					//	break;
					if(SkillLearningLevel[i] > startLv && SkillLearningLevel[i] <= Level && SkillLearning[i] != -1)
					{
						int j = 0;
						for (; j < Skills.Length; j++)
							if (Skills[j] == -1 || Skills[j] == SkillLearning[i])
								break;
						Skills[j] = SkillLearning[i];
						SkillLevels[j]++;
					} 
				}
			}

			return Level;
		}

		virtual public void SaveToFile(string fileName)
		{
			fileName = FILE_PATH + fileName;
			XmlSerializer xs = new XmlSerializer(typeof(Unit));
			TextWriter tw = new StreamWriter(fileName);
			xs.Serialize(tw, this);
		}
	}
}
