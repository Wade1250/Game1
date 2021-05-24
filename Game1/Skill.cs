using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Game1
{
	public enum SkillType
	{
		Physic,
		Magic,
		Heal,
		Buffer,
		Curse,
		Passive,
		StartUp,
	}

	public enum SkillTarget
	{
		Self,
		PartnerSingle,
		PartnerAll,
		EnemySingle,
		EnemyAll,
	}

	public class Skill
	{
		const string SKILL_PATH = "Xml\\Skills\\";
		private static readonly string[] SKILL_FILES = new string[]{ "PhysicSkills.xml", "MagicSkills.xml", "HealSkills.xml",
			"BufferSkills.xml", "CurseSkills.xml", "PassiveSkills.xml", "StartUpSkills.xml" };

		public static Dictionary<int, Skill> Table = new Dictionary<int, Skill>();

		public int Id;
		public String Name;
		public String Description;

		public SkillType SkillType;
		public bool IsMelee;
		public Element Element;
		public SkillTarget Target;

		public int Range;
		public int AttackBonus;
		public int AttackGrow;
		public int DamageBonus;
		public int DamageGrow;
		public int SkillPoint;
		public int SpGrow;

		public Buffer Buffer;
		public int PowerGrow;

		public Skill()
		{
			Id = -1;
			Name = "Default Skill";
			Description = "";

			SkillType = SkillType.Buffer;
			Element = Element.None;
			Target = SkillTarget.Self;
			Range = 0;
			AttackBonus = 0;
			AttackGrow = 0;
			DamageBonus = 0;
			DamageGrow = 0;
			SkillPoint = 0;
			SpGrow = 0;

			Buffer = null;
			PowerGrow = 0;
		}

		public static void LoadSkills()
		{
			List<Skill> skills = null;
			foreach (string name in SKILL_FILES)
			{
				string file = SKILL_PATH + name;
				using (var sr = new StreamReader(file))
				{
					//create the serialiser to create the xml
					XmlSerializer xs = new XmlSerializer(typeof(List<Skill>));
					skills = (List<Skill>)xs.Deserialize(sr);
				}

				foreach (var sk in skills)
					Table.Add(sk.Id, sk);
			}
		}

		//public static void SaveSkills()
		//{
		//	XmlSerializer xs = new XmlSerializer(typeof(List<Skill>));
		//	TextWriter tw = new StreamWriter(SKILL_FILE);
		//	xs.Serialize(tw, Table);
		//}

		//public static string ToString(int id)
		//{
		//	Skill skill = null;
		//	foreach (var item in Table)
		//	{
		//		if (item.Key == id)
		//		{
		//			skill = item.Value;
		//			break;
		//		}
		//	}
		//	string str = "";

		//	if (skill != null)
		//	{
		//		str += "[" + skill.SkillType.ToString() + "] " + skill.Name + "\r\n";
		//	}

		//	return str;
		//}
	}
}

