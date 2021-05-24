using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public enum BufferType
	{
		None,
		AttackUp,
		DefenceUp,
		DamageUp,
		MagicAttackUp,
		MagicDefenceUp,
		AttackDown,
		DefenceDown,
		DamageDown,
		MagicAttackDown,
		MagicDefenceDown,
		SpeedUp,
		SpeedDown,
		SpCostDown,

		AttackAndDefenceUp,
		MagicAttackAndDefenceUp,
		RangeDefenceUp,
		DamageUpWithRange,
		InfantryAttackUp,
		CavalryAttackUp,
		CavalryDefenceUp,
		PhysicSkillDamageUp,

		LightAttackUp,
		DarkAttackUp,
		WaterAttackUp,
		FireAttackUp,
		AirAttackUp,
		EarthAttackUp,
		LightDefenceUp,
		DarkDefenceUp,
		WaterDefenceUp,
		FireDefenceUp,
		AirDefenceUp,
		EarthDefenceUp,
	}

	public class Buffer
	{
		public int SkillId;
		public BufferType Type;
		public int Duration;
		public int Power;
	}
}
