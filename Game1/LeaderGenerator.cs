using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	class LeaderGenerator
	{
		private const int MAX_SEED = 100000;

		static private Random _randBase = new Random();
		static private Random _rand = new Random();
		static private int _seed = 0;

		private static int GetBaseInitiative(UnitRace race, LeaderJob job)
		{
			int value = 50;
			switch (job)
			{
				case LeaderJob.Swordsman:
				case LeaderJob.Warrior:
				case LeaderJob.Pikeman:
					value = 50;
					break;
				case LeaderJob.Archer:
					value = 125;
					break;
				case LeaderJob.Healer:
					value = 10;
					break;
				case LeaderJob.Wizard:
					value = 10;
					break;
				case LeaderJob.Support:
					value = 25;
					break;
				case LeaderJob.Knight:
					value = 80;
					break;
				case LeaderJob.WindWalker:
					value = 50;
					break;
				default:
					break;
			}

			switch (race)
			{
				case UnitRace.Human:
					break;
				case UnitRace.Elf:
					value = (int)(value * 1.2f);
					break;
				case UnitRace.Demon:
					value = (int)(value * 0.8f);
					break;
				default:
					break;
			}

			if (value < 0)
				value = 0;
			return value;
		}

		private static int GetBaseSpeed(UnitRace race, LeaderJob job)
		{
			int value = 100;
			switch (job)
			{
				case LeaderJob.Swordsman:
				case LeaderJob.Warrior:
				case LeaderJob.Pikeman:
					value = 90;
					break;
				case LeaderJob.Archer:
					value = 100;
					break;
				case LeaderJob.Healer:
					value = 75;
					break;
				case LeaderJob.Wizard:
					value = 100;
					break;
				case LeaderJob.Support:
					value = 100;
					break;
				case LeaderJob.WindWalker:
					value = 90;
					break;
				case LeaderJob.Knight:
					value = 120;
					break;
				default:
					break;
			}

			switch (race)
			{
				case UnitRace.Human:
					break;
				case UnitRace.Elf:
					value = (int)(value * 1.2f);
					break;
				case UnitRace.Demon:
					value = (int)(value * 0.8f);
					break;
				default:
					break;
			}

			if (value < 0)
				value = 0;
			return value;
		}

		private static int GetBaseRange(UnitRace race, LeaderJob job)
		{
			int value = 1;
			switch (job)
			{
				case LeaderJob.Archer:
					value = 3;
					break;
				case LeaderJob.Knight:
				case LeaderJob.Pikeman:
					value = 2;
					break;
				default:
					break;
			}

			switch (race)
			{
				case UnitRace.Elf:
					value = value >= 3 ? value + 1 : value;
					break;
				default:
					break;
			}

			if (value < 1)
				value = 1;
			else if (value > 5)
				value = 5;

			return value;
		}

		private static int GetUnitId(UnitRace race, LeaderJob job, bool isEnemy)
		{
			int[] idRace = null;
			int[] idJob = null;

			switch (race)
			{
				case UnitRace.Human:
					idRace = new int[] { 1, 2, 3, 4, 5, 7, 8 };
					break;
				case UnitRace.Elf:
					idRace = new int[] { 1001, 1002, 1003, 1004, 1005, 1006, 1007 };
					break;
				case UnitRace.Demon:
					idRace = new int[] { 9001, 9002 };
					break;
				default:
					break;
			}

			switch (job)
			{
				case LeaderJob.Swordsman:
				case LeaderJob.Warrior:
				case LeaderJob.Pikeman:
					idJob = new int[] { 1, 2, 7, 1002, 1007, 9001, 9002 };
					break;
				case LeaderJob.Archer:
					idJob = new int[] { 3, 8, 1001, 1003, 1004 };
					break;
				case LeaderJob.Healer:
					idJob = new int[] { 4, 1005 };
					break;
				case LeaderJob.Wizard:
					idJob = new int[] { 5, 1006 };
					break;
				case LeaderJob.Support:
					idJob = new int[] { 4, 5, 1005, 1006 };
					break;
				case LeaderJob.Knight:
					idJob = new int[] { 2, 7 };
					break;
				case LeaderJob.WindWalker:
					idJob = new int[] { 1002, 1007 };
					break;
				default:
					idJob = new int[] { 1, 1001, 9001 };
					break;
			}

			List<int> results = new List<int>();
			if (idRace != null && idJob != null)
			{
				for (int i = 0; i < idRace.Length; i++)
				{
					for (int j = 0; j < idJob.Length; j++)
					{
						if (idRace[i] == idJob[j])
						{
							results.Add(idRace[i]);
							break;
						}
					}
				}
			}

			return results[_rand.Next(results.Count)];
		}

		private static UnitRace GetRace(bool isEnemy)
		{
			UnitRace race = UnitRace.Human;

			UnitRace[] races = { UnitRace.Human, UnitRace.Elf, UnitRace.Demon };
			int[] p = { 60, 30, 10 };
			if (isEnemy)
				p = new int[] { 40, 10, 50 };

			int dice = _rand.Next(100);
			int value = 0;
			for (int i = 0; i < p.Length; i++)
			{
				if (dice < value + p[i])
				{
					race = races[i];
					break;
				}
				value += p[i];
			}

			return race;
		}

		private static LeaderJob GetJob(UnitRace race, bool isEnemy)
		{
			LeaderJob job = LeaderJob.None;
			LeaderJob[] jobs = null;
			int[] p = null;

			switch (race)
			{
				case UnitRace.Human:
					jobs = new LeaderJob[] { LeaderJob.Swordsman, LeaderJob.Warrior, LeaderJob.Pikeman, LeaderJob.Archer,
						LeaderJob.Healer, LeaderJob.Wizard, LeaderJob.Support, LeaderJob.Knight };
					p = new int[] { 10, 15, 10, 30, 10, 10, 10, 5 };
					break;
				case UnitRace.Elf:
					jobs = new LeaderJob[] { LeaderJob.Swordsman, LeaderJob.Warrior, LeaderJob.Pikeman, LeaderJob.Archer,
						LeaderJob.Healer, LeaderJob.Wizard, LeaderJob.Support, LeaderJob.WindWalker };
					p = new int[] { 10, 5, 5, 30, 15, 20, 10, 5 };
					break;
				case UnitRace.Demon:
					jobs = new LeaderJob[] { LeaderJob.Warrior };
					p = new int[] { 100 };
					break;
				default:
					break;
			}

			int dice = _rand.Next(100);
			int value = 0;
			for (int i = 0; i < p.Length; i++)
			{
				if (dice < value + p[i])
				{
					job = jobs[i];
					break;
				}
				value += p[i];
			}

			return job;
		}

		private static UnitType GetLeaderType (LeaderJob job)
		{
			UnitType type = UnitType.Infantry;

			switch (job)
			{
				case LeaderJob.Swordsman:
				case LeaderJob.Warrior:
				case LeaderJob.Pikeman:
				case LeaderJob.WindWalker:
					type = UnitType.Infantry;
					break;
				case LeaderJob.Archer:
					type = UnitType.Archer;
					break;
				case LeaderJob.Healer:
					type = UnitType.Healer;
					break;
				case LeaderJob.Wizard:
					type = UnitType.Wizard;
					break;
				case LeaderJob.Support:
					type = UnitType.Support;
					break;
				case LeaderJob.Knight:
					type = UnitType.Cavalry;
					break;
				default:
					break;
			}

			return type;
		}

		private static Element GetElement(UnitRace race, bool isEnemy)
		{
			Element element = Element.Light;
			Element[] elements = { Element.Light, Element.Dark,
				Element.Water, Element.Fire, Element.Air, Element.Earth };
			int[] p = null;

			switch (race)
			{
				case UnitRace.Human:
					p = new int[] { 10, 10, 20, 20, 20, 20 };
					break;
				case UnitRace.Elf:
					p = new int[] { 5, 5, 20, 10, 50, 10 };
					break;
				case UnitRace.Demon:
					p = new int[] { 0, 60, 10, 10, 10, 10 };
					break;
				default:
					break;
			}

			int dice = _rand.Next(100);
			int value = 0;
			for (int i = 0; i < p.Length; i++)
			{
				if (dice < value + p[i])
				{
					element = elements[i];
					break;
				}
				value += p[i];
			}

			return element;
		}

		private static List<int> GetSkills(Leader leader)
		{
			List<int> skillLearning = new List<int>();

			// Too many default skill leads to useless leader.
			//List<int> poolStart = new List<int>() { 5016, 5017, 5018, 5019, 5020, 5021 };	// All races.
			List<int> poolStart = new List<int>() {};
			List<int> poolMiddle = new List<int>();
			List<int> poolHigh = new List<int>();
			int[] skills = new int[Leader.MAX_SKILLS];
			int[] levels = new int[Leader.MAX_SKILLS];

			// Add candidates according to the race.
			switch (leader.Race)
			{
				case UnitRace.Human:
					poolStart.AddRange(new int[] { 5003, 5024 });
					poolMiddle.AddRange(new int[] { 5001 });
					//poolHigh.AddRange(new int[] {});
					break;
				case UnitRace.Elf:
					poolStart.AddRange(new int[] { 5009, 5020 });
					poolMiddle.AddRange(new int[] { 5008 });
					//poolHigh.AddRange(new int[] { });
					break;
				case UnitRace.Demon:
					poolStart.AddRange(new int[] { 5017, 5004 });
					poolMiddle.AddRange(new int[] { 5010 });
					//poolHigh.AddRange(new int[] { });
					break;
				default:
					break;
			}

			// Add candidates according to the job.
			switch (leader.Job)
			{
				case LeaderJob.Swordsman:
					poolStart.AddRange(new int[] { 2, 4, 5004, 4003 });
					poolMiddle.AddRange(new int[] { 8, 5006, 3002 });
					poolHigh.AddRange(new int[] { 11, 501 });
					break;
				case LeaderJob.Warrior:
					poolStart.AddRange(new int[] { 1, 5005, 5001, 3004});
					poolMiddle.AddRange(new int[] { 9, 5009, 3005 });
					poolHigh.AddRange(new int[] { 3006, 3018 });
					break;
				case LeaderJob.Pikeman:
					poolStart.AddRange(new int[] { 2, 5022, 5023, 3013 });
					poolMiddle.AddRange(new int[] { 10, 3003, 3006});
					poolHigh.AddRange(new int[] { 3015, 3012 });
					break;
				case LeaderJob.Archer:
					poolStart.AddRange(new int[] { 3, 5, 1001, 3002 });
					poolMiddle.AddRange(new int[] { 6, 1006, 5004 });
					poolHigh.AddRange(new int[] { 12, 502 });
					break;
				case LeaderJob.Healer:
					poolStart.AddRange(new int[] { 2001, 1002, 5009, 3011 });
					poolMiddle.AddRange(new int[] { 2003, 2002, 3012 });
					poolHigh.AddRange(new int[] { 1008, 2004 });
					break;
				case LeaderJob.Wizard:
					poolStart.AddRange(new int[] { 1001, 1004, 1005, 5009 });
					poolMiddle.AddRange(new int[] { 1006, 1007, 3007 });
					poolHigh.AddRange(new int[] { 1010, 1011 });
					break;
				case LeaderJob.Support:
					poolStart.AddRange(new int[] { 1001, 1003, 4002, 4004 });
					poolMiddle.AddRange(new int[] { 2001, 4006, 4008 });
					poolHigh.AddRange(new int[] { 4009, 4010 });
					break;
				case LeaderJob.Knight:
					poolStart.AddRange(new int[] { 2, 5002, 5004, 3001 });
					poolMiddle.AddRange(new int[] { 13, 5006, 3013 });
					poolHigh.AddRange(new int[] { 7, 15 });
					break;
				case LeaderJob.WindWalker:
					poolStart.AddRange(new int[] { 4, 1006, 5003, 4001 });
					poolMiddle.AddRange(new int[] { 14, 5024, 3014 });
					poolHigh.AddRange(new int[] { 503, 1012 });
					break;
				default:
					break;
			}

			// Pick up skills.
			skills[0] = poolStart[_rand.Next(poolStart.Count)];
			poolStart.Remove(skills[0]);
			skills[1] = poolStart[_rand.Next(poolStart.Count)];
			while (skills[0] == skills[1])
			{
				poolStart.Remove(skills[1]);
				skills[1] = poolStart[_rand.Next(poolStart.Count)];
			}
			poolStart.Remove(skills[1]);

			//poolMiddle.AddRange(poolStart);
			skills[2] = poolMiddle[_rand.Next(poolMiddle.Count)];
			while (skills[2] == skills[0] || skills[2] == skills[1])
			{
				poolMiddle.Remove(skills[2]);
				skills[2] = poolMiddle[_rand.Next(poolMiddle.Count)];
			}
			poolMiddle.Remove(skills[2]);
			skills[3] = poolMiddle[_rand.Next(poolMiddle.Count)];
			while (skills[3] == skills[0] || skills[3] == skills[1] || skills[3] == skills[2])
			{
				poolMiddle.Remove(skills[3]);
				skills[3] = poolMiddle[_rand.Next(poolMiddle.Count)];
			}
			poolMiddle.Remove(skills[3]);

			//poolHigh.AddRange(poolMiddle);
			skills[4] = poolHigh[_rand.Next(poolHigh.Count)];
			while (skills[4] == skills[0] || skills[4] == skills[1] || skills[4] == skills[2] || skills[4] == skills[3])
			{
				poolHigh.Remove(skills[4]);
				skills[4] = poolHigh[_rand.Next(poolHigh.Count)];
			}

			// Max skill levels.
			for (int i = 0; i < Leader.MAX_SKILLS; i++)
				levels[i] = 10 - _rand.Next((i + 1) * 2 + 1);

			for (int i = 0; i < Leader.MAX_SKILLS; i++)
			{
				for (int j = 0; j <= 10; j++)
				{
					if (j <= levels[i])
						skillLearning.Add(skills[i]);
					else
						skillLearning.Add(-1);
				}
			}

			return skillLearning;
		}

		private static Leader GenerateLeaderStatus(Leader leader, bool isEnemy)
		{
			const int MIN_ATTRIBUTE = 50;
			const int MAX_ATTRIBUTE = 90;
			const int MIN_GROW = 1;
			const int MAX_GROW = 2;
			const int HP_RATIO = 30;
			//const int MIN_DAMAGE_RATIO = 2;
			//const int MAX_DAMAGE_RATIO = 4;
			const int MIN_DAMAGE_RATIO = 2;
			const int MAX_DAMAGE_RATIO = 4;
			const int MIN_HP_GROW = 30;
			const int MAX_HP_GROW = 60;
			const int MIN_UNIT_SIZE = 10;
			const int MAX_UNIT_SIZE = 20;
			const int MAX_UNIT_GROW = 5;

			leader.HitPoint = MIN_ATTRIBUTE * HP_RATIO + _rand.Next((MAX_ATTRIBUTE - MIN_ATTRIBUTE) * HP_RATIO + 1);
			leader.SkillPoint = MIN_ATTRIBUTE + _rand.Next(MAX_ATTRIBUTE - MIN_ATTRIBUTE + 1);
			leader.Attack = MIN_ATTRIBUTE + _rand.Next(MAX_ATTRIBUTE - MIN_ATTRIBUTE + 1);
			leader.Defence = MIN_ATTRIBUTE + _rand.Next(MAX_ATTRIBUTE - MIN_ATTRIBUTE + 1);
			leader.MinDamage = MIN_ATTRIBUTE * MIN_DAMAGE_RATIO + _rand.Next((MAX_ATTRIBUTE - MIN_ATTRIBUTE) * MIN_DAMAGE_RATIO + 1);
			leader.MaxDamage = MIN_ATTRIBUTE * MAX_DAMAGE_RATIO + _rand.Next((MAX_ATTRIBUTE - MIN_ATTRIBUTE) * MAX_DAMAGE_RATIO + 1);
			leader.MagicAttack = MIN_ATTRIBUTE + _rand.Next(MAX_ATTRIBUTE - MIN_ATTRIBUTE + 1);
			leader.MagicDefence = MIN_ATTRIBUTE + _rand.Next(MAX_ATTRIBUTE - MIN_ATTRIBUTE + 1);

			leader.Initiative = GetBaseInitiative(leader.Race, leader.Job);
			leader.Initiative += (int)(0 - leader.Initiative * 0.2f) + _rand.Next((int)(leader.Initiative * 0.4f + 1));
			leader.Speed = GetBaseSpeed(leader.Race, leader.Job);
			leader.Speed += (int)(0 - leader.Speed * 0.1f) + _rand.Next((int)(leader.Speed * 0.2f + 1));
			leader.Range = GetBaseRange(leader.Race, leader.Job);

			leader.HpGrow = MIN_HP_GROW + _rand.Next(MAX_HP_GROW - MIN_HP_GROW + 1);
			leader.SpGrow = 2;
			leader.AttGrow = 1;
			leader.DefGrow = 1;
			leader.MinDmgGrow = MIN_GROW * MIN_DAMAGE_RATIO + _rand.Next((MAX_GROW - MIN_GROW) * MIN_DAMAGE_RATIO + 1);
			leader.MaxDmgGrow = MIN_GROW * MAX_DAMAGE_RATIO + _rand.Next((MAX_GROW - MIN_GROW) * MAX_DAMAGE_RATIO + 1);
			leader.MattGrow = 1;
			leader.MdefGrow = 1;
			leader.IntGrow = 1;
			leader.SpdGrow = 1;

			leader.UnitSizeBase = MIN_UNIT_SIZE + _rand.Next(MAX_UNIT_SIZE - MIN_UNIT_SIZE + 1);
			//leader.UnitGrow = 1 + _rand.Next(MAX_UNIT_GROW + 1);
			leader.UnitGrow = MAX_UNIT_GROW;

			if (leader.MaxDamage < leader.MinDamage)
				leader.MaxDamage = leader.MinDamage;
			if (leader.MaxDmgGrow < leader.MinDmgGrow)
				leader.MaxDmgGrow = leader.MinDmgGrow;

			//TODO: Skills
			leader.SkillLearning = GetSkills(leader);

			leader.SkillLearningLevel.AddRange(new int[] { 0, 1, 3, 6, 10, 15, 20, 26, 33, 41, 50 });
			leader.SkillLearningLevel.AddRange(new int[] { 0, 3, 7, 12, 18, 25, 32, 40, 49, 59, 70 });
			leader.SkillLearningLevel.AddRange(new int[] { 10, 17, 24, 31, 38, 45, 52, 59, 66, 73, 80 });
			leader.SkillLearningLevel.AddRange(new int[] { 30, 36, 42, 48, 54, 60, 66, 72, 78, 84, 90 });
			leader.SkillLearningLevel.AddRange(new int[] { 60, 64, 68, 72, 76, 80, 84, 88, 92, 96, 100 });

			leader.Skills[0] = leader.SkillLearning[0];
			leader.Skills[1] = leader.SkillLearning[11];
			leader.SkillLevels[0] = leader.SkillLevels[1] = 0;
			for (int i = 2; i < Leader.MAX_SKILLS; i++)
			{
				leader.Skills[i] = -1;
				leader.SkillLevels[i] = -1;
			}

			return leader;
		}

		public static int SetRandomSeed()
		{
			return SetRandomSeed(_randBase.Next(100000 - BattleController.RANDOM_LEADER_ID_BASE)
				+ BattleController.RANDOM_LEADER_ID_BASE);
		}

		public static int SetRandomSeed(int seed)
		{
			_rand = new Random(seed);
			_seed = seed;
			return _seed;
		}

		private static LeaderJob GetJob(Unit unit, bool isEnemy)
		{
			List<LeaderJob> jobs = new List<LeaderJob>();

			if (unit.Rank == 0)
			{
				// Rank 0 can be all jobs.
				jobs.Add(LeaderJob.Swordsman);
				jobs.Add(LeaderJob.Warrior);
				jobs.Add(LeaderJob.Pikeman);
				jobs.Add(LeaderJob.Archer);
				jobs.Add(LeaderJob.Healer);
				jobs.Add(LeaderJob.Wizard);
				jobs.Add(LeaderJob.Support);
				if (unit.Race == UnitRace.Elf)
					jobs.Add(LeaderJob.WindWalker);
				if (unit.Race == UnitRace.Human)
					jobs.Add(LeaderJob.Knight);
			}
			else
			{
				switch (unit.Type)
				{
					case UnitType.Infantry:
						jobs.Add(LeaderJob.Swordsman);
						jobs.Add(LeaderJob.Warrior);
						jobs.Add(LeaderJob.Pikeman);
						if (unit.Race == UnitRace.Elf)
							jobs.Add(LeaderJob.WindWalker);
						if (unit.Race == UnitRace.Human)
							jobs.Add(LeaderJob.Knight);
						break;
					case UnitType.Archer:
						jobs.Add(LeaderJob.Archer);
						break;
					case UnitType.Healer:
						jobs.Add(LeaderJob.Healer);
						jobs.Add(LeaderJob.Support);
						break;
					case UnitType.Wizard:
						jobs.Add(LeaderJob.Wizard);
						jobs.Add(LeaderJob.Support);
						break;
					case UnitType.Cavalry:
						jobs.Add(LeaderJob.Swordsman);
						jobs.Add(LeaderJob.Warrior);
						jobs.Add(LeaderJob.Pikeman);
						if (unit.Race == UnitRace.Human)
							jobs.Add(LeaderJob.Knight);
						break;
					case UnitType.Flyer:
						break;
					case UnitType.Support:
						jobs.Add(LeaderJob.Healer);
						jobs.Add(LeaderJob.Wizard);
						jobs.Add(LeaderJob.Support);
						break;
					default:
						break;
				}
			}

			LeaderJob job = LeaderJob.None;
			if (jobs.Count > 0)
				job = jobs[_rand.Next(jobs.Count)];

			return job;
		}

		private static Leader GenerateLeader(bool isEnemy, UnitRace race = UnitRace.None, LeaderJob job = LeaderJob.None)
		{
			Leader leader = new Leader();

			leader.Id = _seed;
			leader.Race = race != UnitRace.None ? race : GetRace(isEnemy);
			leader.Job = job != LeaderJob.None ? job : GetJob(leader.Race, isEnemy);
			leader.Type = GetLeaderType(leader.Job);
			leader.Element = GetElement(leader.Race, isEnemy);
			leader.Name = leader.Job.ToString() + _seed;

			leader = GenerateLeaderStatus(leader, isEnemy);

			return leader;
		}

		public static Leader GenerateLeader(Unit unit, bool isEnemy)
		{
			SetRandomSeed();
			LeaderJob job = GetJob(unit, isEnemy);
			Leader leader = GenerateLeader(isEnemy, unit.Race, job);
			leader.UnitGrow += (1 - unit.Rank);

			return leader;
		}

		public static Leader Generate(bool isEnemy, UnitRace race = UnitRace.None, LeaderJob job = LeaderJob.None)
		{
			SetRandomSeed();
			Leader leader = GenerateLeader(isEnemy, race, job);

			int unitId = GetUnitId(leader.Race, leader.Job, isEnemy);
			Unit unit = Unit.LoadWithId(unitId);

			leader.UnitId[0] = unitId;
			leader.UnitGrow += (1 - unit.Rank);
			return leader;
		}
	}
}
