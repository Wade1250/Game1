using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game1
{
	struct MovePoint : IComparable<MovePoint>
	{
		public int Id;
		public int Speed;
		public int Priority;
		public int Value;

		public int CompareTo(MovePoint other)
		{
			// Value greater in front.
			if (this.Value > other.Value)
				return -1;
			else if (this.Value == other.Value)
			{
				// Priority smaller in front.
				if (this.Priority < other.Priority)
					return -1;
				else if (this.Priority > other.Priority)
					return 1;
				else
					return 0;	// Should not have same priority.
			}
			return 1;
		}
	}

	enum DamagePredict
	{
		Normal,
		MinimumDamage,
		MaximumDamage,
	}

	class BattleController
	{
		public const int MAX_TROOPS = 5;
		public const int MAX_LEADERS = 25;
		public const int ENEMY_ID_BIAS = 200;
		public const int COLUMN_SIZE = 3;
		public const int FIELD_SIZE = COLUMN_SIZE * COLUMN_SIZE;
		public const int ENEMY_POSITION_BIAS = 100;   //TODO: Global constant?
		public const int RANDOM_LEADER_ID_BASE = 10000;
		public const int MAX_FORMATION = 5;
		public const int MAX_BATTLE_COUNT = 30;

		//public delegate void TextDelegate(object obj, string text);
		//public delegate void TroopDelegate(object obj, Troop troop);

		public EventHandler<string> ShowMessage = null;
		public EventHandler<int> ShowBattleCount = null;
		public EventHandler<int> ShowScore = null;
		public EventHandler<int> ShowPoint = null;
		public EventHandler<Troop> ShowTroop = null;
		public EventHandler<int> ShowEnable = null;
		public EventHandler<bool> ClearBattleField = null;
		public EventHandler<Troop> ShowTroopInfo = null;
		public EventHandler<int[]> ShowMapping = null;
		public EventHandler<List<MovePoint>> ShowMovePoints = null;
		public EventHandler<List<int>> ShowOrder = null;
		public EventHandler<List<Troop>> ShowFriends = null;
		public EventHandler<Troop[]> ShowEnermies = null;
		public EventHandler<bool> ShowForm = null;
		public EventHandler<Troop> ShowSkillSelect = null;
		public EventHandler<string> ShowSkillDescription = null;
		public EventHandler ShowGameOver = null;
		public EventHandler<int[]> ShowWave = null;

		//Troop[] _friendTroops, _enemyTroops;
		List<Troop> _friendTroops;
		Troop[] _enemyTroops;
		int[] _positionMapping = new int[FIELD_SIZE * 2];
		//int[] _formation = new int[FIELD_SIZE];
		List<int[]> _allFormation = new List<int[]>();
		int _currentGroup = 0;
		List<MovePoint> _movePoints = null;

		private Random _rand = new Random();
		private int _actionId = -1;
		private bool _isOver = false;
		Thread _thread = null;

		private int _battleCount = 0;
		private int _score = 0;
		private int _point = 0;

		// Battle flow:
		// 1. LoadBattle()
		//    a. Load battle config.
		//    b. Check criteria.
		//    c. Modify status.
		// 2. StartBattle()
		//    a. Initialize ActionValue.
		//    b. Initialize PrioritySequence.
		//    c. Check highest speed.
		//    d. Check ActionValue.
		//    e. Check battle ends.
		//    f. Next round.
		//    g. Repeat c~g.

		public BattleController()
		{
			Skill.LoadSkills();

			//_friendTroops = new Troop[MAX_TROOPS];
			_friendTroops = new List<Troop>();
			_enemyTroops = new Troop[MAX_TROOPS];
			for (int i = 0; i < MAX_TROOPS; i++)
			{
				//_friendTroops.Add(null);
				//_friendTroops[i] = null;
				_enemyTroops[i] = null;
			}

			//for (int i = 0; i < FIELD_SIZE; i++)
			//	_formation[i] = -1;
			for (int i = 0; i < MAX_FORMATION; i++)
			{
				int[] newForm = new int[FIELD_SIZE];
				for (int j = 0; j < newForm.Length; j++)
					newForm[j] = -1;
				_allFormation.Add(newForm);
			}
			_currentGroup = 0;
			for (int i = 0; i < FIELD_SIZE * 2; i++)
				_positionMapping[i] = -1;

			//Skill sk = new Skill();
			//Skill.Table = new Dictionary<int, Skill>();
			//Skill.Table.Add(sk.Id, sk);
			//Skill.SaveSkills();
		}

		~BattleController()
		{
			if (_thread != null)
			{
				_isOver = true;
				_thread.Abort();
			}
		}

		public void GenerateFriendTroops()
		{
			
			Troop troop0 = new Troop();

			troop0.Leader = Leader.LoadWithId(1);
			troop0.Unit = Unit.LoadWithId(troop0.Leader.UnitId[0]);

			//troop0.Leader.LevelUp(100);
			//_friendTroops[0].Leader.LevelUp(10);

			//_friendTroops[0].Leader = new Leader();
			//_friendTroops[0].Unit = new Unit();
			//_friendTroops[0].Leader.SaveToFile("LeaderDefault.xml");
			//_friendTroops[0].Unit.SaveToFile("UnitDefault.xml");

			//troopA1.Unit.LoadFromFile("UnitDefault.xml");
			//troopB1.Unit.LoadFromFile("Unit0000.xml");
			//troopA1.MaxNumber = troopA1.TroopNumber = troopA1.CurrentNumber = troopA1.Leader.UnitNumber[0];
			troop0.LeaderHp = troop0.Leader.HitPoint;
			troop0.MaxNumber = troop0.TroopNumber = troop0.CurrentNumber = troop0.Leader.UnitSizeBase;
			troop0.UnitHp = troop0.Unit.HitPoint;
			troop0.LeaderSp = troop0.Leader.SkillPoint;
			troop0.UnitSp = troop0.Unit.SkillPoint;

			troop0.Position = 2;
			_friendTroops.Add(troop0);

			//_formation[1] = 0;
			for (int i = 0; i < MAX_FORMATION; i++)
				_allFormation[i][1] = 0;
			_positionMapping[1] = 0;

			Troop troop1 = new Troop();
			troop1.Leader = Leader.LoadWithId(2);
			troop1.Unit = Unit.LoadWithId(troop1.Leader.UnitId[0]);

			//troop1.Leader.LevelUp(100);

			troop1.LeaderHp = troop1.Leader.HitPoint;
			troop1.MaxNumber = troop1.TroopNumber = troop1.CurrentNumber = troop1.Leader.UnitSizeBase;
			troop1.UnitHp = troop1.Unit.HitPoint;
			troop1.LeaderSp = troop1.Leader.SkillPoint;
			troop1.UnitSp = troop1.Unit.SkillPoint;

			troop1.Position = 8;
			_friendTroops.Add(troop1);

			//_formation[7] = 1;
			for (int i = 0; i < MAX_FORMATION; i++)
				_allFormation[i][7] = 1;
			_positionMapping[7] = 1;

			ShowTroop?.Invoke(this, _friendTroops[0]);
			ShowTroop?.Invoke(this, _friendTroops[1]);
		}

		void GenerateEnermies(int battleCount, int[] waves)
		{
			for (int i = 0; i < MAX_TROOPS; i++)
				_enemyTroops[i] = null;
			for (int i = FIELD_SIZE; i < FIELD_SIZE * 2; i++)
				_positionMapping[i] = -1;

			//_enemyTroops[0] = new Troop();
			//_enemyTroops[0].Leader = null;
			//_enemyTroops[0].Unit = Unit.LoadWithId(2);
			//_enemyTroops[0].LeaderHp = 0;
			//_enemyTroops[0].MaxNumber = _enemyTroops[0].TroopNumber = _enemyTroops[0].CurrentNumber = 15;
			//_enemyTroops[0].UnitHp = _enemyTroops[0].Unit.HitPoint;

			//_enemyTroops[0].Position = ENERMY_POSITION_BIAS + 1;
			//_positionMapping[FIELD_SIZE] = MAX_TROOPS;

			//if (true)
			if (battleCount % 10 == 0)
			{
				if (waves[0] == waves[1])
				{
					_enemyTroops[0] = new Troop();
					_enemyTroops[0].Leader = Leader.LoadWithId(9001);
					_enemyTroops[0].Unit = null;

					_enemyTroops[0].Leader.LevelUp(battleCount);

					_enemyTroops[0].Leader.HitPoint = _enemyTroops[0].Leader.HitPoint * (100 + battleCount) / 100;
					_enemyTroops[0].Leader.MinDamage = _enemyTroops[0].Leader.MinDamage * (100 + battleCount) / 100;
					_enemyTroops[0].Leader.MaxDamage = _enemyTroops[0].Leader.MaxDamage * (100 + battleCount) / 100;

					_enemyTroops[0].LeaderHp = _enemyTroops[0].Leader.HitPoint;
					_enemyTroops[0].LeaderSp = _enemyTroops[0].Leader.SkillPoint;
					_enemyTroops[0].MaxNumber = _enemyTroops[0].TroopNumber = _enemyTroops[0].CurrentNumber = 0;
					_enemyTroops[0].UnitHp = 0;
					_enemyTroops[0].UnitSp = 0;

					_enemyTroops[0].Position = ENEMY_POSITION_BIAS + 2;
					_positionMapping[FIELD_SIZE + 1] = ENEMY_ID_BIAS + 0;
				}
				else
				{
					List<int> unitLists = new List<int>();
					if (battleCount <= 10)
						unitLists.Add(9001);
					else if (battleCount <= 20)
						unitLists.AddRange(new int[] { 9001, 9002 });
					else if (battleCount <= 30)
						unitLists.AddRange(new int[] { 9002 });
					else
						unitLists.AddRange(new int[] { 9002 });

					int numberMin = battleCount / 20 + 1;
					int numberMax = battleCount / 10 + 1;
					//int min = battleCount;
					//int max = battleCount * 3 - battleCount / 10 * 10;
					int min = (int)((10 + battleCount * 5) * (1 + (float)battleCount / 100) * 0.5f);
					int max = (int)((20 + battleCount * 5) * (1 + (float)battleCount / 100));

					int unitMinLevel = battleCount / 20;
					int unitMaxLevel = battleCount / 10 + 1;

					int number = 3;
					if (battleCount > 20)
						number++;

					int minLeaderLv = battleCount / 2;
					int maxLeaderLv = battleCount;
					GenerateEnermies(battleCount, minLeaderLv, maxLeaderLv, unitLists.ToArray(), unitMinLevel, unitMaxLevel, number, min, max);
				}
			}
			else if (battleCount % 5 == 0)
			{
				if (waves[0] == waves[1])
				{
					_enemyTroops[0] = new Troop();
					_enemyTroops[0].Leader = Leader.LoadWithId(4);
					_enemyTroops[0].Unit = Unit.LoadWithId(_enemyTroops[0].Leader.UnitId[0]);

					_enemyTroops[0].Leader.LevelUp(battleCount);
					_enemyTroops[0].Unit.LevelUp(battleCount / 10 + 1);

					_enemyTroops[0].LeaderHp = _enemyTroops[0].Leader.HitPoint;
					_enemyTroops[0].MaxNumber = _enemyTroops[0].TroopNumber = _enemyTroops[0].CurrentNumber =
						(int)(_enemyTroops[0].Leader.UnitSizeBase * (1 + (float)battleCount / 100));
					_enemyTroops[0].UnitHp = _enemyTroops[0].Unit.HitPoint;
					_enemyTroops[0].LeaderSp = _enemyTroops[0].Leader.SkillPoint;
					_enemyTroops[0].UnitSp = _enemyTroops[0].Unit.SkillPoint;

					_enemyTroops[0].Position = ENEMY_POSITION_BIAS + 1;
					_positionMapping[FIELD_SIZE] = ENEMY_ID_BIAS + 0;

					int[] follows = { 0, 1005, 1006, 1007, 1004 };
					int[] positions = { 1, 5, 9, 3, 7 };

					for (int i = 1; i <= battleCount / 10 + 1 && i < MAX_TROOPS; i++)
					{
						Troop troop = new Troop();
						troop.Unit = Unit.LoadWithId(follows[i]);
						troop.Unit.LevelUp(battleCount / 10 + 1);

						troop.Leader = LeaderGenerator.GenerateLeader(troop.Unit, true);
						troop.Leader.LevelUp(battleCount);
						troop.LeaderHp = troop.Leader.HitPoint;
						troop.LeaderSp = troop.Leader.SkillPoint;

						troop.MaxNumber = troop.TroopNumber = troop.CurrentNumber =
							(int)((10 + battleCount * 5) * (1 + 0.2f * (1 - troop.Unit.Rank)) * (1 + (float)battleCount / 100));
						troop.UnitHp = troop.Unit.HitPoint;
						troop.UnitSp = troop.Unit.SkillPoint;

						troop.Position = ENEMY_POSITION_BIAS + positions[i];
						_enemyTroops[i] = troop;
						_positionMapping[FIELD_SIZE + positions[i] - 1] = ENEMY_ID_BIAS + i;
					}
				}
				else
				{
					List<int> unitLists = new List<int>();
					if (battleCount <= 10)
						unitLists.Add(1001);
					else if (battleCount <= 20)
						unitLists.AddRange(new int[] { 1001, 1002, 1003, 1005, 1006 });
					else if (battleCount <= 30)
						unitLists.AddRange(new int[] { 1005, 1006, 1004, 1007 });
					else
						unitLists.AddRange(new int[] { 1005, 1006, 1004, 1007 });

					int numberMin = battleCount / 20 + 1;
					int numberMax = battleCount / 10 + 1;
					//int min = battleCount;
					//int max = battleCount * 3 - battleCount / 10 * 10;
					int min = (int)((10 + battleCount * 5) * (1 + (float)battleCount / 100) * 0.5f);
					int max = (int)((20 + battleCount * 5) * (1 + (float)battleCount / 100));

					int unitMinLevel = battleCount / 20;
					int unitMaxLevel = battleCount / 10 + 1;

					int number = 3;
					if (battleCount > 20)
						number++;

					int minLeaderLv = battleCount / 2;
					int maxLeaderLv = battleCount;
					GenerateEnermies(battleCount, minLeaderLv, maxLeaderLv, unitLists.ToArray(), unitMinLevel, unitMaxLevel, number, min, max);
				}
			}
			else
			{
				List<int> unitLists = new List<int>();
				if (battleCount <= 5)
				{
					unitLists.Add(1);
					unitLists.Add(1001);
					unitLists.Add(9001);
				}
				else if (battleCount <= 10)
				{
					unitLists.AddRange(new int[] { 1, 2, 3, 4, 5 });
					//unitLists.AddRange(new int[] { 1001, 1002, 1003, 1005, 1006 });
					//unitLists.AddRange(new int[] { 9001, 9002 });
				}
				else if (battleCount <= 15)
				{
					//unitLists.AddRange(new int[] { 1, 2, 3, 4, 5 });
					unitLists.AddRange(new int[] { 1001, 1002, 1003, 1005, 1006 });
					//unitLists.AddRange(new int[] { 9002 });
				}
				else if (battleCount <= 20)
				{
					//unitLists.AddRange(new int[] { 4, 5, 7, 8 });
					//unitLists.AddRange(new int[] { 1002, 1003, 1005, 1006, 1004, 1007 });
					unitLists.AddRange(new int[] { 9001, 9002 });
				}
				else if (battleCount <= 25)
				{
					unitLists.AddRange(new int[] { 4, 5, 7, 8 });
					unitLists.AddRange(new int[] { 1005, 1006, 1004, 1007 });
					//unitLists.AddRange(new int[] { 9002 });
				}
				//else if (battleCount <= 60)
				//{
				//	//unitLists.AddRange(new int[] { 2, 3, 4, 5, 7, 8 });
				//	unitLists.AddRange(new int[] { 1005, 1006, 1004, 1007 });
				//	//unitLists.AddRange(new int[] { 9002 });
				//}
				else if (battleCount <= 30)
				{
					//unitLists.AddRange(new int[] { 2, 3, 4, 5, 7, 8 });
					//unitLists.AddRange(new int[] { 1005, 1006, 1004, 1007 });
					unitLists.AddRange(new int[] { 9002 });
				}
				else
				{
					unitLists.AddRange(new int[] { 4, 5, 7, 8 });
					unitLists.AddRange(new int[] { 1005, 1006, 1004, 1007 });
					unitLists.AddRange(new int[] { 9002 });
				}

				//int numberMin = 2 + (battleCount - 1) / 20;
				//int numberMax = 3 + (battleCount - 1) / 20;
				//int min = battleCount;
				//int max = battleCount * 3 - battleCount / 10 * 10;
				int min = (int)((10 + battleCount * 5) * (1 + (float)battleCount / 100) * 0.5f);
				int max = (int)((20 + battleCount * 5) * (1 + (float)battleCount / 100));

				int unitMinLevel = battleCount / 20;
				int unitMaxLevel = battleCount / 10 + 1;

				//int number = 1;
				//if (battleCount >= 10)
				//	number++;
				//if (battleCount >= 40)
				//	number++;
				//if (battleCount >= 70)
				//	number++;
				//if (battleCount >= 90)
				//	number++;
				int number = 3;
				if (battleCount > 20)
					number++;
				//int number = numberMin + _rand.Next((numberMax - numberMin) + 1);
				//number = number > MAX_TROOPS ? MAX_TROOPS : number;

				int minLeaderLv = battleCount / 2;
				int maxLeaderLv = battleCount;
				GenerateEnermies(battleCount / 2, minLeaderLv, maxLeaderLv, unitLists.ToArray(), unitMinLevel, unitMaxLevel, number, min, max);
			}

			ClearBattleField?.Invoke(this, false);
			for (int i = 0; i < _enemyTroops.Length; i++)
				if (_enemyTroops[i] != null)
					ShowTroop?.Invoke(this, _enemyTroops[i]);
		}

		void GenerateEnermies(int chanceWithLeader, int minLeaderLv, int maxLeaderLv, int[] unitLists, int unitMinLv, int unitMaxLv, int number, int min, int max)
		{
			// Front: 123; Middle: 456, Back: 789.
			int[] count = { 0, 0, 0 };

			if (unitLists.Length > 0)
			{
				//TODO: Generate with leader.

				for (int i = 0; i < number; i++)
				{
					_enemyTroops[i] = new Troop();

					int unitId = _rand.Next(unitLists.Length);
					_enemyTroops[i].Unit = Unit.LoadWithId(unitLists[unitId]);
					int unitLevel = unitMinLv + _rand.Next(unitMaxLv - unitMinLv + 1);
					_enemyTroops[i].Unit.LevelUp(unitLevel);

					int size = min + _rand.Next(max - min);
					// Adjust troop size according to rank.
					float refine = 1.0f - (_enemyTroops[i].Unit.Rank - 1) * 0.2f;
					size = (int)(size * refine);

					if (_rand.Next(100) < chanceWithLeader)
					{
						_enemyTroops[i].Leader = LeaderGenerator.GenerateLeader(_enemyTroops[i].Unit, true);
						_enemyTroops[i].Leader.LevelUp(minLeaderLv + _rand.Next(maxLeaderLv - minLeaderLv + 1));
						_enemyTroops[i].LeaderHp = _enemyTroops[i].Leader.HitPoint;
						_enemyTroops[i].LeaderSp = _enemyTroops[i].Leader.SkillPoint;
					}
					else
					{
						_enemyTroops[i].Leader = null;
						_enemyTroops[i].LeaderHp = 0;
						_enemyTroops[i].LeaderSp = 0;
					}

					_enemyTroops[i].MaxNumber = _enemyTroops[i].TroopNumber = _enemyTroops[i].CurrentNumber = size;
					_enemyTroops[i].UnitHp = _enemyTroops[i].Unit.HitPoint;
					_enemyTroops[i].UnitSp = _enemyTroops[i].Unit.SkillPoint;

					int[] cols;
					switch (_enemyTroops[i].Unit.Type)
					{
						case UnitType.Infantry:
							cols = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
							break;
						case UnitType.Archer:
							cols = new int[] { 0, 1, 1, 1, 2, 2, 2, 2, 2, 2 };
							break;
						case UnitType.Healer:
							cols = new int[] { 0, 0, 0, 1, 1, 1, 1, 2, 2, 2 };
							break;
						case UnitType.Wizard:
							cols = new int[] { 0, 1, 1, 2, 2, 2, 2, 2, 2, 2 };
							break;
						case UnitType.Cavalry:
							cols = new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 };
							break;
						case UnitType.Flyer:
							cols = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
							break;
						case UnitType.Support:
							cols = new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 2, 2 };
							break;
						default:
							cols = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
							break;
					}

					int col = cols[_rand.Next(10)];
					int position = 0;
					if (count[col] >= COLUMN_SIZE)
					{
						for (int j = 0; j < 3; j++)
						{
							if (count[j] < COLUMN_SIZE)
							{
								col = j;
								break;
							}
						}
					}

					position = col * COLUMN_SIZE + count[col];

					_enemyTroops[i].Position = ENEMY_POSITION_BIAS + position + 1;
					_positionMapping[FIELD_SIZE + position] = ENEMY_ID_BIAS + i;
					count[col]++;

					//ShowTroop?.Invoke(this, _enemyTroops[i]);
				}
			}
		}

		public void StartBattle()
		{
			_thread = new Thread(new ThreadStart(BattleThread));
			_thread.Start();
		}

		public void BattleThread()
		{
			bool isWin = false;
			bool isLost = false;

			GenerateFriendTroops();

			while (!_isOver && !isLost)
			{
				isWin = false;
				_battleCount++;

				ShowBattleCount?.Invoke(this, _battleCount);
				ShowScore?.Invoke(this, _score);
				ShowPoint?.Invoke(this, _point);

				// Replenish?!
				Replenish();
				// Clear all buffer.
				for (int i = 0; i < _friendTroops.Count; i++)
					_friendTroops[i].Buffers.Clear();

				//Town Form
				if (_battleCount > 1)
				{
					ShowForm?.Invoke(this, false);
					for (int i = 0; i < _friendTroops.Count; i++)
						_friendTroops[i].Position = -1;
					for (int i = 0; i < FIELD_SIZE; i++)
					{
						if (_allFormation[_currentGroup][i] != -1)
							_friendTroops[_allFormation[_currentGroup][i]].Position = i + 1;
					}

					using (TownForm tf = new TownForm())
					{
						//tf.Setup(ref _friendTroops, ref _positionMapping, ref _point);
						tf.Setup(ref _friendTroops, ref _allFormation, ref _currentGroup, ref _point, _battleCount);
						tf.UpdateData(this, null);
						Application.Run(tf);
						_point = tf.Points;
					}
					ShowPoint?.Invoke(this, _point);
					ShowForm?.Invoke(this, true);
				}

				for (int i = 0; i < FIELD_SIZE; i++)
					_positionMapping[i] = _allFormation[_currentGroup][i];

				// Generate enermies.
				ClearBattleField?.Invoke(this, true);
				//for (int i = 0; i < MAX_TROOPS; i++)
				//{
				//	if (_friendTroops[i] != null)
				//		ShowTroop?.Invoke(this, _friendTroops[i]);
				//}
				for (int i = 0; i < FIELD_SIZE; i++)
				{
					if (_positionMapping[i] != -1)
						ShowTroop?.Invoke(this, _friendTroops[_positionMapping[i]]);
				}
				int[] waves = new int[2];
				waves[0] = 1;
				waves[1] = 1;
				if (_battleCount % 5 == 4)
					waves[1]++;
				if (_battleCount % 5 == 0)
					waves[1]++;
				if (_battleCount % 10 == 0)
					waves[1]++;
				GenerateEnermies(_battleCount, waves);

				if (_battleCount > 1)
					ShowMessage?.Invoke(this, "");
				string msg = "第" + _battleCount + "戰開始...";
				ShowMessage?.Invoke(this, msg);
				ShowTroopInfo?.Invoke(this, null);

				msg = "Wave " + waves[0] + "/" + waves[1];
				ShowMessage?.Invoke(this, msg);
				ShowWave?.Invoke(this, waves);

				// Check startup skills
				for (int i = 0; i < FIELD_SIZE; i++)
				{
					if (_positionMapping[i] != -1)
					{
						Troop troop = _friendTroops[_positionMapping[i]];
						StartUpAction(troop, false);
					}

					if (_positionMapping[FIELD_SIZE + i] != -1)
					{
						Troop troop = _enemyTroops[_positionMapping[FIELD_SIZE + i] - ENEMY_ID_BIAS];
						StartUpAction(troop, true);
					}
				}

				_movePoints = new List<MovePoint>();
				int priority = 0;
				//List<Troop> nowGroup = new List<Troop>();
				for (int i = 0; i < FIELD_SIZE; i++)
				{
					MovePoint mp = new MovePoint();
					if (_positionMapping[i] != -1)
					{
						mp.Id = _positionMapping[i];
						mp.Priority = priority++;
						Troop troop = _friendTroops[mp.Id];

						mp.Value = troop.Initiative;
						mp.Speed = troop.Speed + Modifier.CheckSpeedModifier(troop);
						mp.Speed = mp.Speed > 0 ? mp.Speed : 0;
						_movePoints.Add(mp);

						//nowGroup.Add(_friendTroops[mp.Id]);
					}

					mp = new MovePoint();
					if (_positionMapping[FIELD_SIZE + i] != -1)
					{
						mp.Id = _positionMapping[FIELD_SIZE + i];
						int id = mp.Id - ENEMY_ID_BIAS;
						mp.Priority = priority++;
						mp.Value = _enemyTroops[id].Initiative;
						mp.Speed = _enemyTroops[id].Speed + Modifier.CheckSpeedModifier(_enemyTroops[id]);
						mp.Speed = mp.Speed > 0 ? mp.Speed : 0;
						_movePoints.Add(mp);
					}
				}

				Thread.Sleep(100);
				ShowFriends?.Invoke(this, _friendTroops);
				//ShowFriends?.Invoke(this, nowGroup.ToArray());
				ShowEnermies?.Invoke(this, _enemyTroops);

				while (!_isOver && !isWin && !isLost)
				{
					int maxSpeed = CheckMaxSpeed(_movePoints);
					_movePoints.Sort();

					for (int i = 0; i < _movePoints.Count; i++)
					{
						ShowMovePoints?.Invoke(this, _movePoints);
						ComputeActionSequences(_movePoints, i, maxSpeed, priority);

						MovePoint mp = _movePoints[i];
						Troop currentTroop = null;
						if (mp.Id < ENEMY_ID_BIAS)
							currentTroop = _friendTroops[mp.Id];
						else
							currentTroop = _enemyTroops[mp.Id - ENEMY_ID_BIAS];

						if (currentTroop.IsAlive)
						{
							if (mp.Value >= maxSpeed)
							{
								// Check if any empty.
								// Only first column should be checked.
								int isEmptyColumn = 0;
								for (int j = 0; j < COLUMN_SIZE; j++)
									if (_positionMapping[j] == -1 || !_friendTroops[_positionMapping[j]].IsAlive)
										isEmptyColumn--;
								if (isEmptyColumn + COLUMN_SIZE == 0)
								{
									for (int j = 0; j < COLUMN_SIZE * 2; j++)
									{
										if (j < COLUMN_SIZE && _positionMapping[j] != -1)
											_friendTroops[_positionMapping[j]].Position = -1;
										_positionMapping[j] = _positionMapping[j + COLUMN_SIZE];
										if (_positionMapping[j] != -1)
											_friendTroops[_positionMapping[j]].Position -= COLUMN_SIZE;
									}
									for (int j = COLUMN_SIZE * 2; j < FIELD_SIZE; j++)
										_positionMapping[j] = -1;

									// Check if further.
									isEmptyColumn = 0;
									for (int j = 0; j < COLUMN_SIZE; j++)
										if (_positionMapping[j] == -1 || !_friendTroops[_positionMapping[j]].IsAlive)
											isEmptyColumn--;
									if (isEmptyColumn + COLUMN_SIZE == 0)
									{
										for (int j = 0; j < COLUMN_SIZE; j++)
										{
											if (_positionMapping[j] != -1)
												_friendTroops[_positionMapping[j]].Position = -1;
											_positionMapping[j] = _positionMapping[j + COLUMN_SIZE];
											if (_positionMapping[j] != -1)
												_friendTroops[_positionMapping[j]].Position -= COLUMN_SIZE;
										}
										for (int j = COLUMN_SIZE; j < COLUMN_SIZE * 2; j++)
											_positionMapping[j] = -1;
									}

									ShowMapping?.Invoke(this, _positionMapping);

									ClearBattleField?.Invoke(this, true);
									//for (int j = 0; j < _friendTroops.Length; j++)
									//{
									//	if (_friendTroops[j] != null)
									//		ShowTroop?.Invoke(this, _friendTroops[j]);
									//}
									for (int j = 0; j < FIELD_SIZE; j++)
									{
										if (_positionMapping[j] != -1)
											ShowTroop?.Invoke(this, _friendTroops[_positionMapping[j]]);
									}
								}

								// Enemy side.
								isEmptyColumn = 0;
								for (int j = FIELD_SIZE; j < FIELD_SIZE + COLUMN_SIZE; j++)
									if (_positionMapping[j] == -1 || !_enemyTroops[_positionMapping[j] - ENEMY_ID_BIAS].IsAlive)
										isEmptyColumn--;
								if (isEmptyColumn + COLUMN_SIZE == 0)
								{
									for (int j = FIELD_SIZE; j < FIELD_SIZE + COLUMN_SIZE * 2; j++)
									{
										if (j < FIELD_SIZE + COLUMN_SIZE && _positionMapping[j] != -1)
											_enemyTroops[_positionMapping[j] - ENEMY_ID_BIAS].Position = -1;
										_positionMapping[j] = _positionMapping[j + COLUMN_SIZE];
										if (_positionMapping[j] != -1)
											_enemyTroops[_positionMapping[j] - ENEMY_ID_BIAS].Position -= COLUMN_SIZE;
									}
									for (int j = FIELD_SIZE + COLUMN_SIZE * 2; j < FIELD_SIZE + FIELD_SIZE; j++)
										_positionMapping[j] = -1;

									// Check if further.
									isEmptyColumn = 0;
									for (int j = FIELD_SIZE; j < FIELD_SIZE + COLUMN_SIZE; j++)
										if (_positionMapping[j] == -1 || !_enemyTroops[_positionMapping[j] - ENEMY_ID_BIAS].IsAlive)
											isEmptyColumn--;
									if (isEmptyColumn + COLUMN_SIZE == 0)
									{
										for (int j = FIELD_SIZE; j < FIELD_SIZE + COLUMN_SIZE; j++)
										{
											if (_positionMapping[j] != -1)
												_enemyTroops[_positionMapping[j] - ENEMY_ID_BIAS].Position = -1;
											_positionMapping[j] = _positionMapping[j + COLUMN_SIZE];
											if (_positionMapping[j] != -1)
												_enemyTroops[_positionMapping[j] - ENEMY_ID_BIAS].Position -= COLUMN_SIZE;
										}

										for (int j = FIELD_SIZE + COLUMN_SIZE; j < FIELD_SIZE + COLUMN_SIZE * 2; j++)
											_positionMapping[j] = -1;
									}

									ClearBattleField?.Invoke(this, false);
									for (int j = 0; j < _enemyTroops.Length; j++)
									{
										if (_enemyTroops[j] != null)
											ShowTroop?.Invoke(this, _enemyTroops[j]);
									}
								}

								ShowMapping?.Invoke(this, _positionMapping);

								// Buffer count down.
								List<Buffer> newBuffers = new List<Buffer>();
								foreach (Buffer buf in currentTroop.Buffers)
								{
									buf.Duration--;
									if (buf.Duration > 0)
										newBuffers.Add(buf);
								}
								currentTroop.Buffers = newBuffers;

								//if (mp.Value >= maxSpeed)
								{
									string name;
									if (mp.Id < ENEMY_ID_BIAS)
										name = _friendTroops[mp.Id].Name;
									else
										name = _enemyTroops[mp.Id - ENEMY_ID_BIAS].Name;

									msg = name + "的行動...";
									ShowMessage?.Invoke(this, msg);

									if (mp.Id < ENEMY_ID_BIAS)
									{
										//for (int j = 0; j < MAX_TROOPS; j++)
										//{
										//	if (_enemyTroops[j] != null)
										//	{
										//		if (_enemyTroops[j].IsAlive && CheckRange(_friendTroops[mp.Id], _enemyTroops[j]) <=
										//			_friendTroops[mp.Id].Range)
										//			ShowEnable?.Invoke(this, _enemyTroops[j].Position);
										//	}
										//}
										_actionId = mp.Id;
										ShowSkillSelect?.Invoke(this, _friendTroops[mp.Id]);

										while (_actionId >= 0)
											Thread.Sleep(100);
									}
									else
									{
										// AI
										EnemyAi(mp.Id - ENEMY_ID_BIAS);
									}
									mp.Value -= maxSpeed;
									mp.Priority = priority++;   //TODO: Limited priority?!
								}

							}
							mp.Speed = currentTroop.Speed + Modifier.CheckSpeedModifier(currentTroop);
							mp.Speed = mp.Speed > 0 ? mp.Speed : 0;
							mp.Value += mp.Speed;
							_movePoints[i] = mp;

							isLost = true;
							//for (int j = 0; j < MAX_TROOPS; j++)
							//{
							//	if (_friendTroops[j] != null && (_friendTroops[j].CurrentNumber > 0 || _friendTroops[j].LeaderHp > 0))
							//		isLost = false;
							//}
							for (int j = 0; j < FIELD_SIZE; j++)
							{
								if (_positionMapping[j] != -1 && _friendTroops[_positionMapping[j]].IsAlive)
									isLost = false;
							}
							if (isLost)
								break;

							isWin = true;
							for (int j = 0; j < MAX_TROOPS; j++)
							{
								if (_enemyTroops[j] != null && (_enemyTroops[j].CurrentNumber > 0 || _enemyTroops[j].LeaderHp > 0))
									isWin = false;
							}

							if (isWin)
								break;

							ShowScore?.Invoke(this, _score);
							ShowPoint?.Invoke(this, _point);
							//ShowMovePoints?.Invoke(this, movePoints);
						}
					}

					if (isWin && waves[0] < waves[1])
					{
						waves[0]++;
						isWin = false;

						ShowMessage?.Invoke(this, "");
						msg = "Wave " + waves[0] + "/" + waves[1];
						ShowWave?.Invoke(this, waves);
						ShowMessage?.Invoke(this, msg);

						// Generate enermies.
						ClearBattleField?.Invoke(this, true);
						for (int i = 0; i < FIELD_SIZE; i++)
						{
							if (_positionMapping[i] != -1)
								ShowTroop?.Invoke(this, _friendTroops[_positionMapping[i]]);
						}
						GenerateEnermies(_battleCount, waves);

						// Check startup skills
						for (int i = 0; i < FIELD_SIZE; i++)
						{
							if (_positionMapping[i] != -1)
							{
								Troop troop = _friendTroops[_positionMapping[i]];
								StartUpAction(troop, false);
							}

							if (_positionMapping[FIELD_SIZE + i] != -1)
							{
								Troop troop = _enemyTroops[_positionMapping[FIELD_SIZE + i] - ENEMY_ID_BIAS];
								StartUpAction(troop, true);
							}
						}

						_movePoints = new List<MovePoint>();
						//priority = 0;
						//List<Troop> nowGroup = new List<Troop>();
						for (int i = 0; i < FIELD_SIZE; i++)
						{
							MovePoint mp = new MovePoint();
							if (_positionMapping[i] != -1)
							{
								mp.Id = _positionMapping[i];
								mp.Priority = priority++;
								Troop troop = _friendTroops[mp.Id];

								mp.Value = troop.Initiative;
								mp.Speed = troop.Speed + Modifier.CheckSpeedModifier(troop);
								mp.Speed = mp.Speed > 0 ? mp.Speed : 0;
								_movePoints.Add(mp);

								//nowGroup.Add(_friendTroops[mp.Id]);
							}

							mp = new MovePoint();
							if (_positionMapping[FIELD_SIZE + i] != -1)
							{
								mp.Id = _positionMapping[FIELD_SIZE + i];
								int id = mp.Id - ENEMY_ID_BIAS;
								mp.Priority = priority++;
								mp.Value = _enemyTroops[id].Initiative;
								mp.Speed = _enemyTroops[id].Speed + Modifier.CheckSpeedModifier(_enemyTroops[id]);
								mp.Speed = mp.Speed > 0 ? mp.Speed : 0;
								_movePoints.Add(mp);
							}
						}

						Thread.Sleep(100);
						ShowFriends?.Invoke(this, _friendTroops);
						//ShowFriends?.Invoke(this, nowGroup.ToArray());
						ShowEnermies?.Invoke(this, _enemyTroops);
					}
				}

				if (isWin)
				{
					int score = _battleCount * (25 + _friendTroops.Count * 3);
					if (_battleCount % 10 == 0)
						score = _battleCount * (100 + _friendTroops.Count * 4);
					msg = "戰鬥勝利!得到過關獎勵" + score + "分";
					_score += score;
					_point += score;
				}
				else
					msg = "戰鬥敗北!";

				ShowScore?.Invoke(this, _score);
				ShowPoint?.Invoke(this, _point);
				ShowMessage?.Invoke(this, msg);
				ShowMessage.Invoke(this, "");   // New line.

				if (isWin && _battleCount >= MAX_BATTLE_COUNT)
				{
					_isOver = true;
					break;
				}
			}

			if (_isOver)
				ShowMessage?.Invoke(this, "遊戲獲勝!");
			ShowGameOver?.Invoke(this, null);
		}

		private void ComputeActionSequences(List<MovePoint> movePoints, int index, int maxSpeed, int priority)
		{
			const int MAX_SHOWN = 9;
			List<int> sequence = new List<int>();
			List<MovePoint> simulation = new List<MovePoint>();

			for (int i = 0; i < movePoints.Count; i++)
			{
				MovePoint mp = new MovePoint();
				mp.Id = movePoints[i].Id;
				mp.Priority = movePoints[i].Priority;
				mp.Speed = movePoints[i].Speed;
				mp.Value = movePoints[i].Value;
				simulation.Add(mp);
			}

			while (sequence.Count <= MAX_SHOWN)
			{
				MovePoint mp = simulation[index];
				Troop troop = GetTroop(mp.Id);
				if (troop != null && troop.IsAlive)
				{
					if (simulation[index].Value >= maxSpeed)
					{
						mp.Value -= maxSpeed;
						mp.Priority = priority++;
						sequence.Add(simulation[index].Id);
					}

					mp.Value += mp.Speed;
					simulation[index] = mp;
				}

				index++;

				if (index >= simulation.Count)
				{
					index = 0;
					simulation.Sort();
				}
			}

			ShowOrder?.Invoke(this, sequence);
		}

		private void Replenish()
		{
			const float HEAL_RATIO = 0.1f;
			const float SP_RECOVERY_RATE = 0.2f;
			const int RESURRECTION = 1;

			for (int i = 0; i < _friendTroops.Count; i++)
			{
				//if (_friendTroops[i].IsAlive)
				//{
					if (_friendTroops[i].Leader != null)
					{
						_friendTroops[i].LeaderHp += (int)(_friendTroops[i].Leader.HitPoint * HEAL_RATIO);
						_friendTroops[i].LeaderSp += (int)(_friendTroops[i].Leader.SkillPoint * SP_RECOVERY_RATE);
						if (_friendTroops[i].LeaderHp > _friendTroops[i].Leader.HitPoint)
							_friendTroops[i].LeaderHp = _friendTroops[i].Leader.HitPoint;
						if (_friendTroops[i].LeaderSp > _friendTroops[i].Leader.SkillPoint)
							_friendTroops[i].LeaderSp = _friendTroops[i].Leader.SkillPoint;
					}
					if (_friendTroops[i].TroopNumber > 0)
					{
						if (_friendTroops[i].TroopNumber < _friendTroops[i].MaxNumber)
						{
							_friendTroops[i].TroopNumber += RESURRECTION;
							ShowMessage?.Invoke(this, string.Format("{0}部隊復活{1}單位。", _friendTroops[i].Name, RESURRECTION));
						}
						int heal = _friendTroops[i].TroopNumber - _friendTroops[i].CurrentNumber;
						if (heal > 0)
						{
							heal = Math.Max((int)(heal * HEAL_RATIO), 1);
							_friendTroops[i].CurrentNumber += heal;
							ShowMessage?.Invoke(this, string.Format("{0}部隊回復{1}單位。", _friendTroops[i].Name, heal));
						}
						_friendTroops[i].UnitHp = _friendTroops[i].Unit.HitPoint;
						int spRecover = Math.Max((int)(_friendTroops[i].Unit.SkillPoint * SP_RECOVERY_RATE), 1);
						_friendTroops[i].UnitSp += spRecover;
						if (_friendTroops[i].UnitSp > _friendTroops[i].Unit.SkillPoint)
							_friendTroops[i].UnitSp = _friendTroops[i].Unit.SkillPoint;
					}
				//}
			}
		}

		private int ComputeScore(Troop troop, int score)
		{
			if (troop != null)
			{
				if (!troop.IsAlive)
					score += troop.MaxNumber;

				if (troop.Unit != null)
					score = troop.Unit.Rank > 0 ? score * troop.Unit.Rank : score / 2;

				if (!troop.IsAlive && troop.Leader != null)
					score += troop.Leader.Level * 5;
			}

			return score;
		}

		private Troop GetTroop (int id)
		{
			if (id < ENEMY_ID_BIAS)
				return _friendTroops[id];
			else
				return _enemyTroops[id - ENEMY_ID_BIAS];
		}

		private int CheckMaxSpeed(List<MovePoint> movePoints)
		{
			int maxSpeed = 0;

			foreach (MovePoint mp in movePoints)
			{
				Troop troop = GetTroop(mp.Id);
				if (troop != null && troop.IsAlive)
					maxSpeed = Math.Max(maxSpeed, mp.Speed);
			}

			return maxSpeed;
		}

		public static int CheckRange(int originPos, int targetPos)
		{
			int range = 0;
			bool isDifferent = false;

			if (originPos >= ENEMY_POSITION_BIAS)
				originPos = 0 - (originPos - ENEMY_POSITION_BIAS);
			if (targetPos >= ENEMY_POSITION_BIAS)
				targetPos = 0 - (targetPos - ENEMY_POSITION_BIAS);

			if (originPos * targetPos < 0)
				isDifferent = true;
			originPos = (Math.Abs(originPos) - 1) / COLUMN_SIZE;
			targetPos = (Math.Abs(targetPos) - 1) / COLUMN_SIZE;
			if (isDifferent)
				targetPos = 0 - targetPos;
			range = Math.Abs(originPos - targetPos) + (isDifferent ? 1 : 0);
			return range;
		}

		public static int CheckRange(Troop origin, Troop target)
		{
			if (origin == null || target == null)
				return 0;
			return CheckRange(origin.Position, target.Position);
		}

		public void Action(int target, int skillId)
		{
			//TODO: Check distance.
			if (_actionId >= 0)
			{
				if (skillId == -1)
				{
					target -= (ENEMY_POSITION_BIAS + 1);

					int enermy = _positionMapping[FIELD_SIZE + target];
					if (enermy != -1)
					{
						enermy -= ENEMY_ID_BIAS;
						if (_enemyTroops[enermy].IsAlive)
						{
							Troop actionTroop = _friendTroops[_actionId];
							string msg = actionTroop.Name + "攻擊" + _enemyTroops[enermy].Name + "。";
							ShowMessage?.Invoke(this, msg);
							int score = Attack(ref actionTroop, ref _enemyTroops[enermy], false, null, 0);

							if (_enemyTroops[enermy].IsAlive &&
								CheckRange(actionTroop, _enemyTroops[enermy]) <= _enemyTroops[enermy].Range)
							{
								msg = _enemyTroops[enermy].Name + "反擊。";
								ShowMessage?.Invoke(this, msg);
								Attack(ref _enemyTroops[enermy], ref actionTroop, true, null, 0);
								_friendTroops[_actionId] = actionTroop;
							}

							score = ComputeScore(_enemyTroops[enermy], score);
							_score += score;
							_point += score;

							_actionId = -1;
						}
					}
				}
				else
				{
					Troop actionTroop = _friendTroops[_actionId];
					Skill skill = Skill.Table[skillId];
					int skillLv = actionTroop.Skills[skillId];
					int score = 0;
					int spCost = skill.SkillPoint + skill.SpGrow * skillLv + Modifier.CheckSpModifier(actionTroop, null, skill);
					spCost = spCost > 0 ? spCost : 0;
					actionTroop.AddSp(0 - spCost);

					string msg = actionTroop.Name + "使用技能" + skill.Name + "Lv" + skillLv;
					ShowMessage?.Invoke(this, msg);

					switch (skill.SkillType)
					{
						case SkillType.Physic:
						case SkillType.Magic:
							target -= (ENEMY_POSITION_BIAS + 1);
							if (skill.Target == SkillTarget.EnemySingle)
							{
								Troop enemyTroop = _enemyTroops[_positionMapping[FIELD_SIZE + target] - ENEMY_ID_BIAS];
								score = Attack(ref actionTroop, ref enemyTroop, false, skill, skillLv);
								score = ComputeScore(enemyTroop, score);
								_score += score;
								_point += score;
							}
							else if (skill.Target == SkillTarget.EnemyAll)
							{
								for (int i = 0; i < MAX_TROOPS; i++)
								{
									if (_enemyTroops[i] != null && _enemyTroops[i].IsAlive)
									{
										score = Attack(ref actionTroop, ref _enemyTroops[i], false, skill, skillLv);
										score = ComputeScore(_enemyTroops[i], score);
										_score += score;
										_point += score;
									}
								}
							}
							break;
						case SkillType.Heal:
							target--;
							if (skill.Target == SkillTarget.Self || skill.Target == SkillTarget.PartnerSingle)
							{
								Troop targetTroop = _friendTroops[_positionMapping[target]];
								Heal(actionTroop, ref targetTroop, skill, skillLv);
							}
							else if (skill.Target == SkillTarget.PartnerAll)
							{
								for (int i = 0; i < FIELD_SIZE; i++)
								{
									if (_positionMapping[i] != -1)
									{
										Troop targetTroop = _friendTroops[_positionMapping[i]];
										if (targetTroop.IsAlive)
											Heal(actionTroop, ref targetTroop, skill, skillLv);
										_friendTroops[_positionMapping[i]] = targetTroop;
									}
								}
							}
							break;
						case SkillType.Buffer:
							target--;
							if (skill.Target == SkillTarget.Self || skill.Target == SkillTarget.PartnerSingle)
							{
								Troop targetTroop = _friendTroops[_positionMapping[target]];
								CastBuffer(actionTroop, ref targetTroop, skill, skillLv);
							}
							else if (skill.Target == SkillTarget.PartnerAll)
							{
								for (int i = 0; i < FIELD_SIZE; i++)
								{
									if (_positionMapping[i] != -1)
									{
										Troop targetTroop = _friendTroops[_positionMapping[i]];
										if (targetTroop.IsAlive)
											CastBuffer(actionTroop, ref targetTroop, skill, skillLv);
										_friendTroops[_positionMapping[i]] = targetTroop;
									}
								}
							}
							break;
						case SkillType.Curse:
							target -= (ENEMY_POSITION_BIAS + 1);
							if (skill.Target == SkillTarget.EnemySingle)
							{
								Troop enemyTroop = _enemyTroops[_positionMapping[FIELD_SIZE + target] - ENEMY_ID_BIAS];
								CastBuffer(actionTroop, ref enemyTroop, skill, skillLv);
							}
							else if (skill.Target == SkillTarget.EnemyAll)
							{
								for (int i = 0; i < MAX_TROOPS; i++)
								{
									if (_enemyTroops[i] != null && _enemyTroops[i].IsAlive)
										CastBuffer(actionTroop, ref _enemyTroops[i], skill, skillLv);
								}
							}
							break;
						//case SkillType.Passive:
						//	break;
						//case SkillType.Startup:
						//	break;
						default:
							break;
					}
					_friendTroops[_actionId] = actionTroop;
					_actionId = -1;
				}
			}
		}

		private int SelectSingleTarget(int enemyId, int range)
		{
			int targetId = -1;
			List<int> remainer = new List<int>();

			for (int j = 0; j < FIELD_SIZE; j++)
			{
				if (_positionMapping[j] != -1 && _friendTroops[_positionMapping[j]].IsAlive &&
					CheckRange(_enemyTroops[enemyId], _friendTroops[_positionMapping[j]]) <= range)
					remainer.Add(_positionMapping[j]);
			}

			if (remainer.Count > 0)
				targetId = remainer[_rand.Next(remainer.Count)];

			return targetId;
		}

		private int SelectSingleEnemy()
		{
			int targetId = -1;
			List<int> remainer = new List<int>();

			for (int j = 0; j < FIELD_SIZE; j++)
			{
				if (_positionMapping[FIELD_SIZE + j] != -1)
				{
					int id = _positionMapping[FIELD_SIZE + j];
					if (_enemyTroops[id - ENEMY_ID_BIAS].IsAlive)
						remainer.Add(id);
				}
			}

			if (remainer.Count > 0)
				targetId = remainer[_rand.Next(remainer.Count)];

			return targetId;
		}

		private int FindHealTarget(int healAmount)
		{
			int targetId = -1;
			List<int> remainer = new List<int>();

			for (int j = 0; j < FIELD_SIZE; j++)
			{
				if (_positionMapping[FIELD_SIZE + j] != -1)
				{
					Troop target = _enemyTroops[_positionMapping[FIELD_SIZE + j] - ENEMY_ID_BIAS];
					// Will not heal leader-only troop!

					if (target.IsAlive && target.Unit != null &&
						((target.TroopNumber - target.CurrentNumber) * target.Unit.HitPoint > healAmount ||
						(target.CurrentNumber * 2 < target.TroopNumber) && ((target.TroopNumber - target.CurrentNumber) * target.Unit.HitPoint * 2 > healAmount) ||
						target.CurrentNumber * 4 < target.TroopNumber))
						remainer.Add(_positionMapping[FIELD_SIZE + j]);
				}
			}

			if (remainer.Count > 0)
				targetId = remainer[_rand.Next(remainer.Count)];

			return targetId;
		}

		private void EnemyAi(int enemyId)
		{
			Troop enemy = _enemyTroops[enemyId];

			AiType ai = enemy.Leader != null ? enemy.Leader.Ai : enemy.Unit != null ? enemy.Unit.Ai : AiType.Default;

			// Default Ai
			// Heal > Buffer > Curse > Physical/Magic

			int troopCount = 0;
			for (int i = 0; i < MAX_TROOPS; i++)
			{
				if (_enemyTroops[i] != null && _enemyTroops[i].IsAlive)
					troopCount++;
			}

			// Check skills
			List<int> healSkills = new List<int>();
			List<int> bufSkills = new List<int>();
			List<int> curseSkills = new List<int>();
			List<int> attackSkills = new List<int>();

			// Check available skills
			foreach (var pair in enemy.Skills)
			{
				int spCost = Skill.Table[pair.Key].SkillPoint + pair.Value * Skill.Table[pair.Key].SpGrow;
				if (spCost < enemy.CurrentSp)
				{
					switch (Skill.Table[pair.Key].SkillType)
					{
						case SkillType.Physic:
						case SkillType.Magic:
							attackSkills.Add(pair.Key);
							break;
						case SkillType.Heal:
							healSkills.Add(pair.Key);
							break;
						case SkillType.Buffer:
							bufSkills.Add(pair.Key);
							break;
						case SkillType.Curse:
							curseSkills.Add(pair.Key);
							break;
						default:
							break;
					}
				}
			}

			bool done = false;

			const int UNEXPECT_ACTION_RATE = 10;
			const int UNEXPECT_ACTION_BASE = 100;

			// Heal
			if (!done && healSkills.Count > 0 && _rand.Next(UNEXPECT_ACTION_BASE) > UNEXPECT_ACTION_RATE)
			{
				int skillId = healSkills[_rand.Next(healSkills.Count)];
				int healAmount = ComputeHealAmount(enemy, null, Skill.Table[skillId], enemy.Skills[skillId]);

				if (Skill.Table[skillId].Target == SkillTarget.Self)
				{
					if ((enemy.Unit != null && ((enemy.TroopNumber - enemy.CurrentNumber) * enemy.Unit.HitPoint > healAmount) ||
						(enemy.CurrentNumber * 2 < enemy.TroopNumber && (enemy.TroopNumber - enemy.CurrentNumber) * enemy.Unit.HitPoint * 2 > healAmount) ||
						enemy.CurrentNumber * 4 < enemy.TroopNumber) ||	(enemy.Leader != null && enemy.Leader.HitPoint - enemy.LeaderHp > healAmount))
					{
						int spCost = Skill.Table[skillId].SkillPoint + Skill.Table[skillId].SpGrow * enemy.Skills[skillId]
							+ Modifier.CheckSpModifier(enemy, null, Skill.Table[skillId]);
						spCost = spCost > 0 ? spCost : 0;
						enemy.AddSp(0 - spCost);

						string msg = enemy.Name + "使用技能" + Skill.Table[skillId].Name + "Lv" + enemy.Skills[skillId];
						ShowMessage?.Invoke(this, msg);

						Heal(enemy, ref enemy, Skill.Table[skillId], enemy.Skills[skillId]);
						done = true;
					}
				}
				else
				{
					int target = FindHealTarget(healAmount);

					if (target != -1)
					{
						target -= ENEMY_ID_BIAS;
						int spCost = Skill.Table[skillId].SkillPoint + Skill.Table[skillId].SpGrow * enemy.Skills[skillId]
							+ Modifier.CheckSpModifier(enemy, null, Skill.Table[skillId]);
						spCost = spCost > 0 ? spCost : 0;
						enemy.AddSp(0 - spCost);

						string msg = enemy.Name + "使用技能" + Skill.Table[skillId].Name + "Lv" + enemy.Skills[skillId];
						ShowMessage?.Invoke(this, msg);

						if (Skill.Table[skillId].Target == SkillTarget.PartnerSingle)
							Heal(enemy, ref _enemyTroops[target], Skill.Table[skillId], enemy.Skills[skillId]);
						else if (Skill.Table[skillId].Target == SkillTarget.PartnerAll)
						{
							for (int i = 0; i < MAX_TROOPS; i++)
							{
								if (_enemyTroops[i] != null && _enemyTroops[i].IsAlive)
										Heal(enemy, ref _enemyTroops[i], Skill.Table[skillId], enemy.Skills[skillId]);
							}
						}

						done = true;
					}
				}
			}

			// Buffer
			if (!done && bufSkills.Count > 0 && _rand.Next(UNEXPECT_ACTION_BASE) > UNEXPECT_ACTION_RATE)
			{
				int skillId = bufSkills[_rand.Next(bufSkills.Count)];
				int targetId = enemyId;

				if (Skill.Table[skillId].Target == SkillTarget.PartnerSingle)
					targetId = SelectSingleEnemy();

				// targetId always return valid enemy (at least caster).
				if (targetId >= ENEMY_ID_BIAS)
					targetId -= ENEMY_ID_BIAS;
				bool already = false;
				foreach (var buf in _enemyTroops[targetId].Buffers)
				{
					if (buf.SkillId == skillId)
					{
						already = true;
						break;
					}
				}
				if (!already)
				{
					int spCost = Skill.Table[skillId].SkillPoint + Skill.Table[skillId].SpGrow * enemy.Skills[skillId]
						+ Modifier.CheckSpModifier(enemy, null, Skill.Table[skillId]);
					spCost = spCost > 0 ? spCost : 0;
					enemy.AddSp(0 - spCost);

					string msg = enemy.Name + "使用技能" + Skill.Table[skillId].Name + "Lv" + enemy.Skills[skillId];
					ShowMessage?.Invoke(this, msg);

					if (Skill.Table[skillId].Target == SkillTarget.Self ||
						Skill.Table[skillId].Target == SkillTarget.PartnerSingle)
						CastBuffer(enemy, ref _enemyTroops[targetId], Skill.Table[skillId], enemy.Skills[skillId]);
					else if (Skill.Table[skillId].Target == SkillTarget.PartnerAll)
					{
						for (int i = 0; i < MAX_TROOPS; i++)
						{
							if (_enemyTroops[i] != null && _enemyTroops[i].IsAlive)
								CastBuffer(enemy, ref _enemyTroops[i],	Skill.Table[skillId], enemy.Skills[skillId]);
						}
					}

					done = true;
				}
			}

			// Curse
			if (!done && curseSkills.Count > 0 && _rand.Next(UNEXPECT_ACTION_BASE) > UNEXPECT_ACTION_RATE)
			{
				int skillId = curseSkills[_rand.Next(curseSkills.Count)];
				int targetId = SelectSingleTarget(enemyId, Skill.Table[skillId].Range);

				if (targetId != -1)
				{
					bool already = false;
					foreach (var buf in _friendTroops[targetId].Buffers)
					{
						if (buf.SkillId == skillId)
						{
							already = true;
							break;
						}
					}
					if (!already)
					{
						int spCost = Skill.Table[skillId].SkillPoint + Skill.Table[skillId].SpGrow * enemy.Skills[skillId]
							+ Modifier.CheckSpModifier(enemy, null, Skill.Table[skillId]);
						spCost = spCost > 0 ? spCost : 0;
						enemy.AddSp(0 - spCost);

						string msg = enemy.Name + "使用技能" + Skill.Table[skillId].Name + "Lv" + enemy.Skills[skillId];
						ShowMessage?.Invoke(this, msg);

						if (Skill.Table[skillId].Target == SkillTarget.EnemySingle)
						{
							Troop targetTroop = _friendTroops[targetId];
							CastBuffer(enemy, ref targetTroop, Skill.Table[skillId], enemy.Skills[skillId]);
							_friendTroops[targetId] = targetTroop;
						}
						else if (Skill.Table[skillId].Target == SkillTarget.EnemyAll)
						{
							for (int i = 0; i < FIELD_SIZE; i++)
							{
								if (_positionMapping[i] != -1)
								{
									Troop targetTroop = _friendTroops[_positionMapping[i]];
									if (targetTroop.IsAlive)
										CastBuffer(enemy, ref targetTroop, Skill.Table[skillId], enemy.Skills[skillId]);
									_friendTroops[_positionMapping[i]] = targetTroop;
								}
							}
						}

						done = true;
					}
				}
			}

			// Skill attack
			if (!done && attackSkills.Count > 0 && _rand.Next(UNEXPECT_ACTION_BASE) > UNEXPECT_ACTION_RATE)
			{
				//TODO: check target before random pick.
				int skillId = attackSkills[_rand.Next(attackSkills.Count)];

				int targetId = SelectSingleTarget(enemyId, Skill.Table[skillId].Range);
				if (targetId != -1)
				{
					int spCost = Skill.Table[skillId].SkillPoint + Skill.Table[skillId].SpGrow * enemy.Skills[skillId]
						+ Modifier.CheckSpModifier(enemy, null, Skill.Table[skillId]);
					spCost = spCost > 0 ? spCost : 0;
					enemy.AddSp(0 - spCost);

					string msg = enemy.Name + "使用技能" + Skill.Table[skillId].Name + "Lv" + enemy.Skills[skillId];
					ShowMessage?.Invoke(this, msg);

					if (Skill.Table[skillId].Target == SkillTarget.EnemySingle)
					{
						Troop targetTroop = _friendTroops[targetId];
						Attack(ref enemy, ref targetTroop, false, Skill.Table[skillId], enemy.Skills[skillId]);
						_friendTroops[targetId] = targetTroop;
					}
					else if (Skill.Table[skillId].Target == SkillTarget.EnemyAll)
					{
						for (int i = 0; i < FIELD_SIZE; i++)
						{
							if (_positionMapping[i] != -1)
							{
								Troop targetTroop = _friendTroops[_positionMapping[i]];
								if (targetTroop.IsAlive)
									Attack(ref enemy, ref targetTroop, false, Skill.Table[skillId], enemy.Skills[skillId]);
								_friendTroops[_positionMapping[i]] = targetTroop;
							}
						}
					}

					done = true;
				}
			}

			// Attack
			if (!done)
			{
				int targetId = SelectSingleTarget(enemyId, enemy.Range);
				if (targetId != -1)
				{
					Troop targetTroop = _friendTroops[targetId];
					int score = 0;
					string msg = enemy.Name + "攻擊" + targetTroop.Name + "。";
					ShowMessage?.Invoke(this, msg);
					Attack(ref enemy, ref targetTroop, false, null, 0);

					if (targetTroop.IsAlive && CheckRange(targetTroop, enemy) <= targetTroop.Range)
					{
						msg = targetTroop.Name + "反擊。";
						ShowMessage?.Invoke(this, msg);
						score = Attack(ref targetTroop, ref enemy, true, null, 0);
					}
					_friendTroops[targetId] = targetTroop;

					score = ComputeScore(enemy, score);
					_score += score;
					_point += score;

					done = true;
				}
			}

			if (!done)
				Defence(ref enemy);

			_enemyTroops[enemyId] = enemy;
		}

		private void CastBuffer(Troop caster, ref Troop targetTroop, Skill skill, int skillLv)
		{
			const int DURATION_THRESHOLD = 100;

			int mattLeader = caster.LeaderHp <= 0 ? 0 : 
				caster.Leader.MagicAttack + Modifier.CheckMagicAttackModifier(caster, true, targetTroop, false, skill);
			int mattUnit = caster.CurrentNumber <= 0 ? 0 :
				caster.Unit.MagicAttack + Modifier.CheckMagicAttackModifier(caster, false, targetTroop, false, skill);
			int durationPlus = Math.Max(mattLeader, mattUnit) / DURATION_THRESHOLD;

			Buffer buf = new Buffer();	// Should be new one, otherwise will get the pointer from Skill.Table.
			buf.SkillId = skill.Id;
			buf.Type = skill.Buffer.Type;
			buf.Power = skill.Buffer.Power + skill.PowerGrow * skillLv;
			buf.Duration = skill.Buffer.Duration + durationPlus;

			string msg = "對" + targetTroop.Name + "施展" + skill.Name + "。";
			ShowMessage?.Invoke(this, msg);

			int i = 0;
			for (; i < targetTroop.Buffers.Count; i++)
			{
				if (targetTroop.Buffers[i].SkillId == buf.SkillId)
				{
					targetTroop.Buffers[i].Power = Math.Max(targetTroop.Buffers[i].Power, buf.Power);
					targetTroop.Buffers[i].Duration = Math.Max(targetTroop.Buffers[i].Duration, buf.Duration);
					break;
				}

			}
			if (i == targetTroop.Buffers.Count)
				targetTroop.Buffers.Add(buf);

			ShowTroop?.Invoke(this, caster);
			ShowTroop?.Invoke(this, targetTroop);
		}

		private int ComputeHealAmount(Troop healer, Troop targetTroop, Skill skill, int skillLv)
		{
			const int LEADER_HEAL_RATIO = 3;
			// Compute heal amount.
			int bonus = 0;
			int amount = 0;

			if (healer.LeaderHp > 0)
			{
				bonus = healer.Leader.MagicAttack + Modifier.CheckMagicAttackModifier(healer, true, targetTroop, false, skill);
				amount += (skill.DamageBonus + skill.DamageGrow * skillLv + bonus * LEADER_HEAL_RATIO) * (100 + bonus) / 100;
			}
			if (healer.CurrentNumber > 0)
			{
				bonus = healer.Unit.MagicAttack + Modifier.CheckMagicAttackModifier(healer, false, targetTroop, false, skill);
				amount += (skill.DamageBonus + skill.DamageGrow * skillLv + bonus * healer.CurrentNumber) * (100 + bonus) / 100;
			}

			return amount;
		}

		private void Heal(Troop healer, ref Troop targetTroop, Skill skill, int skillLv)
		{
			int amount = ComputeHealAmount(healer, targetTroop, skill, skillLv);

			string msg = "回復" + targetTroop.Name + "部隊 " + amount + " 點生命。";
			ShowMessage?.Invoke(this, msg);
			// Heal target.
			if (amount > 0 && targetTroop.Leader != null)
			{
				if (targetTroop.LeaderHp + amount > targetTroop.Leader.HitPoint)
				{
					amount -= (targetTroop.Leader.HitPoint - targetTroop.LeaderHp);
					targetTroop.LeaderHp = targetTroop.Leader.HitPoint;
				}
				else
				{
					targetTroop.LeaderHp += amount;
					amount = 0;
				}
			}
			if (amount > 0 && targetTroop.Unit != null)
			{
				int num = amount / targetTroop.Unit.HitPoint;
				if (num > targetTroop.TroopNumber - targetTroop.CurrentNumber)
				{
					num = targetTroop.TroopNumber - targetTroop.CurrentNumber;
					targetTroop.CurrentNumber = targetTroop.TroopNumber;
					targetTroop.UnitHp = targetTroop.Unit.HitPoint;
				}
				else
				{
					targetTroop.CurrentNumber += num;
					amount -= targetTroop.Unit.HitPoint * num;

					if (targetTroop.UnitHp + amount > targetTroop.Unit.HitPoint)
					{
						num++;
						targetTroop.UnitHp = targetTroop.Unit.HitPoint;
						if (targetTroop.CurrentNumber < targetTroop.TroopNumber)
							targetTroop.CurrentNumber++;
					}
					else
						targetTroop.UnitHp += amount;
				}
				if (num > 0)
				{
					msg = targetTroop.Name + "部隊回復" + num + "單位。";
					ShowMessage?.Invoke(this, msg);
				}
			}
			ShowTroop?.Invoke(this, healer);
			ShowTroop?.Invoke(this, targetTroop);
		}

		public void Defence()
		{
			if (_actionId >= 0)
			{
				Troop actionTroop = _friendTroops[_actionId];
				Defence(ref actionTroop);
				_friendTroops[_actionId] = actionTroop;
				_actionId = -1;
			}
		}

		public static Element CheckElement(Troop troop, bool isLeader, Skill skill)
		{
			Element element = Element.None;
			if (skill != null)
				element = skill.Element;

			if (troop != null)
			{
				if (element == Element.None)
					element = isLeader ? troop.Leader.Element : troop.Unit.Element;
			}

			return element;
		}

		private int ComputeDamage (Troop attacker, Troop defender, bool isCounter, Skill skill, int skillLv, DamagePredict predict)
		{
			int damage = 0;

			const int LEADER_MAGIC_DAMAGE_RATIO = 3;
			int att = 0, def = 0;
			int minDam = 0, maxDam = 0;

			if (attacker == null || defender == null)
				return 0;

			Unit targetUnit = defender.Unit;
			if (defender.CurrentNumber <= 0)
				targetUnit = defender.Leader;
			if (targetUnit == null)
				return 0;

			bool hasLeader = false;
			List<Unit> attackUnit = new List<Unit>();
			if (attacker.Leader != null && attacker.LeaderHp > 0)
			{
				attackUnit.Add(attacker.Leader);
				hasLeader = true;
			}
			if (attacker.Unit != null && attacker.CurrentNumber > 0)
				attackUnit.Add(attacker.Unit);

			for (int i = 0; i < attackUnit.Count; i++)
			{
				bool isLeader = (hasLeader && i == 0);
				att = def = minDam = maxDam = 0;

				//Element element = Element.None;
				//if (skill != null)
				//	element = skill.Element;
				//if (element == Element.None)
				//	element = attackUnit[i].Element;
				if (skill == null || skill.SkillType == SkillType.Physic)
				{
					att = attackUnit[i].Attack + Modifier.CheckAttackModifier(attacker, isLeader, defender, isCounter, skill);
					def = targetUnit.Defence + Modifier.CheckDefenceModifier(attacker, isLeader, defender, isCounter, skill);
					int damMod = Modifier.CheckDamageModifier(attacker, isLeader, defender, false, skill);
					minDam = attackUnit[i].MinDamage + damMod;
					maxDam = attackUnit[i].MaxDamage + damMod;
				}
				else if (skill.SkillType == SkillType.Magic)
				{
					att = attackUnit[i].MagicAttack + Modifier.CheckMagicAttackModifier(attacker, isLeader, defender, isCounter, skill);
					def = targetUnit.MagicDefence + Modifier.CheckMagicDefenceModifier(attacker, isLeader, defender, isCounter, skill);
					minDam = maxDam = att * (isLeader ? LEADER_MAGIC_DAMAGE_RATIO : 1);
				}

				Element element = CheckElement(attacker, isLeader, skill);
				att += Modifier.ElementModifier(element, targetUnit.Element);

				if (skill != null)
				{
					att += skill.AttackBonus + skill.AttackGrow * skillLv;
					minDam += skill.DamageBonus + skill.DamageGrow * skillLv;
					maxDam += skill.DamageBonus + skill.DamageGrow * skillLv;
				}

				damage += InflictDamage(att, def, minDam, maxDam, (isLeader ? 1 : attacker.CurrentNumber), predict);
			}

			return damage;
		}

		private int Attack(ref Troop attacker, ref Troop defender, bool isCounter, Skill skill, int skillLv)
		{
			const int ATTACK_DEAD_RATIO = 20;  // 50%
			const int SKILL_DEAD_RATIO = 50;  // 50%
			int damage = 0;
			int remainDamage = 0;
			string msg = "";

			// Nothing will happen if no leader nor units.
			if (!attacker.IsAlive)
				return 0;
			if (!defender.IsAlive)
				return 0;

			if (skill == null && attacker.Range < CheckRange(attacker, defender))
				return 0;
			if (skill != null && skill.Range < CheckRange(attacker, defender))
				return 0;

			damage = ComputeDamage(attacker, defender, isCounter, skill, skillLv, DamagePredict.Normal);
			msg = "對" + defender.Name + "造成 " + damage + " 點傷害。";
			if (defender.CurrentNumber > 0)
				ShowMessage?.Invoke(this, msg);

			int totalDead = 0;
			if (damage > 0)
			{
				int dead = 0;
				remainDamage = damage;

				if (defender.Unit != null)
				{
					if (defender.CurrentNumber > 0)
						remainDamage = CheckCasualty(defender.CurrentNumber, defender.UnitHp, defender.Unit.HitPoint, damage,
							ref defender.UnitHp, ref dead);

					if (dead > 0)
					{
						int deadRatio = (skill == null) ? ATTACK_DEAD_RATIO : SKILL_DEAD_RATIO;
						defender.TroopNumber -= dead * deadRatio / 100;
						defender.CurrentNumber -= dead;

						msg = defender.Name + "損失 " + dead + " 單位。";
						ShowMessage?.Invoke(this, msg);
					}
					totalDead += dead;
				}

				if (remainDamage > 0)
				{
					// All units dead. Check leader.

					if (defender.LeaderHp > 0)
					{
						float ratio = (float)remainDamage / damage;
						damage = ComputeDamage(attacker, defender, isCounter, skill, skillLv, DamagePredict.Normal);
						damage = (int)(damage * ratio);
						msg = "對" + defender.Name + "造成 " + damage + " 點傷害。";
						ShowMessage?.Invoke(this, msg);

						CheckCasualty(1, defender.LeaderHp, defender.Leader.HitPoint, damage, ref defender.LeaderHp, ref dead); // (void)
						if (dead > 0)
							ShowMessage?.Invoke(this, defender.Name + "部隊全滅！");
						totalDead += dead;
					}
					else
						ShowMessage?.Invoke(this, defender.Name + "部隊全滅！");
				}
			}

			ShowTroop?.Invoke(this, attacker);
			ShowTroop?.Invoke(this, defender);

			return totalDead;
		}

		private int InflictDamage(int att, int def, int minDam, int maxDam, int number, DamagePredict predict)
		{
			const int MIN_RATIO = 1;    // 1%
			const int RATIO_STEP = 1;   // 1%
			int ratio, damage = 0;
			double randomDamage = 0;

			ratio = 100 / RATIO_STEP + att - def;
			ratio = (ratio <= 0) ? MIN_RATIO : ratio * RATIO_STEP;

			switch (predict)
			{
				case DamagePredict.MinimumDamage:
					randomDamage = minDam;
					break;
				case DamagePredict.MaximumDamage:
					randomDamage = maxDam;
					break;
				default:
					randomDamage = minDam + _rand.NextDouble() * (maxDam - minDam);
					break;
			}

			damage += (int)(randomDamage * ratio * number / 100);
			return damage;
		}

		private int CheckCasualty(int number, int currentHp, int maxHp, int damage, ref int remainHp, ref int dead)
		{
			int remainDamage = 0;
			int hit;

			hit = currentHp + maxHp * (number - 1);	// Should -1.
			if (hit <= damage)
			{
				// All dead.
				remainDamage = damage - hit;
				remainHp = 0;
				dead = number;
			}
			else
			{
				dead = damage / maxHp;
				hit = damage - dead * maxHp;
				if (hit >= currentHp)
				{
					dead++;
					remainHp = currentHp + maxHp - hit;
				}
				else
					remainHp = currentHp - hit;
			}
			return remainDamage;
		}

		private void Defence(ref Troop troop)
		{
			const int DEFENCE_BONUS = 25;

			Buffer buf = new Buffer();
			buf.Type = BufferType.DefenceUp;
			buf.Power = DEFENCE_BONUS;
			buf.Duration = 1;
			troop.Buffers.Add(buf);

			string msg = troop.Name + "防禦。";
			ShowMessage?.Invoke(this, msg);
			ShowTroop?.Invoke(this, troop);
		}

		private void Pass(ref Troop troop)
		{
			string msg = troop.Name + "待命。";
			ShowMessage?.Invoke(this, msg);
			ShowTroop?.Invoke(this, troop);
		}

		private void StartUpAction(Troop troop, bool isEnemy)
		{
			foreach (var pair in troop.Skills)
			{
				Skill sk = Skill.Table[pair.Key];
				if (sk.SkillType == SkillType.StartUp)
				{
					string msg = troop.Name + "發動技能" + sk.Name + "Lv" + pair.Value;
					ShowMessage?.Invoke(this, msg);

					if (sk.Target == SkillTarget.Self)
							CastBuffer(troop, ref troop, sk, pair.Value);
					else if (sk.Target == SkillTarget.PartnerAll && isEnemy ||
						sk.Target == SkillTarget.EnemyAll && !isEnemy)
					{
						for (int i = 0; i < MAX_TROOPS; i++)
						{
							if (_enemyTroops[i] != null)
								CastBuffer(troop, ref _enemyTroops[i], sk, pair.Value);
						}
					}
					else if (sk.Target == SkillTarget.PartnerAll && !isEnemy ||
						sk.Target == SkillTarget.EnemyAll && isEnemy)
					{
						for (int i = 0; i < FIELD_SIZE; i++)
						{
							if (_positionMapping[i] != -1)
							{
								Troop targetTroop = _friendTroops[_positionMapping[i]];
								CastBuffer(troop, ref targetTroop, sk, pair.Value);
								_friendTroops[_positionMapping[i]] = targetTroop;
							}
						}
					}
				}
			}
		}


		public void TroopInfo(int positionId)
		{
			Troop troop = null;

			if (positionId > ENEMY_POSITION_BIAS)
			{
				positionId -= (ENEMY_POSITION_BIAS + 1);
				if (_positionMapping[FIELD_SIZE + positionId] != -1)
					troop = _enemyTroops[_positionMapping[FIELD_SIZE + positionId] - ENEMY_ID_BIAS];
			}
			else if (positionId > 0)
			{
				positionId--;
				if (_positionMapping[positionId] != -1)
					troop = _friendTroops[_positionMapping[positionId]];
			}

			ShowTroopInfo?.Invoke(this, troop);
		}

		public void GetSkillResult(Troop attacker, int targetId, int skillId, int skillLv)
		{
			Skill skill = null;
			if (skillId != -1)
				skill = Skill.Table[skillId];

			Troop target = null;
			if (targetId < ENEMY_POSITION_BIAS)
			{
				if (_positionMapping[targetId - 1] != -1)
					target = _friendTroops[_positionMapping[targetId - 1]];
			}
			else if (_positionMapping[FIELD_SIZE + targetId - ENEMY_POSITION_BIAS - 1] != -1)
				target = _enemyTroops[_positionMapping[FIELD_SIZE + targetId - ENEMY_POSITION_BIAS - 1] - ENEMY_ID_BIAS];

			int min = 0, max = 0;
			string msg = "";
			if (target != null)
			{
				if (skillId == -1 || skill.SkillType == SkillType.Physic || skill.SkillType == SkillType.Magic)
				{
					min = ComputeDamage(attacker, target, false, skill, skillLv, DamagePredict.MinimumDamage);
					max = ComputeDamage(attacker, target, false, skill, skillLv, DamagePredict.MaximumDamage);
					msg = "造成 " + min + " - " + max + " 點傷害";
					if (target.Unit != null)
						msg += " (約" + min / target.Unit.HitPoint + "-"+ max / target.Unit.HitPoint+"單位)";
				}
				else if (skill.SkillType == SkillType.Heal)
				{
					min = max = ComputeHealAmount(attacker, target, skill, skillLv);
					msg = "回復 " + min + " 點傷害";
					if (target.Unit != null)
						msg += " (約" + min / target.Unit.HitPoint + "單位)";
				}
			}

			ShowSkillDescription?.Invoke(this, msg);
		}

		private void RemoveTroopFromMp (int id)
		{
			List<MovePoint> mps = new List<MovePoint>();
			for (int i = 0; i < _movePoints.Count; i++)
			{
				if (_movePoints[i].Id != id)
					mps.Add(_movePoints[i]);
			}
			_movePoints = mps;
		}

		public void CallOut (int positionId, int friendId)
		{
			if (_positionMapping[positionId - 1] != -1)
			{
				_friendTroops[_positionMapping[positionId - 1]].Position = -1;
				RemoveTroopFromMp(friendId);
			}
			_positionMapping[positionId - 1] = friendId;

			Troop troop = _friendTroops[friendId];
			_friendTroops[friendId].Position = positionId;

			MovePoint mp = new MovePoint();
			mp.Id = friendId;
			mp.Priority = 0;
			mp.Speed = troop.Speed + Modifier.CheckSpeedModifier(troop);
			mp.Value = troop.Initiative;
			_movePoints.Add(mp);

			//TODO: modify movePoint

			ShowTroop?.Invoke(this, _friendTroops[friendId]);
		}
	}
}
