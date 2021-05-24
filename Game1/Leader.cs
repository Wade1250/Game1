using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Game1
{
	public enum LeaderJob
	{
		None,
		Swordsman,
		Warrior,
		Pikeman,
		Archer,
		Healer,
		Wizard,
		Support,

		Knight,
		WindWalker,
		Boss,
	}

	public class Leader : Unit
	{
		public const int MAX_UNITS = 1;
		public new const int MAX_SKILLS = 5;

		private const string FILE_PATH = "Xml\\Leader\\";
		private const string FILE_FORMAT = "Leader{0:D4}.xml";
		private const string DEFAULT_FILE = "LeaderDefault.xml";

		public LeaderJob Job;   // Use LeaderJob instead of UnitType unless LeaderJob is none.
		public int LeaderSkill;
		public int SubSkill;
		public int UnitSizeBase;
		public int UnitGrow;

		public int[] UnitId = new int[MAX_UNITS];
		public int[] UnitSize = new int[MAX_UNITS];		
		public int[] UnitNumber = new int[MAX_UNITS];
		public int[] UnitLevel = new int[MAX_UNITS];
		public int[] UnitExp = new int[MAX_UNITS];

		public Leader()
		{
			Id = 0;
			Name = "Default Leader";
			Race = UnitRace.Human;
			Type = UnitType.Infantry;
			Element = Element.Light;
			Rank = 0;
			Avatar = null;
			Ai = AiType.Default;

			Job = LeaderJob.None;
			Level = 0;
			Experience = 0;
			LeaderSkill = -1;
			SubSkill = -1;
			SkillLearning = new List<int>();
			SkillLearningLevel = new List<int>();

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

			UnitSizeBase = 0;
			UnitGrow = 10;
			Skills = new int[MAX_SKILLS];
			SkillLevels = new int[MAX_SKILLS];
			for (int i = 0; i < MAX_SKILLS; i++)
			{
				Skills[i] = -1;
				SkillLevels[i] = -1;
			}

			UnitId[0] = 0;
			UnitSize[0] = 100;
			UnitNumber[0] = 100;
			UnitLevel[0] = 0;
			UnitExp[0] = 0;
			for (int i = 1; i < MAX_UNITS; i++)
			{
				UnitId[i] = -1;
				UnitSize[i] = 0;
				UnitNumber[i] = 0;
				UnitLevel[i] = 0;
				UnitExp[i] = 0;
			}
		}

		public new int LevelUp(int level)
		{
			UnitSizeBase += level * UnitGrow;
			return base.LevelUp(level);
		}

		public new static Leader LoadWithId(int leaderId)
		{
			string file = DEFAULT_FILE;
			if (leaderId > 0)
				file = string.Format(FILE_FORMAT, leaderId);

			Leader temp = null;
			file = FILE_PATH + file;
			using (var sr = new StreamReader(file))
			{
				XmlSerializer xs = new XmlSerializer(typeof(Leader));
				temp = (Leader)xs.Deserialize(sr);
				//this.Id = temp.Id;
				//this.Name = temp.Name;
				//this.Race = temp.Race;
				//this.Type = temp.Type;
				//this.Element = temp.Element;
				//this.Rank = temp.Rank;
				//this.Avatar = temp.Avatar;

				//this.Job = temp.Job;
				//this.Level = temp.Level;
				//this.Experience = temp.Experience;
				//this.LeaderSkill = temp.LeaderSkill;
				//this.SubSkill = temp.SubSkill;
				//this.SkillLearning = null;
				//this.SkillLearningLevel = null;

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

				//this.HpGrow = temp.HpGrow;
				//this.SpGrow = temp.SpGrow;
				//this.UnitGrow = temp.UnitGrow;
				//this.AttRate = temp.AttRate;
				//this.DefRate = temp.DefRate;
				//this.MinDmgRate = temp.MinDmgRate;
				//this.MaxDmgRate = temp.MaxDmgRate;
				//this.MattRate = temp.MattRate;
				//this.MdefRate = temp.MdefRate;
				//this.IntRate = temp.IntRate;
				//this.SpdRate = temp.SpdRate;

				//this.Skills = temp.Skills;
				//this.UpgradeSkills = temp.UpgradeSkills;

				//this.UnitId = temp.UnitId;
				//this.UnitSize = temp.UnitSize;
				//this.UnitNumber = temp.UnitNumber;
				//this.UnitLevel = temp.UnitLevel;
				//this.UnitExp = temp.UnitExp;
			}

			return temp;
		}

		public override void SaveToFile(string fileName)
		{
			fileName = FILE_PATH + fileName;
			XmlSerializer xs = new XmlSerializer(typeof(Leader));
			TextWriter tw = new StreamWriter(fileName);
			xs.Serialize(tw, this);
		}

	}
}
