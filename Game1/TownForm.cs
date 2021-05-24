using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game1
{
	public partial class TownForm : Form
	{
		public class OrderTeam : Comparer<int>
		{
			private List<Troop> _friendTroops = null;

			public OrderTeam(List<Troop> troops)
			{
				_friendTroops = troops;
			}

			public override int Compare(int x, int y)
			{
				int positionX = _friendTroops[x].Position;
				int positionY = _friendTroops[y].Position;

				if (positionX != -1 && positionY != -1)
					return (positionX < positionY) ? -1 : 1;
				else if (positionX == -1)
					return 1;
				else if (positionY == -1)
					return -1;
				return 0;
			}
		}

		public class OrderJob : Comparer<int>
		{
			private List<Troop> _friendTroops = null;

			public OrderJob(List<Troop> troops)
			{
				_friendTroops = troops;
			}

			public override int Compare(int x, int y)
			{
				LeaderJob jobX = _friendTroops[x].Leader.Job;
				LeaderJob jobY = _friendTroops[y].Leader.Job;

				return (jobX < jobY) ? -1 : (jobX == jobY) ? 0 : 1;
			}
		}

		public class OrderLevel : Comparer<int>
		{
			private List<Troop> _friendTroops = null;

			public OrderLevel(List<Troop> troops)
			{
				_friendTroops = troops;
			}

			public override int Compare(int x, int y)
			{
				int lvX = _friendTroops[x].Leader.Level;
				int lvY = _friendTroops[y].Leader.Level;

				return (lvX < lvY) ? 1 : (lvX == lvY) ? 0 : -1;
			}
		}

		public class OrderUnit : Comparer<int>
		{
			private List<Troop> _friendTroops = null;

			public OrderUnit(List<Troop> troops)
			{
				_friendTroops = troops;
			}

			public override int Compare(int x, int y)
			{
				UnitType typeX = _friendTroops[x].Unit.Type;
				UnitType typeY = _friendTroops[y].Unit.Type;

				if (typeX < typeY)
					return -1;
				else if (typeX > typeY)
					return 1;

				int rankX = _friendTroops[x].Unit.Rank;
				int rankY = _friendTroops[y].Unit.Rank;

				if (rankX < rankY)
					return 1;
				else if (rankX > rankY)
					return -1;

				int unitIdX = _friendTroops[x].Unit.Id;
				int unitIdY = _friendTroops[y].Unit.Id;

				if (unitIdX < unitIdY)
					return -1;
				else if (unitIdX > unitIdY)
					return 1;

				return 0;
			}
		}

		private const int GROUP_NUMBER = 5;
		private const int POOL_SIZE = 5;

		static private List<int> _poolHeroId = new List<int> { 3 };
		static private int _orderType = 0;

		//private Troop[] _friendTroops = null;
		private List<Troop> _friendTroops = null;
		private List<Troop> _troopPool = null;
		private int[] _positionMapping = null;
		private List<int[]> _allMappings = null;
		private int _currentGroup = 0;
		private int _battleCount = 0;
		private int _points = 0;

		public int Points { get { return _points; } }

		public TownForm()
		{
			InitializeComponent();
		}

		private int GenerateUnitId()
		{
			List<int> ids = new List<int>();

			if (_battleCount <= 5)
				ids.AddRange(new int[] { 1, 1001});
			if (_battleCount >= 5 && _battleCount <= 10)
				ids.AddRange(new int[] { 1, 2, 3, 4, 5 });
			if (_battleCount >= 10 && _battleCount <= 15)
				ids.AddRange(new int[] { 1001, 1002, 1003, 1005, 1006 });
			if (_battleCount >= 15 && _battleCount <= 20)
			{
				ids.AddRange(new int[] { 1, 2, 3, 4, 5 });
				ids.AddRange(new int[] { 1001, 1002, 1003, 1005, 1006 });
			}
			//if (_battleCount >= 40 && _battleCount <= 50)
			//	ids.AddRange(new int[] { 2, 3, 4, 5, 7, 8 });
			//if (_battleCount >= 50 && _battleCount <= 60)
			//	ids.AddRange(new int[] { 1002, 1003, 1005, 1006, 1004, 1007 });
			//if (_battleCount >= 60 && _battleCount <= 70)
			//{
			//	ids.AddRange(new int[] { 2, 3, 4, 5, 7, 8 });
			//	ids.AddRange(new int[] { 1002, 1003, 1005, 1006, 1004, 1007 });
			//}
			if (_battleCount >= 20)
			{
				ids.AddRange(new int[] { 4, 5, 7, 8 });
				ids.AddRange(new int[] { 1005, 1006, 1004, 1007 });
			}

			if (ids.Count <= 0)
				return 1;
			else
			{
				Random rand = new Random();
				return ids[rand.Next(ids.Count)];
			}
		}

		private void GeneratePool()
		{
			panelHire.Controls.Clear();
			_troopPool = new List<Troop>();
			Troop troop = null;
			LeaderPanel panel = null;
			Random rand = new Random();

			int count = 0;
			if (_poolHeroId.Count > 0)
			{
				int id = _poolHeroId[rand.Next(_poolHeroId.Count)];

				troop = new Troop();
				troop.Leader = Leader.LoadWithId(id);
				troop.Unit = Unit.LoadWithId(troop.Leader.UnitId[0]);

				troop.LeaderHp = troop.Leader.HitPoint;
				troop.CurrentNumber = troop.TroopNumber = troop.MaxNumber = troop.Leader.UnitSizeBase;
				troop.UnitHp = troop.Unit.HitPoint;
				troop.LeaderSp = troop.Leader.SkillPoint;
				troop.UnitSp = troop.Unit.SkillPoint;
				troop.Position = -1;

				panel = new LeaderPanel();
				if (troop.Leader.Id == 3)		// Special leader(s).
					panel.BaseHirePoint = 0;
				panel.Setup(troop.Leader.Id, _battleCount, _points, true, troop);
				panel.OnPointUpdated += UpdatePoint;
				panel.OnInfo += ShowTroopInfo;
				panel.OnHire += HireTroop;
				panel.IsHire = false;
				panel.Troop = troop;

				panelHire.Controls.Add(panel);
				count++;
			}

			for (int i = count; i < POOL_SIZE; i++)
			{
				troop = new Troop();
				//troop.Leader = LeaderGenerator.Generate(false);
				int unitId = GenerateUnitId();
				troop.Unit = Unit.LoadWithId(unitId);
				troop.Leader = LeaderGenerator.GenerateLeader(troop.Unit, false);

				int leaderLv = _battleCount / 2 + rand.Next(_battleCount / 2 + 1);
				troop.Leader.LevelUp(leaderLv);

				troop.LeaderHp = troop.Leader.HitPoint;
				troop.CurrentNumber = troop.TroopNumber = troop.MaxNumber = troop.Leader.UnitSizeBase;

				//if (troop.Leader.UnitId[0] != -1)
				{
					//troop.Unit = Unit.LoadWithId(troop.Leader.UnitId[0]);

					int unitLv = _battleCount / 20 + rand.Next(_battleCount / 20 + 1);
					troop.Unit.LevelUp(unitLv);

					troop.UnitHp = troop.Unit.HitPoint;
					troop.UnitSp = troop.Unit.SkillPoint;
				}

				troop.LeaderSp = troop.Leader.SkillPoint;
				troop.Position = -1;

				int[] basePoints = { 25, 100, 400, 1600 };
				panel = new LeaderPanel();
				if (troop.Leader.Id == 4)       // Special leader(s).
					panel.BaseHirePoint = 0;
				else
					panel.BaseHirePoint = basePoints[troop.Unit.Rank];
				panel.Setup(troop.Leader.Id, _battleCount, _points, true, troop);
				panel.OnPointUpdated += UpdatePoint;
				panel.OnInfo += ShowTroopInfo;
				panel.OnHire += HireTroop;
				panel.IsHire = false;
				panel.OnDisband += DisbandTroop;
				panel.SetHire(_friendTroops.Count < BattleController.MAX_LEADERS);
				panel.Troop = troop;

				panelHire.Controls.Add(panel);
			}
		}

		//internal void Setup(ref Troop[] troops, ref int[] mappings, ref int points)
		public void Setup(ref List<Troop> troops, ref List<int[]> mappings, ref int currentGroup, ref int points, int battle)
		{
			_battleCount = battle;
			_friendTroops = troops;
			_allMappings = mappings;
			_positionMapping = _allMappings[currentGroup];
			_currentGroup = currentGroup;
			_points = points;

			buttonReturn.Text += "(" + battle + ")";
			labelNumLeader.Text = "將領數: " + _friendTroops.Count + "/" + BattleController.MAX_LEADERS;

			GeneratePool();
			UpdateComboBox();
			comboBoxOrder.SelectedIndex = _orderType;
		}

		public void UpdatePoint(object sender, int point)
		{
			_points = point;
			labelPoint.Text = "獎勵點數: " + Points;

			foreach (Control control in panelTroops.Controls)
			{
				LeaderPanel panel = (LeaderPanel)control;
				panel.Point = Points;
				panel.Troop = panel.Troop;	// Force update.
			}

			foreach (Control control in panelHire.Controls)
			{
				LeaderPanel panel = (LeaderPanel)control;
				panel.Point = Points;
				panel.Troop = panel.Troop;  // Force update.
			}
		}

		public void UpdateComboBox()
		{
			ComboBox[] combos = { comboBoxA1, comboBoxA2, comboBoxA3, comboBoxA4, comboBoxA5,
					comboBoxA6, comboBoxA7, comboBoxA8, comboBoxA9 };

			for (int j = 0; j < combos.Length; j++)
				combos[j].Items.Clear();
			for (int i = 0; i < _friendTroops.Count; i++)
			{
				if (_friendTroops[i] != null)
				{
					TroopItem item = new TroopItem();
					item.Index = i;
					item.Name = _friendTroops[i].Leader.Name;
					item.Level = _friendTroops[i].Leader.Level;
					item.UnitName = _friendTroops[i].Unit.Name;
					item.CurrentNumber = _friendTroops[i].CurrentNumber;
					item.TroopNumber = _friendTroops[i].TroopNumber;

					for (int j = 0; j < combos.Length; j++)
						combos[j].Items.Add(item.Name);
				}
			}

			UpdateData(this, null);
		}

		public void UpdateData(object sender, EventArgs e)
		{
			if (textBoxTroopInfo.InvokeRequired)
				textBoxTroopInfo.Invoke(new EventHandler(UpdateData), sender, e);
			else
			{
				Button[] infoButtons = { buttonInfoA1, buttonInfoA2, buttonInfoA3, buttonInfoA4, buttonInfoA5,
					buttonInfoA6, buttonInfoA7, buttonInfoA8, buttonInfoA9 };
				ComboBox[] combos = { comboBoxA1, comboBoxA2, comboBoxA3, comboBoxA4, comboBoxA5,
					comboBoxA6, comboBoxA7, comboBoxA8, comboBoxA9 };

				for (int i = 0; i < BattleController.FIELD_SIZE; i++)
				{
					if (_positionMapping[i] != -1)
						infoButtons[i].Visible = true;
					else
						infoButtons[i].Visible = false;

					//if (_positionMapping[i] != -1)
					//	ShowTroop(this, _friendTroops[_positionMapping[i]]);

					if (_positionMapping[i] != -1)
						combos[i].SelectedIndex = _positionMapping[i];
				}

				labelPoint.Text = "獎勵點數: " + Points;
			}
		}

		private void buttonReturn_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void buttonInfo_Click(object sender, EventArgs e)
		{
			Button[] sideA = { buttonInfoA1, buttonInfoA2, buttonInfoA3, buttonInfoA4, buttonInfoA5,
				buttonInfoA6, buttonInfoA7, buttonInfoA8, buttonInfoA9 };

			textBoxTroopInfo.Text = "";
			for (int i = 0; i < BattleController.FIELD_SIZE; i++)
			{
				if (sender == sideA[i])
				{
					TroopInfo(i + 1);
					break;
				}
			}
		}

		//private void ShowTroop(object obj, Troop troop)
		//{
		//	Label[] sideA = { labelA1, labelA2, labelA3, labelA4, labelA5, labelA6, labelA7, labelA8, labelA9 };
		//	Label targetLabel = null;
		//	int position = troop.Position;
		//	if (position > 0)
		//	{
		//		targetLabel = sideA[position - 1];

		//		string name = "";
		//		if (troop.Leader != null)
		//			name += troop.Leader.Name;
		//		if (troop.Unit != null)
		//			name += "(" + troop.Unit.Name + ")";

		//		if (troop.CurrentNumber == 0 && troop.Leader != null)
		//			targetLabel.Text = name + "\nHP:" + troop.LeaderHp + "/" + troop.Leader.HitPoint;
		//		else
		//			targetLabel.Text = name + "\n" + troop.CurrentNumber + "/" + troop.TroopNumber;
		//	}
		//}

		public void TroopInfo(int positionId)
		{
			Troop troop = null;

			positionId--;
			if (_positionMapping[positionId] != -1)
				troop = _friendTroops[_positionMapping[positionId]];

			ShowTroopInfo(this, troop);
		}

		private void ShowTroopInfo(object obj, Troop troop)
		{
			textBoxTroopInfo.Text = "";
			if (troop != null)
				textBoxTroopInfo.Text = troop.ToString();
		}

		private void HireTroop(object sender, Troop troop)
		{
			LeaderPanel panel = (LeaderPanel)sender;
			//panel.OnHire = null;

			if (_poolHeroId.Contains(troop.Leader.Id))
				_poolHeroId.Remove(troop.Leader.Id);

			_friendTroops.Add(troop);
			panelHire.Controls.Remove(panel);
			panelTroops.Controls.Add(panel);

			labelNumLeader.Text = "將領數: " + _friendTroops.Count + "/" + BattleController.MAX_LEADERS;
			UpdateComboBox();

			if (_friendTroops.Count >= BattleController.MAX_LEADERS)
			{
				foreach (Control control in panelHire.Controls)
				{
					LeaderPanel leader = (LeaderPanel)control;
					leader.SetHire(false);
				}
			}

			comboBoxOrder_SelectedIndexChanged(comboBoxOrder, null);	// Refresh.
		}

		private void DisbandTroop(object sender, Troop troop)
		{
			LeaderPanel panel = (LeaderPanel)sender;
			//panel.OnDisband = null;

			if (troop.Position == -1)
			{
				//TODO: check if exists in other group.
				int index = -1;
				for (int i = 0; i < _friendTroops.Count; i++)
				{
					if (_friendTroops[i] == troop)
					{
						index = i;
						break;
					}
				}

				_friendTroops.Remove(troop);
				panel.IsHire = false;
				panelHire.Controls.Add(panel);
				panelTroops.Controls.Remove(panel);

				//TODO: other groups should also change.
				for (int i = 0; i < BattleController.FIELD_SIZE; i++)
				{
					if (_positionMapping[i] >= index)
						_positionMapping[i]--;
				}

				labelNumLeader.Text = "將領數: " + _friendTroops.Count + "/" + BattleController.MAX_LEADERS;
				UpdateComboBox();

				if (_friendTroops.Count == BattleController.MAX_LEADERS - 1)
				{
					foreach (Control control in panelHire.Controls)
					{
						LeaderPanel leader = (LeaderPanel)control;
						leader.SetHire(true);
					}
				}
			}
			else
				textBoxMessage.AppendText("無法解雇目前隊伍中的將領!\r\n");
		}

		private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox[] combos = { comboBoxA1, comboBoxA2, comboBoxA3, comboBoxA4, comboBoxA5,
					comboBoxA6, comboBoxA7, comboBoxA8, comboBoxA9 };
			ComboBox thisOne = (ComboBox)sender;
			int position = 0;

			int id = thisOne.SelectedIndex;

			if (id >= 0)
			{
				for (int i = 0; i < combos.Length; i++)
				{
					if (combos[i] == thisOne)
						position = i + 1;
					else if (combos[i].SelectedIndex == id)
						combos[i].SelectedIndex = -1;
				}

				for (int i = 0; i < BattleController.FIELD_SIZE; i++)
				{
					if (_positionMapping[i] == id)
					{
						_positionMapping[i] = -1;
						break;
					}
				}
				if (_positionMapping[position - 1] != -1)
					_friendTroops[_positionMapping[position - 1]].Position = -1;
				_positionMapping[position - 1] = id;
				_friendTroops[id].Position = position;

				UpdateData(this, null);
			}
		}

		private void TownForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int count = 0;
			for (int i = 0; i < BattleController.FIELD_SIZE; i++)
			{
				if (_positionMapping[i] != -1)
					count++;
			}

			if (count > BattleController.MAX_TROOPS)
			{
				textBoxMessage.AppendText("隊伍中最多" + BattleController.MAX_TROOPS + "個部隊同時上場!\r\n");
				e.Cancel = true;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			button1.Enabled = false;
			GeneratePool();
		}

		private void comboBoxOrder_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<int> orderList = new List<int>();

			for (int i = 0; i < _friendTroops.Count; i++)
				orderList.Add(i);

			_orderType = comboBoxOrder.SelectedIndex;
			switch (_orderType)
			{
				case 0:
					break;
				case 1:
					OrderTeam team = new OrderTeam(_friendTroops);
					orderList.Sort(team);
					break;
				case 2:
					OrderJob job = new OrderJob(_friendTroops);
					orderList.Sort(job);
					break;
				case 3:
					OrderLevel lv = new OrderLevel(_friendTroops);
					orderList.Sort(lv);
					break;
				case 4:
					OrderUnit unit = new OrderUnit(_friendTroops);
					orderList.Sort(unit);
					break;
				default:
					break;
			}

			panelTroops.Controls.Clear();
			for (int i = 0; i < _friendTroops.Count; i++)
			{
				//if (_friendTroops[i] != null)
				{
					LeaderPanel panel = new LeaderPanel();
					panel.Setup(_friendTroops[orderList[i]].Leader.Id, _battleCount, _points, true, _friendTroops[orderList[i]]);
					panel.OnPointUpdated += UpdatePoint;
					panel.OnInfo += ShowTroopInfo;
					panel.OnHire += HireTroop;
					panel.OnDisband += DisbandTroop;
					panelTroops.Controls.Add(panel);
				}
			}
		}

		private void OnGroupSelected(object sender, EventArgs e)
		{
			//RadioButton[] buttons = { radioButtonGroup1 }
		}
	}
}
