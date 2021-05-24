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
	public struct TroopItem
	{
		public int Index;
		public string Name;
		public int Level;
		public string UnitName;
		public int CurrentNumber;
		public int TroopNumber;

		public override string ToString()
		{
			return Name + " Lv" + Level + " (" + UnitName + " " + CurrentNumber + "/" + TroopNumber + ")";
		}
	}

    public partial class BattleForm : Form
    {
		BattleController bc = new BattleController();
		const int ACTION_CALL = -2;

		private List<Troop> _friendTroops = null;
		private Troop[] _enemyTroops = null;
		private int[] _positionMappings = null;

		private Troop _actionTroop = null;
		private int _actionSkill = -1;

		public BattleForm()
        {
            InitializeComponent();

			bc.ShowMessage += new EventHandler<string>(ShowMessage);
			bc.ShowTroop += new EventHandler<Troop>(ShowTroop);
			bc.ShowBattleCount += new EventHandler<int>(ShowBattleCount);
			bc.ShowScore += new EventHandler<int>(ShowScore);
			bc.ShowPoint += new EventHandler<int>(ShowPoint);
			bc.ShowEnable += new EventHandler<int>(ShowEnable);
			bc.ClearBattleField += new EventHandler<bool>(ClearBattleField);
			bc.ShowTroopInfo += new EventHandler<Troop>(ShowTroopInfo);
			bc.ShowMapping += new EventHandler<int[]>(ShowMapping);
			bc.ShowFriends += new EventHandler<List<Troop>>(ShowFriends);
			bc.ShowEnermies += new EventHandler<Troop[]>(ShowEnermies);
			bc.ShowMovePoints += new EventHandler<List<MovePoint>>(ShowMovePoints);
			bc.ShowOrder += new EventHandler<List<int>>(ShowOrder);
			bc.ShowForm += new EventHandler<bool>(ShowForm);
			bc.ShowSkillSelect += new EventHandler<Troop>(ShowSkillSelect);
			bc.ShowSkillDescription += new EventHandler<string>(ShowPredictDamage);
			bc.ShowGameOver += new EventHandler(ShowGameOver);
			bc.ShowWave += new EventHandler<int[]>(ShowWave);

			//bc.LoadFrienTroops();
			bc.StartBattle();
        }

		private void ShowForm(object sender, bool isVisible)
		{
			if (this.InvokeRequired)
				this.Invoke(new EventHandler<bool>(ShowForm), sender, isVisible);
			else
				this.Visible = isVisible;
		}

		private void ShowBattleCount(object sender, int count)
		{
			if (labelBattle.InvokeRequired)
				labelBattle.Invoke(new EventHandler<int>(ShowBattleCount), sender, count);
			else
				labelBattle.Text = "第 "+ count + " 戰";
		}

		private void ShowScore(object sender, int score)
		{
			if (labelScore.InvokeRequired)
				labelScore.Invoke(new EventHandler<int>(ShowScore), sender, score);
			else
				labelScore.Text = "得分: " + score;
		}

		private void ShowPoint(object sender, int point)
		{
			if (labelPoint.InvokeRequired)
				labelPoint.Invoke(new EventHandler<int>(ShowPoint), sender, point);
			else
				labelPoint.Text = "獎勵點數: " + point;
		}

		private void ShowFriendSide(bool show, bool showEmpty)
		{
			Button[] targets = { buttonA1, buttonA2, buttonA3, buttonA4, buttonA5, buttonA6, buttonA7, buttonA8, buttonA9 };
			for (int i = 0; i < targets.Length; i++)
				//targets[i].Visible = show && (showEmpty || (_positionMappings[i] != -1 && _friendTroops[_positionMappings[i]].IsAlive));
				targets[i].Visible = (show && (_positionMappings[i] != -1 && _friendTroops[_positionMappings[i]].IsAlive)) ||
					(showEmpty && (_positionMappings[i] == -1 || !_friendTroops[_positionMappings[i]].IsAlive));
		}

		private void ShowEnemySide(bool show, bool showEmpty, int range)
		{
			Button[] targets = { buttonB1, buttonB2, buttonB3, buttonB4, buttonB5, buttonB6, buttonB7, buttonB8, buttonB9 };
			for (int i = 0; i < targets.Length; i++)
				targets[i].Visible = show && (showEmpty || (_positionMappings[BattleController.FIELD_SIZE + i] != -1) &&
					_enemyTroops[_positionMappings[BattleController.FIELD_SIZE + i] - BattleController.ENEMY_ID_BIAS].IsAlive &&
					BattleController.CheckRange(_actionTroop.Position, BattleController.ENEMY_POSITION_BIAS + i + 1) <= range);
		}

		private void ShowEnable(object sender, int position)
		{
			Button[] targets = { buttonB1, buttonB2, buttonB3, buttonB4, buttonB5, buttonB6, buttonB7, buttonB8, buttonB9 };

			if (position > BattleController.ENEMY_POSITION_BIAS)
				position -= BattleController.ENEMY_POSITION_BIAS;

			if (targets[position - 1].InvokeRequired)
				targets[position - 1].Invoke(new EventHandler<int>(ShowEnable), sender, position);
			else
				targets[position - 1].Visible = true;
		}

		private int CheckAlive()
		{
			int num = 0;
			for (int i = 0; i < BattleController.FIELD_SIZE; i++)
			{
				if (_positionMappings[i] != -1 && _friendTroops[_positionMappings[i]].IsAlive)
					num++;
			}

			return num;
		}

		private bool HasReserve()
		{
			bool has = false;

			for (int i = 0; i < _friendTroops.Count; i++)
			{
				if (_friendTroops[i].Position == -1 && _friendTroops[i].IsAlive)
				{
					has = true;
					break;
				}
			}

			return has;
		}

		private void ShowSkillSelect(object sender, Troop troop)
		{
			if (groupBox1.InvokeRequired)
				groupBox1.Invoke(new EventHandler<Troop>(ShowSkillSelect), sender, troop);
			else
			{
				_actionTroop = troop;
				RadioButton[] buttons = { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5,
					radioButton6,radioButton7,radioButton8,radioButton9 };

				int count = 1;
				foreach (var pair in troop.Skills)
				{
					int spCost = Skill.Table[pair.Key].SkillPoint + Skill.Table[pair.Key].SpGrow * pair.Value;
					spCost += Modifier.CheckSpModifier(troop, null, Skill.Table[pair.Key]);
					spCost = spCost > 0 ? spCost : 0;

					buttons[count].Text = "(" + Skill.Table[pair.Key].SkillType + ") " + Skill.Table[pair.Key].Name +
						"Lv" + pair.Value + " (" + spCost + ")";
					buttons[count].Visible = true;

					if (Skill.Table[pair.Key].SkillType == SkillType.Passive || Skill.Table[pair.Key].SkillType == SkillType.StartUp
						|| spCost > troop.CurrentSp)
						buttons[count].Enabled = false;
					else
						buttons[count].Enabled = true;
					count++;
				}
				for (int i = count; i < buttons.Length; i++)
					buttons[i].Visible = false;

				_actionSkill = -1;
				labelSkillDescription.Text = "普通攻擊";
				labelPredict.Text = "";
				radioButton1.Checked = true;

				BuildCallList(this, null);
				radioButton10.Enabled = (CheckAlive() < BattleController.MAX_TROOPS) & HasReserve();
				radioButton10.Checked = false;
				radioButton10.Text = "救援 (" + comboBoxCall.Items.Count + ")";
				comboBoxCall.Enabled = buttonInfoC1.Enabled = radioButton10.Enabled;

				ShowSelectButtons(SkillTarget.EnemySingle, troop.Range);
			}
		}

		private void ClearBattleField(object obj, bool isFriendSide)
		{
			if (labelA1.InvokeRequired)
				labelA1.Invoke(new EventHandler<bool>(ClearBattleField), obj, isFriendSide);
			else
			{
				Label[] sideA = { labelA1, labelA2, labelA3, labelA4, labelA5, labelA6, labelA7, labelA8, labelA9 };
				Label[] sideB = { labelB1, labelB2, labelB3, labelB4, labelB5, labelB6, labelB7, labelB8, labelB9 };
				Label[] targetLabel = isFriendSide ? sideA : sideB;

				foreach(Label l in targetLabel)
					l.Text = "";
			}
		}

		private void ShowTroop(object obj, Troop troop)
		{
			if (labelA1.InvokeRequired)
				labelA1.Invoke(new EventHandler<Troop>(ShowTroop), obj, troop);
			else
			{

				Label[] sideA = { labelA1, labelA2, labelA3, labelA4, labelA5, labelA6, labelA7, labelA8, labelA9 };
				Label[] sideB = { labelB1, labelB2, labelB3, labelB4, labelB5, labelB6, labelB7, labelB8, labelB9 };
				Label targetLabel = null;
				//switch (position)
				//{
				//	case 0:
				//		targetLabel = labelA1;
				//		break;
				//	case 1:
				//		targetLabel = labelB1;
				//		break;
				//	default:
				//		break;
				//}
				int position = troop.Position;
				if (position > 0)
				{
					if (position > BattleController.ENEMY_POSITION_BIAS)
						targetLabel = sideB[position - BattleController.ENEMY_POSITION_BIAS - 1];
					else
						targetLabel = sideA[position - 1];

					string name = "";
					if (troop.Leader != null)
						name += " " + troop.Leader.Name;
					if (troop.Unit != null)
					{
						if (troop.Leader != null)
							name += "\r\n";
						name += "(" + troop.Unit.Name + ")";
					}

					//if (targetLabel != null)
					//	targetLabel.Text = troop.CurrentNumber + "/" + troop.TroopNumber + "/" + troop.MaxNumber;
					if (troop.CurrentNumber == 0 && troop.Leader != null)
						targetLabel.Text = name + "\nHP:" + troop.LeaderHp + "/" + troop.Leader.HitPoint;
					else
						targetLabel.Text = name + "\n" + troop.CurrentNumber + "/" + troop.TroopNumber;
					targetLabel.Text += "\nSP:" + troop.CurrentSp + "/" + troop.MaxSp;
				}
			}
		}

		private void ShowTroopInfo(object obj, Troop troop)
		{
			if (textBoxTroopInfo.InvokeRequired)
				textBoxTroopInfo.Invoke(new EventHandler<Troop>(ShowTroopInfo), obj, troop);
			else
			{
				textBoxTroopInfo.Text = "";
				if (troop != null)
					textBoxTroopInfo.Text = troop.ToString();
			}
		}

		private void ShowMapping(object obj, int[] mappings)
		{
			if (buttonInfoA1.InvokeRequired)
				buttonInfoA1.Invoke(new EventHandler<int[]>(ShowMapping), obj, mappings);
			else
			{
				_positionMappings = mappings;
				Button[] buttons = { buttonInfoA1, buttonInfoA2, buttonInfoA3, buttonInfoA4, buttonInfoA5,
					buttonInfoA6, buttonInfoA7, buttonInfoA8, buttonInfoA9,
					buttonInfoB1, buttonInfoB2, buttonInfoB3, buttonInfoB4, buttonInfoB5,
					buttonInfoB6, buttonInfoB7, buttonInfoB8, buttonInfoB9 };
				for (int i = 0; i < BattleController.FIELD_SIZE * 2; i++)
				{
					if (mappings[i] != -1)
						buttons[i].Visible = true;
					else
						buttons[i].Visible = false;
				}
			}
		}

		private void ShowMessage(object obj, string text)
		{
			if (textBoxMessage.InvokeRequired)
				textBoxMessage.Invoke(new EventHandler<string>(ShowMessage), obj, text);
			else
				textBoxMessage.AppendText(text + "\r\n");
		}

		private void Action(object sender, EventArgs e)
		{
			Button[] targetA = { buttonA1, buttonA2, buttonA3, buttonA4, buttonA5, buttonA6, buttonA7, buttonA8, buttonA9 };
			Button[] targetB = { buttonB1, buttonB2, buttonB3, buttonB4, buttonB5, buttonB6, buttonB7, buttonB8, buttonB9 };

			for (int i = 0; i < BattleController.FIELD_SIZE; i++)
			{
				if (sender == targetA[i])
				{
					if (_actionSkill == ACTION_CALL)
					{
						bc.CallOut(i + 1, ((TroopItem)comboBoxCall.SelectedItem).Index);

						ShowFriendSide(false, false);
						ShowSkillSelect(this, _actionTroop);
						ShowMapping(this, _positionMappings);
					}
					else
						bc.Action(i + 1, _actionSkill);
				}
				else if (sender == targetB[i])
					bc.Action(i + BattleController.ENEMY_POSITION_BIAS + 1, _actionSkill);
			}
		}

		private void BattleForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Environment.Exit(0);
		}

		private void buttonDefence_Click(object sender, EventArgs e)
		{
			bc.Defence();
		}

		private void buttonInfo_Click(object sender, EventArgs e)
		{
			Button[] sideA = { buttonInfoA1, buttonInfoA2, buttonInfoA3, buttonInfoA4, buttonInfoA5,
				buttonInfoA6, buttonInfoA7, buttonInfoA8, buttonInfoA9 };
			Button[] sideB = { buttonInfoB1, buttonInfoB2, buttonInfoB3, buttonInfoB4, buttonInfoB5,
				buttonInfoB6, buttonInfoB7, buttonInfoB8, buttonInfoB9 };

			textBoxTroopInfo.Text = "";
			for (int i = 0; i < BattleController.FIELD_SIZE; i++)
			{
				if (sender == sideA[i])
				{
					bc.TroopInfo(i + 1);
					break;
				}
				else if (sender == sideB[i])
				{
					bc.TroopInfo(i + BattleController.ENEMY_POSITION_BIAS + 1);
					break;
				}
			}

			if (sender == buttonInfoC1)
			{
				int index = ((TroopItem)comboBoxCall.SelectedItem).Index;
				ShowTroopInfo(this, _friendTroops[index]);
			}
		}

		private void BuildCallList(object obj, EventArgs e)
		{
			if (comboBoxCall.InvokeRequired)
				comboBoxCall.Invoke(new EventHandler(BuildCallList), obj);
			else
			{
				comboBoxCall.Items.Clear();
				for (int i = 0; i < _friendTroops.Count; i++)
				{
					if (_friendTroops[i].Position == -1 && _friendTroops[i].IsAlive)
					{
						TroopItem item = new TroopItem();
						item.Index = i;
						item.Name = _friendTroops[i].Leader.Name;
						item.Level = _friendTroops[i].Leader.Level;
						item.UnitName = _friendTroops[i].Unit.Name;
						item.CurrentNumber = _friendTroops[i].CurrentNumber;
						item.TroopNumber = _friendTroops[i].TroopNumber;

						comboBoxCall.Items.Add(item);
					}
				}

				if (comboBoxCall.Items.Count > 0)
					comboBoxCall.SelectedIndex = 0;
			}
		}

		private void ShowFriends(object obj, List<Troop> troop)
		{
			_friendTroops = troop;
		}

		private void ShowEnermies(object obj, Troop[] troop)
		{
			_enemyTroops = troop;
		}

		private void ShowMovePoints(object obj, List<MovePoint> movePoints)
		{
			if (textBoxMp.InvokeRequired)
				textBoxMp.Invoke(new EventHandler<List<MovePoint>>(ShowMovePoints), obj, movePoints);
			else
			{
				textBoxMp.Text = "";
				if (_friendTroops!= null && _enemyTroops != null)
				{
					string str = "";

					foreach (MovePoint mp in movePoints)
					{
						int id = mp.Id;
						Troop troop = null;
						if (id < BattleController.ENEMY_ID_BIAS)
							troop = _friendTroops[id];
						else
							troop = _enemyTroops[id - BattleController.ENEMY_ID_BIAS];

						str += troop.Name;
						if (troop.IsAlive)
							str += "[" + mp.Priority + "]: ";
						else
							str += "[全滅]: ";
						str += mp.Value + "(+" + mp.Speed + ")\r\n";
					}

					textBoxMp.Text = str;
				}
			}
		}

		private void ShowOrder(object sender, List<int> ids)
		{
			if (textBoxMp.InvokeRequired)
				textBoxMp.Invoke(new EventHandler<List<int>>(ShowOrder), sender, ids);
			else
			{
				string str = "\r\n======\r\n\r\n";
				for (int i = 0; i < ids.Count; i++)
				{
					Troop troop = null;
					if (ids[i] < BattleController.ENEMY_ID_BIAS)
						troop = _friendTroops[ids[i]];
					else
						troop = _enemyTroops[ids[i] - BattleController.ENEMY_ID_BIAS];

					str += "[" + i + "] " + troop.Name + "\r\n";
				}

				textBoxMp.Text += str;
			}
		}
		private void ShowEmptySlots()
		{
			Button[] friends = { buttonA1, buttonA2, buttonA3, buttonA4, buttonA5, buttonA6, buttonA7, buttonA8, buttonA9 };
			Button[] enemies = { buttonB1, buttonB2, buttonB3, buttonB4, buttonB5, buttonB6, buttonB7, buttonB8, buttonB9 };
			ShowFriendSide(false, true);
			ShowEnemySide(false, false, 0);
		}

		private void ShowSelectButtons(SkillTarget target, int range)
		{
			Button[] friends = { buttonA1, buttonA2, buttonA3, buttonA4, buttonA5, buttonA6, buttonA7, buttonA8, buttonA9 };
			Button[] enemies = { buttonB1, buttonB2, buttonB3, buttonB4, buttonB5, buttonB6, buttonB7, buttonB8, buttonB9 };

			switch (target)
			{
				case SkillTarget.Self:
					ShowFriendSide(false, false);
					ShowEnemySide(false, false, 0);
					friends[_actionTroop.Position - 1].Visible = true;
					break;
				case SkillTarget.PartnerSingle:
					ShowFriendSide(true, false);
					ShowEnemySide(false, false, 0);
					break;
				case SkillTarget.PartnerAll:
					ShowFriendSide(true, true);
					ShowEnemySide(false, false, 0);
					break;
				case SkillTarget.EnemySingle:
					ShowFriendSide(false, false);
					ShowEnemySide(true, false, range);
					break;
				case SkillTarget.EnemyAll:
					ShowFriendSide(false, false);
					ShowEnemySide(true, true, 0);
					break;
				default:
					ShowFriendSide(false, false);
					ShowEnemySide(false, false, 0);
					break;
			}
		}

		private void OnSkillSelected(object sender, EventArgs e)
		{
			RadioButton[] buttons = { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5,
					radioButton6, radioButton7, radioButton8, radioButton9 };

			int i = 0;
			for (; i < buttons.Length; i++)
				if (buttons[i] == sender)
					break;
			_actionSkill = -1;

			SkillTarget target = SkillTarget.EnemySingle;
			int range = _actionTroop.Range;
			if (i > 0)
			{
				foreach (var pair in _actionTroop.Skills)
				{
					if (--i == 0)
					{
						target = Skill.Table[pair.Key].Target;
						range = Skill.Table[pair.Key].Range;
						_actionSkill = pair.Key;
					}
				}
			}

			if (radioButton10 == sender)
			{
				_actionSkill = ACTION_CALL;
				labelSkillDescription.Text = "呼叫後備部隊上場";
				ShowEmptySlots();
			}
			else
			{
				if (_actionSkill == -1)
					labelSkillDescription.Text = "普通攻擊";
				else
					labelSkillDescription.Text = Skill.Table[_actionSkill].Description;

				ShowSelectButtons(target, range);
			}
		}

		private void button_MouseHover(object sender, EventArgs e)
		{
			Button[] targetA = { buttonA1, buttonA2, buttonA3, buttonA4, buttonA5, buttonA6, buttonA7, buttonA8, buttonA9 };
			Button[] targetB = { buttonB1, buttonB2, buttonB3, buttonB4, buttonB5, buttonB6, buttonB7, buttonB8, buttonB9 };

			int targetId = -1;
			for (int i = 0; i < BattleController.FIELD_SIZE; i++)
			{
				if (sender == targetA[i])
					targetId = i + 1;
				else if (sender == targetB[i])
					targetId = i + BattleController.ENEMY_POSITION_BIAS + 1;
			}

			if (_actionSkill == ACTION_CALL)
				labelPredict.Text = "";
			else
			{
				int skillLv = 0;
				if (_actionSkill != -1)
					skillLv = _actionTroop.Skills[_actionSkill];
				bc.GetSkillResult(_actionTroop, targetId, _actionSkill, skillLv);
			}
		}

		private void ShowPredictDamage(object sender, string str)
		{
			if (labelPredict.InvokeRequired)
				labelPredict.Invoke(new EventHandler<string>(ShowPredictDamage), sender, str);
			else
			{
				labelPredict.Text = str;
			}
		}

		private void button_MouseLeave(object sender, EventArgs e)
		{
			labelPredict.Text = "";
		}

		private void ShowGameOver(object sender, EventArgs e)
		{
			if (groupBox1.InvokeRequired)
				groupBox1.Invoke(new EventHandler(ShowGameOver), sender, e);
			else
			{
				groupBox1.Enabled = false;
			}
		}

		private void ShowWave(object sender, int[] waves)
		{
			if (labelWave.InvokeRequired)
				labelWave.Invoke(new EventHandler<int[]>(ShowWave), sender, waves);
			else
				labelWave.Text = "Wave " + waves[0] + "/" + waves[1];
		}

		private void comboBoxCall_Click(object sender, EventArgs e)
		{
			radioButton10.Checked = true;
			OnSkillSelected(radioButton10, null);
		}
	}
}
