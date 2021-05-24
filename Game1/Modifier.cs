using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	class Modifier
	{
		#region Modifiers
		public static int ElementModifier(Element attacker, Element defender)
		{
			// Dark>WFAE>Light: +25		-10
			// W>F>A>E>W:		+50		-20
			// Light>Dark:		+100	-40
			//const int BONUS_BIG = 100;
			//const int BONUS_MIDDLE = 50;
			//const int BONUS_SMALL = 25;
			//const int PENALTY_BIG = -40;
			//const int PENALTY_MIDDLE = -20;
			//const int PENALTY_SMALL = -10;
			const int BONUS_BIG = 60;
			const int BONUS_MIDDLE = 35;
			const int BONUS_SMALL = 15;
			const int PENALTY_BIG = -20;
			const int PENALTY_MIDDLE = -10;
			const int PENALTY_SMALL = -5;

			int bonus = 0;

			switch (attacker)
			{
				case Element.Light:
					if (defender == Element.Dark)
						bonus = BONUS_BIG;
					else if (defender == Element.Light)
						bonus = 0;
					else
						bonus = PENALTY_SMALL;
					break;
				case Element.Dark:
					if (defender == Element.Dark)
						bonus = 0;
					else if (defender == Element.Light)
						bonus = PENALTY_BIG;
					else
						bonus = BONUS_SMALL;
					break;
				case Element.Water:
					if (defender == Element.Dark)
						bonus = PENALTY_SMALL;
					else if (defender == Element.Light)
						bonus = BONUS_SMALL;
					else if (defender == Element.Fire)
						bonus = BONUS_MIDDLE;
					else if (defender == Element.Earth)
						bonus = PENALTY_MIDDLE;
					else
						bonus = 0;
					break;
				case Element.Fire:
					if (defender == Element.Dark)
						bonus = PENALTY_SMALL;
					else if (defender == Element.Light)
						bonus = BONUS_SMALL;
					else if (defender == Element.Air)
						bonus = BONUS_MIDDLE;
					else if (defender == Element.Water)
						bonus = PENALTY_MIDDLE;
					else
						bonus = 0;
					break;
				case Element.Air:
					if (defender == Element.Dark)
						bonus = PENALTY_SMALL;
					else if (defender == Element.Light)
						bonus = BONUS_SMALL;
					else if (defender == Element.Earth)
						bonus = BONUS_MIDDLE;
					else if (defender == Element.Fire)
						bonus = PENALTY_MIDDLE;
					else
						bonus = 0;
					break;
				case Element.Earth:
					if (defender == Element.Dark)
						bonus = PENALTY_SMALL;
					else if (defender == Element.Light)
						bonus = BONUS_SMALL;
					else if (defender == Element.Water)
						bonus = BONUS_MIDDLE;
					else if (defender == Element.Air)
						bonus = PENALTY_MIDDLE;
					else
						bonus = 0;
					break;
				default:
					break;
			}

			return bonus;
		}

		public static int CheckAttackModifier(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			const int COUNTER_PANALTY = -50;
			return (isCounter ? COUNTER_PANALTY : 0) + CheckAttackPassive(attacker, isAttackerLeader, defender, isCounter, skill) +
				CheckAttackBuffer(attacker, isAttackerLeader, defender, isCounter, skill);
		}

		public static int CheckDefenceModifier(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			return CheckDefencePassive(attacker, isAttackerLeader, defender, isCounter, skill) +
				CheckDefenceBuffer(attacker, isAttackerLeader, defender, isCounter, skill);
		}

		public static int CheckDamageModifier(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			return CheckDamagePassive(attacker, isAttackerLeader, defender, isCounter, skill) +
				CheckDamageBuffer(attacker, isAttackerLeader, defender, isCounter, skill);
		}

		public static int CheckMagicAttackModifier(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			return CheckMagicAttackPassive(attacker, isAttackerLeader, defender, isCounter, skill) +
				CheckMagicAttackBuffer(attacker, isAttackerLeader, defender, isCounter, skill);
		}

		public static int CheckMagicDefenceModifier(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			return CheckMagicDefencePassive(attacker, isAttackerLeader, defender, isCounter, skill) +
				CheckMagicDefenceBuffer(attacker, isAttackerLeader, defender, isCounter, skill);
		}

		public static int CheckSpeedModifier(Troop troop)
		{
			return CheckSpeedPassive(troop) + CheckSpeedBuffer(troop);
		}

		// Other class may use this to check sp available.
		public static int CheckSpModifier(Troop attacker, Troop defender, Skill skill)
		{
			return CheckSpPassive(attacker, defender, skill) + CheckSpBuffer(attacker, defender, skill);
		}

		#endregion

		#region Buffers
		private static int CheckAttackBuffer(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (Buffer buf in attacker.Buffers)
			{
				if (buf.Duration > 0)
				{
					Element element = (defender == null ? Element.None : (defender.CurrentNumber > 0 ? defender.Unit.Element : defender.Leader.Element));
					switch (buf.Type)
					{
						case BufferType.AttackUp:
						case BufferType.AttackAndDefenceUp:
							value += buf.Power;
							break;
						case BufferType.AttackDown:
							value -= buf.Power;
							break;
						case BufferType.LightAttackUp:
							if (element == Element.Light)
								value += buf.Power;
							break;
						case BufferType.DarkAttackUp:
							if (element == Element.Dark)
								value += buf.Power;
							break;
						case BufferType.WaterAttackUp:
							if (element == Element.Water)
								value += buf.Power;
							break;
						case BufferType.FireAttackUp:
							if (element == Element.Fire)
								value += buf.Power;
							break;
						case BufferType.AirAttackUp:
							if (element == Element.Air)
								value += buf.Power;
							break;
						case BufferType.EarthAttackUp:
							if (element == Element.Earth)
								value += buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckDefenceBuffer(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (Buffer buf in defender.Buffers)
			{
				if (buf.Duration > 0)
				{
					Element attackElement = BattleController.CheckElement(attacker, isAttackerLeader, skill);
					switch (buf.Type)
					{
						case BufferType.DefenceUp:
						case BufferType.AttackAndDefenceUp:
							value += buf.Power;
							break;
						case BufferType.DefenceDown:
							value -= buf.Power;
							break;
						case BufferType.RangeDefenceUp:
							bool isRange = skill == null ? (attacker == null ? false : attacker.Range > Unit.RANGER_RANGE) : !skill.IsMelee;
							if (isRange)
								value += buf.Power;
							break;
						case BufferType.LightDefenceUp:
							if (attackElement == Element.Light)
								value += buf.Power;
							break;
						case BufferType.DarkDefenceUp:
							if (attackElement == Element.Dark)
								value += buf.Power;
							break;
						case BufferType.WaterDefenceUp:
							if (attackElement == Element.Water)
								value += buf.Power;
							break;
						case BufferType.FireDefenceUp:
							if (attackElement == Element.Fire)
								value += buf.Power;
							break;
						case BufferType.AirDefenceUp:
							if (attackElement == Element.Air)
								value += buf.Power;
							break;
						case BufferType.EarthDefenceUp:
							if (attackElement == Element.Earth)
								value += buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckDamageBuffer(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (Buffer buf in attacker.Buffers)
			{
				if (buf.Duration > 0)
				{
					switch (buf.Type)
					{
						case BufferType.DamageUp:
							value += buf.Power;
							break;
						case BufferType.DamageDown:
							value -= buf.Power;
							break;
						case BufferType.PhysicSkillDamageUp:
							if (skill != null && skill.SkillType == SkillType.Physic)
								value += buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckMagicAttackBuffer(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (Buffer buf in attacker.Buffers)
			{
				if (buf.Duration > 0)
				{
					Element element = (defender == null ? Element.None : (defender.CurrentNumber > 0 ? defender.Unit.Element : defender.Leader.Element));
					switch (buf.Type)
					{
						case BufferType.MagicAttackUp:
						case BufferType.MagicAttackAndDefenceUp:
							value += buf.Power;
							break;
						case BufferType.MagicAttackDown:
							value -= buf.Power;
							break;
						case BufferType.LightAttackUp:
							if (element == Element.Light)
								value += buf.Power;
							break;
						case BufferType.DarkAttackUp:
							if (element == Element.Dark)
								value += buf.Power;
							break;
						case BufferType.WaterAttackUp:
							if (element == Element.Water)
								value += buf.Power;
							break;
						case BufferType.FireAttackUp:
							if (element == Element.Fire)
								value += buf.Power;
							break;
						case BufferType.AirAttackUp:
							if (element == Element.Air)
								value += buf.Power;
							break;
						case BufferType.EarthAttackUp:
							if (element == Element.Earth)
								value += buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckMagicDefenceBuffer(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (Buffer buf in defender.Buffers)
			{
				if (buf.Duration > 0)
				{
					Element attackElement = BattleController.CheckElement(attacker, isAttackerLeader, skill);
					UnitType unit = (attacker == null ? UnitType.None : (isAttackerLeader ? attacker.Leader.Type : attacker.Unit.Type));
					switch (buf.Type)
					{
						case BufferType.MagicDefenceUp:
						case BufferType.MagicAttackAndDefenceUp:
							value += buf.Power;
							break;
						case BufferType.MagicDefenceDown:
							value -= buf.Power;
							break;
						case BufferType.RangeDefenceUp:
							bool isRange = skill == null ? (attacker == null ? false : attacker.Range > Unit.RANGER_RANGE) : !skill.IsMelee;
							if (isRange)
								value += buf.Power;
							break;
						case BufferType.CavalryDefenceUp:
							if (unit == UnitType.Cavalry)
								value += buf.Power;
							break;
						case BufferType.LightDefenceUp:
							if (attackElement == Element.Light)
								value += buf.Power;
							break;
						case BufferType.DarkDefenceUp:
							if (attackElement == Element.Dark)
								value += buf.Power;
							break;
						case BufferType.WaterDefenceUp:
							if (attackElement == Element.Water)
								value += buf.Power;
							break;
						case BufferType.FireDefenceUp:
							if (attackElement == Element.Fire)
								value += buf.Power;
							break;
						case BufferType.AirDefenceUp:
							if (attackElement == Element.Air)
								value += buf.Power;
							break;
						case BufferType.EarthDefenceUp:
							if (attackElement == Element.Earth)
								value += buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckSpeedBuffer(Troop troop)
		{
			int value = 0;

			foreach (Buffer buf in troop.Buffers)
			{
				if (buf.Duration > 0)
				{
					switch (buf.Type)
					{
						case BufferType.SpeedUp:
							value += buf.Power;
							break;
						case BufferType.SpeedDown:
							value -= buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckSpBuffer(Troop attacker, Troop defender, Skill skill)
		{
			int value = 0;

			foreach (Buffer buf in attacker.Buffers)
			{
				if (buf.Duration > 0)
				{
					switch (buf.Type)
					{
						case BufferType.SpCostDown:
							value -= buf.Power;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}
		#endregion

		#region Passives
		private static int CheckAttackPassive(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (var pair in attacker.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive)
				{
					Element element = (defender == null ? Element.None : (defender.CurrentNumber > 0 ? defender.Unit.Element :
						(defender.LeaderHp > 0 ? defender.Leader.Element : Element.None)));
					UnitType unit = (defender == null ? UnitType.None : (defender.CurrentNumber > 0 ? defender.Unit.Type :
						(defender.LeaderHp > 0 ? defender.Leader.Type : UnitType.None)));
					switch (sk.Buffer.Type)
					{
						case BufferType.AttackUp:
						case BufferType.AttackAndDefenceUp:
							value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.AttackDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.InfantryAttackUp:
							if (unit == UnitType.Infantry)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.CavalryAttackUp:
							if (unit == UnitType.Cavalry)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.LightAttackUp:
							if (element == Element.Light)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DarkAttackUp:
							if (element == Element.Dark)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.WaterAttackUp:
							if (element == Element.Water)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.FireAttackUp:
							if (element == Element.Fire)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.AirAttackUp:
							if (element == Element.Air)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.EarthAttackUp:
							if (element == Element.Light)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckDefencePassive(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (var pair in defender.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive && sk.Target == SkillTarget.Self)
				{
					Element attackElement = BattleController.CheckElement(attacker, isAttackerLeader, skill);
					UnitType unit = (attacker == null ? UnitType.None : (isAttackerLeader ? attacker.Leader.Type : attacker.Unit.Type));
					switch (sk.Buffer.Type)
					{
						case BufferType.DefenceUp:
						case BufferType.AttackAndDefenceUp:
							value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DefenceDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.RangeDefenceUp:
							bool isRange = skill == null ? (attacker == null ? false : attacker.Range > Unit.RANGER_RANGE) : !skill.IsMelee;
							if (isRange)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.CavalryDefenceUp:
							if (unit == UnitType.Cavalry)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.LightDefenceUp:
							if (attackElement == Element.Light)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DarkDefenceUp:
							if (attackElement == Element.Dark)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.WaterDefenceUp:
							if (attackElement == Element.Water)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.FireDefenceUp:
							if (attackElement == Element.Fire)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.AirDefenceUp:
							if (attackElement == Element.Air)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.EarthDefenceUp:
							if (attackElement == Element.Earth)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckDamagePassive(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (var pair in attacker.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive && sk.Target == SkillTarget.Self)
				{
					switch (sk.Buffer.Type)
					{
						case BufferType.DamageUp:
							value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DamageDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DamageUpWithRange:
							if (defender != null)
							{
								int range = BattleController.CheckRange(attacker, defender);
								value += range * (sk.Buffer.Power + sk.PowerGrow * pair.Value);
							}
							break;
						case BufferType.PhysicSkillDamageUp:
							if (skill != null && skill.SkillType == SkillType.Physic)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckMagicAttackPassive(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (var pair in attacker.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive)
				{
					Element element = (defender == null ? Element.None : (defender.CurrentNumber > 0 ? defender.Unit.Element : defender.Leader.Element));
					switch (sk.Buffer.Type)
					{
						case BufferType.MagicAttackUp:
						case BufferType.MagicAttackAndDefenceUp:
							value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.MagicAttackDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.LightAttackUp:
							if (element == Element.Light)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DarkAttackUp:
							if (element == Element.Dark)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.WaterAttackUp:
							if (element == Element.Water)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.FireAttackUp:
							if (element == Element.Fire)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.AirAttackUp:
							if (element == Element.Air)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.EarthAttackUp:
							if (element == Element.Light)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckMagicDefencePassive(Troop attacker, bool isAttackerLeader, Troop defender, bool isCounter, Skill skill)
		{
			int value = 0;

			foreach (var pair in defender.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive && sk.Target == SkillTarget.Self)
				{
					Element attackElement = BattleController.CheckElement(attacker, isAttackerLeader, skill);
					switch (sk.Buffer.Type)
					{
						case BufferType.MagicDefenceUp:
						case BufferType.MagicAttackAndDefenceUp:
							value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.MagicDefenceDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.RangeDefenceUp:
							bool isRange = skill == null ? (attacker == null ? false : attacker.Range > Unit.RANGER_RANGE) : !skill.IsMelee;
							if (isRange)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.LightDefenceUp:
							if (attackElement == Element.Light)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.DarkDefenceUp:
							if (attackElement == Element.Dark)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.WaterDefenceUp:
							if (attackElement == Element.Water)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.FireDefenceUp:
							if (attackElement == Element.Fire)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.AirDefenceUp:
							if (attackElement == Element.Air)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.EarthDefenceUp:
							if (attackElement == Element.Earth)
								value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckSpeedPassive(Troop troop)
		{
			int value = 0;

			foreach (var pair in troop.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive && sk.Target == SkillTarget.Self)
				{
					switch (sk.Buffer.Type)
					{
						case BufferType.SpeedUp:
							value += sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						case BufferType.SpeedDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		private static int CheckSpPassive(Troop attacker, Troop defender, Skill skill)
		{
			int value = 0;

			foreach (var pair in attacker.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.Passive && sk.Target == SkillTarget.Self)
				{
					switch (sk.Buffer.Type)
					{
						case BufferType.SpCostDown:
							value -= sk.Buffer.Power + sk.PowerGrow * pair.Value;
							break;
						default:
							break;
					}
				}
			}

			return value;
		}

		#endregion
	}
}
