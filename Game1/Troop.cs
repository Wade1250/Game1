using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public class Troop
	{
		public Leader Leader;
		public Unit Unit;
		public int LeaderHp;
		public int UnitHp;
		public int CurrentNumber;
		public int TroopNumber;
		public int MaxNumber;
		public List<Buffer> Buffers;
		public int Position;
		public int LeaderSp;
		public int UnitSp;

		public Troop()
		{
			Leader = null;
			Unit = null;
			Buffers = new List<Buffer>();
		}

		public bool IsAlive { get { return LeaderHp > 0 || CurrentNumber > 0; } }
		public string Name
		{
			get
			{
				if (Leader != null)
					return Leader.Name;
				if (Unit != null)
					return Unit.Name;
				else
					return null;
			}
		}

		public int CurrentSp
		{
			get
			{
				int sp = 0;
				if (Leader != null && LeaderHp > 0)
					sp += LeaderSp;
				if (Unit != null && CurrentNumber > 0)
					sp += UnitSp;
				return sp;
			}
		}

		public void AddSp(int value)
		{
			if (value >= 0)
			{
				int sp = value;
				if (Leader != null && LeaderHp > 0)
				{
					LeaderSp += sp;
					if (LeaderSp > Leader.SkillPoint)
					{
						sp = LeaderSp - Leader.SkillPoint;
						LeaderSp = Leader.SkillPoint;
					}
					else
						sp = 0;
				}
				if (Unit != null && CurrentNumber > 0 && sp > 0)
				{
					UnitSp += sp;
					if (UnitSp > Unit.SkillPoint)
						UnitSp = Unit.SkillPoint;
				}
			}
			else
			{
				int sp = value;
				if (Unit != null && CurrentNumber > 0)
				{
					UnitSp += sp;
					if (UnitSp < 0)
					{
						sp = UnitSp;
						UnitSp = 0;
					}
					else sp = 0;
				}
				if (Leader != null && LeaderHp > 0 && sp < 0)
				{
					LeaderSp += sp;
					if (LeaderSp < 0)
						LeaderSp = 0;
				}
			}
		}

		public int Initiative
		{
			get
			{
				// Faster
				int ini = 0;
				if (Leader != null && LeaderHp > 0)
					ini = Leader.Initiative;
				if (Unit != null && CurrentNumber > 0)
					ini = Math.Max(ini, Unit.Initiative);
				return ini;
			}
		}
		public int Speed
		{
			get
			{
				// Slower
				int spd = 0;
				if (Leader != null && LeaderHp > 0)
					spd = Leader.Speed;
				if (Unit != null && CurrentNumber > 0)
				{
					if (spd == 0)
						spd = Unit.Speed;
					else
						spd = Math.Min(spd, Unit.Speed);
				}
				return spd;
			}
		}
		public int Range { get { return (Unit != null && CurrentNumber > 0) ? Unit.Range : (Leader != null ? Leader.Range : 0); } }

		public int MaxSp { get { return ((Leader != null) ? Leader.SkillPoint : 0) + ((Unit != null) ? Unit.SkillPoint : 0); } }

		public Dictionary<int, int> Skills
		{
			get
			{
				Dictionary<int, int> skills = new Dictionary<int, int>();

				if (Leader != null && LeaderHp > 0)
				{
					for (int i = 0; i < Leader.MAX_SKILLS; i++)
					{
						if (Leader.Skills[i] != -1)
							skills.Add(Leader.Skills[i], Leader.SkillLevels[i]);
					}
				}
				if (Unit != null && CurrentNumber > 0)
				{
					for (int i = 0; i < Unit.MAX_SKILLS; i++)
					{
						if (Unit.Skills[i] != -1)
						{
							if (skills.ContainsKey(Unit.Skills[i]))
								skills[Unit.Skills[i]] += Unit.SkillLevels[i] + 1;
							else
								skills.Add(Unit.Skills[i], Unit.SkillLevels[i]);
						}
					}
				}

				return skills;
			}
		}

		public override string ToString()
		{
			string str = "";

			if (Leader != null)
			{
				str += Leader.Name + "\r\n";
				str += Leader.Race + " " + Leader.Job + "(" + Leader.Type + ") ";
				str += "Lv" + Leader.Level + "\r\n";
				str += "屬性: " + Leader.Element + "\r\n";
				str += "HP: " + LeaderHp + "/" + Leader.HitPoint + " [+" + Leader.HpGrow + "]\r\n";
				str += "SP: " + LeaderSp + "/" + Leader.SkillPoint + " [+" + Leader.SpGrow + "]\r\n";
				str += "攻擊 " + Leader.Attack + " [+" + Leader.AttGrow + "]";
				if (Modifier.CheckAttackModifier(this, true, null, false, null) != 0)
					str += " => " + (Leader.Attack + Modifier.CheckAttackModifier(this, true, null, false, null));
				str += "\r\n";
				str += "防禦 " + Leader.Defence + " [+" + Leader.DefGrow + "]";
				if (Modifier.CheckDefenceModifier(null, false, this, false, null) != 0)
					str += " => " + (Leader.Defence + Modifier.CheckDefenceModifier(null, false, this, false, null));
				str += "\r\n";
				str += "傷害 " + Leader.MinDamage + "-" + Leader.MaxDamage +
					" [+" + Leader.MinDmgGrow + "-" + Leader.MaxDmgGrow + "]";
				if (Modifier.CheckDamageModifier(this, true, null, false, null) != 0)
					str += " => " +	(Leader.MinDamage + Modifier.CheckDamageModifier(this, true, null, false, null)) + "-" +
					(Leader.MaxDamage + Modifier.CheckDamageModifier(this, true, null, false, null));
				str += "\r\n";
				str += "魔攻 " + Leader.MagicAttack + " [+" + Leader.MattGrow + "]";
				if (Modifier.CheckMagicAttackModifier(this, true, null, false, null) != 0)
					str += " => " + (Leader.MagicAttack + Modifier.CheckMagicAttackModifier(this, true, null, false, null));
				str += "\r\n";
				str += "魔防 " + Leader.MagicDefence + " [+" + Leader.MdefGrow + "]";
				if (Modifier.CheckMagicDefenceModifier(null, false, this, false, null) != 0)
					str += " => " + (Leader.MagicDefence + Modifier.CheckMagicDefenceModifier(null, false, this, false, null));
				str += "\r\n";
				str += "先攻 " + Leader.Initiative + " [+" + Leader.IntGrow + "]\r\n";
				str += "速度 " + Leader.Speed + " [+" + Leader.SpdGrow + "]";
				if (Modifier.CheckSpeedModifier(this) != 0)
					str += " => " + (Leader.Speed + Modifier.CheckSpeedModifier(this));
				str += "\r\n";
				str += "攻擊距離=" + Leader.Range + "\r\n";
				str += "帶兵量 " + Leader.UnitSizeBase + " [+" + Leader.UnitGrow + "]\r\n";
				str += "\r\n[技能]\r\n";

				for (int i = 0; i < Leader.Skills.Length; i++)
				{
					if (Leader.Skills[i] != -1)
						str += Skill.Table[Leader.Skills[i]].Name + "Lv" + Leader.SkillLevels[i] + "\r\n";
				}

				List<int> skillIds = new List<int>();
				Dictionary<int, int> nextLevel = new Dictionary<int, int>();
				Dictionary<int, int> maxLevel = new Dictionary<int, int>();
				for (int i = 0; i < Leader.SkillLearning.Count; i++)
				{
					if (Leader.SkillLearning[i] != -1)
					{
						if (skillIds.Contains(Leader.SkillLearning[i]))
							maxLevel[Leader.SkillLearning[i]]++;
						else
						{
							skillIds.Add(Leader.SkillLearning[i]);
							maxLevel.Add(Leader.SkillLearning[i], 0);
						}

						if (Leader.SkillLearningLevel[i] > Leader.Level && !nextLevel.ContainsKey(Leader.SkillLearning[i]))
							nextLevel.Add(Leader.SkillLearning[i], Leader.SkillLearningLevel[i]);
					}
				}
				for (int i = 0; i < skillIds.Count; i++)
				{
					if (nextLevel.ContainsKey(skillIds[i]))
						str += "(需求等級Lv" + nextLevel[skillIds[i]] + ") " + Skill.Table[skillIds[i]].Name +
							" [Max: Lv" + maxLevel[skillIds[i]] + "]\r\n";
				}
			}
			else
				str += "無將領\r\n";
			str += "\r\n===\r\n\r\n";

			if (Unit != null)
			{
				str += Unit.Name + " ";
				str += CurrentNumber + "/" + TroopNumber + "/" + MaxNumber + "\r\n";
				str += "Rank " + Unit.Rank + "\r\n";
				str += Unit.Race + " " + Unit.Type + " ";
				str += "Lv" + Unit.Level + "\r\n";
				str += "屬性: " + Unit.Element + "\r\n";
				str += "HP: " + UnitHp + "/" + Unit.HitPoint + " [+" + Unit.HpGrow + "]\r\n";
				str += "SP: " + UnitSp + "/" + Unit.SkillPoint + " [+" + Unit.SpGrow + "]\r\n";
				str += "攻擊 " + Unit.Attack + " [+" + Unit.AttGrow + "]";
				if (Modifier.CheckAttackModifier(this, false, null, false, null) != 0)
					str += " => " + (Unit.Attack + Modifier.CheckAttackModifier(this, false, null, false, null));
				str += "\r\n";
				str += "防禦 " + Unit.Defence + " [+" + Unit.DefGrow + "]";
				if (Modifier.CheckDefenceModifier(null, false, this, false, null) != 0)
					str += " => " + (Unit.Defence + Modifier.CheckDefenceModifier(null, false, this, false, null));
				str += "\r\n";
				str += "傷害 " + Unit.MinDamage + "-" + Unit.MaxDamage + " [+" + Unit.MinDmgGrow + "-" + Unit.MaxDmgGrow + "]";
				if (Modifier.CheckDamageModifier(this, false, null, false, null) != 0)
					str += " => " + (Unit.MinDamage + Modifier.CheckDamageModifier(this, false, null, false, null)) + "-" +
					(Unit.MaxDamage + Modifier.CheckDamageModifier(this, false, null, false, null));
				str += "\r\n";
				str += "魔攻 " + Unit.MagicAttack + " [+" + Unit.MattGrow + "]";
				if (Modifier.CheckMagicAttackModifier(this, false, null, false, null) != 0)
					str += " => " + (Unit.MagicAttack + Modifier.CheckMagicAttackModifier(this, false, null, false, null));
				str += "\r\n";
				str += "魔防 " + Unit.MagicDefence + " [+" + Unit.MdefGrow + "]";
				if (Modifier.CheckMagicDefenceModifier(null, false, this, false, null) != 0)
					str += " => " + (Unit.MagicDefence + Modifier.CheckMagicDefenceModifier(null, false, this, false, null));
				str += "\r\n";
				str += "先攻 " + Unit.Initiative + " [+" + Unit.IntGrow + "]\r\n";
				str += "速度 " + Unit.Speed + " [+" + Unit.SpdGrow + "]";
				if (Modifier.CheckSpeedModifier(this) != 0)
					str += " => " + (Unit.Speed + Modifier.CheckSpeedModifier(this));
				str += "\r\n";
				str += "攻擊距離=" + Unit.Range + "\r\n";
				str += "\r\n[技能]\r\n";

				for (int i = 0; i < Unit.Skills.Length; i++)
				{
					if (Unit.Skills[i] != -1)
						str += Skill.Table[Unit.Skills[i]].Name + "Lv" + Unit.SkillLevels[i] + "\r\n";
				}
				for (int i = 0; i < Unit.SkillLearning.Count; i++)
				{
					if (Unit.SkillLearningLevel[i] > Unit.Level)
						str += "(需求等級Lv" + Unit.SkillLearningLevel[i] + ") " + Skill.Table[Unit.SkillLearning[i]].Name + "\r\n";
				}
			}
			else
				str += "無部隊\r\n";
			str += "\r\n===\r\n\r\n";

			str += "SP: " + CurrentSp + "/" + MaxSp + "\r\n\r\n";

			str += "\r\n[技能]\r\n";
			foreach (var pair in Skills)
			{
				int power = Skill.Table[pair.Key].DamageBonus + Skill.Table[pair.Key].DamageGrow * pair.Value;
				if (Skill.Table[pair.Key].SkillType == SkillType.Buffer || Skill.Table[pair.Key].SkillType == SkillType.Curse ||
					Skill.Table[pair.Key].SkillType == SkillType.Passive || Skill.Table[pair.Key].SkillType == SkillType.StartUp)
					power = Skill.Table[pair.Key].Buffer.Power + Skill.Table[pair.Key].PowerGrow * pair.Value;
				str += "(" + Skill.Table[pair.Key].SkillType + ") " + Skill.Table[pair.Key].Name + "Lv" + pair.Value + "\r\n";
				str += "  " + Skill.Table[pair.Key].Description + " " + power + "\r\n\r\n";
			}

			str += "\r\n[Buffer]\r\n";
			foreach (Buffer buf in Buffers)
			{
				if (buf.SkillId <= 0)
					str += "防禦";
				else
					str += Skill.Table[buf.SkillId].Name;
				str += ": "+ buf.Type.ToString() + " " + buf.Power + " (" + buf.Duration + ")\r\n";
			}
			str += "\r\n";

			return str;
		}
	}
}
