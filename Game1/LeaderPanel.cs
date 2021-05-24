using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game1
{
	public partial class LeaderPanel : UserControl
	{
		public EventHandler<int> OnPointUpdated = null;
		public EventHandler<Troop> OnInfo = null;
		public EventHandler<Troop> OnHire = null;
		public EventHandler<Troop> OnDisband = null;

		private const int SP_HEAL = 10;

		private int _id = -1;
		private bool _isHire = false;
		private Troop _troop = null;
		private int _point = 0;
		private int _baseHirePoint = 100;

		private int _leaderUpPoint = 0;
		private int _unitUpPoint = 0;
		private int _healPoint = 0;
		private int _hirePoint = 0;
		private int _battleCount = 0;

		public LeaderPanel()
		{
			InitializeComponent();
			IsHire = true;
		}

		internal void Setup (int id, int battleCount, int points, bool isHire, Troop troop)
		{
			_id = id;
			_point = points;
			_battleCount = battleCount;
			IsHire = isHire;
			Troop = troop;
			buttonDisband.Visible = id >= BattleController.RANDOM_LEADER_ID_BASE;
		}

		public bool IsHire
		{
			get	{ return _isHire; }

			set
			{
				_isHire = value;

				buttonLeader.Visible = _isHire;
				buttonUnit.Visible = _isHire;
				buttonSp.Visible = _isHire;
				buttonDisband.Visible = _isHire && _id >= BattleController.RANDOM_LEADER_ID_BASE;

				labelLeaderUp.Visible = _isHire;
				labelUnitUp.Visible = _isHire;
				labelHeal.Visible = _isHire;
				labelHire.Visible = !_isHire;
			}
		}

		internal Troop Troop
		{
			get { return _troop; }
			set
			{
				_troop = value;

				CheckPoints();
				if (_troop != null)
				{
					if (_troop.Leader != null)
					{
						labelLeader.Text = _troop.Leader.Name;
						labelLeaderLv.Text = _troop.Leader.Job + " Lv: " + _troop.Leader.Level;
						if (_troop.Leader.Level < _battleCount)
						{
							labelLeaderUp.Text = "升級需 " + _leaderUpPoint;
							buttonLeader.Enabled = _point >= _leaderUpPoint;
						}
						else
						{
							labelLeaderUp.Text = "目前無法升級";
							buttonLeader.Enabled = false;
						}
					}
					else
					{
						labelLeader.Text = "無將領";
						labelLeaderLv.Text = "";
						labelLeaderUp.Text = "";
						buttonLeader.Enabled = false;
					}

					if (_troop.Unit != null)
					{
						labelUnit.Text = _troop.Unit.Name;
						labelUnitLv.Text = "Lv: " + _troop.Unit.Level;
						if (_troop.Unit.Level <= _battleCount / 10)
						{
							labelUnitUp.Text = "升級需 " + _unitUpPoint;
							buttonUnit.Enabled = _point >= _unitUpPoint;
						}
						else
						{
							labelUnitUp.Text = "目前無法升級";
							buttonUnit.Enabled = false;
						}

						labelNumber.Text = _troop.CurrentNumber + "/" + _troop.TroopNumber + "/" + _troop.MaxNumber;
						labelHeal.Text = "回復需 " + _healPoint;
						buttonHeal.Enabled = (_point >= _healPoint && _healPoint != 0);
					}
					else
					{
						labelUnit.Text = "無部隊";
						labelUnitLv.Text = "";
						labelUnitUp.Text = "";
						buttonUnit.Enabled = false;

						labelNumber.Text = "";
						labelHeal.Text = "";
						buttonHeal.Enabled = false;
					}

					labelSp.Text = "SP:" + _troop.CurrentSp + "/" + _troop.MaxSp;
					if (_troop.CurrentSp < _troop.MaxSp && _point >= SP_HEAL)
						buttonSp.Enabled = true;
					else
						buttonSp.Enabled = false;

					if (!_isHire)
					{
						labelHire.Text = "雇用需 " + _hirePoint;
						buttonHeal.Enabled = buttonHeal.Enabled && (_point >= _hirePoint);
					}
				}
			}
		}

		public int Point
		{
			get { return _point; }
			set { _point = value; }
		}

		public int BaseHirePoint { get { return _baseHirePoint; } set { _baseHirePoint = value; } }

		private void CheckPoints()
		{
			if (_troop != null)
			{
				_hirePoint = _baseHirePoint;

				if (_troop.Leader != null)
				{
					_leaderUpPoint = 10 * (_troop.Leader.Level + 1);
					_hirePoint += 5 * _troop.Leader.Level * (_troop.Leader.Level + 1) / 2;
				}

				if (_troop.Unit != null)
				{
					_unitUpPoint = 10 * (_troop.Unit.Level + 1) * (_troop.Unit.Level + 1);
					if (_troop.Unit.Rank <= 0)
						_unitUpPoint /= 2;
					else
						_unitUpPoint *= _troop.Unit.Rank;

					_healPoint = (_troop.MaxNumber - _troop.CurrentNumber) + (_troop.MaxNumber - _troop.TroopNumber) * 2;
					if (_troop.Unit.Rank <= 0)
						_healPoint /= 2;
					else
						_healPoint *= _troop.Unit.Rank;

					// Sigma (n^2) = n(n+1)(2n+1)/6
					int troopLvCost = 5 * _troop.Unit.Level * (_troop.Unit.Level + 1) * (2 * _troop.Unit.Level + 1) / 6;
					if (_troop.Unit.Rank <= 0)
						troopLvCost /= 2;
					else
						troopLvCost *= _troop.Unit.Rank;

					int troopNumberCost = _troop.MaxNumber;
					if (_troop.Unit.Rank <= 0)
						troopNumberCost /= 2;
					else
						troopNumberCost *= _troop.Unit.Rank;

					_hirePoint += (troopLvCost + troopNumberCost);
				}
			}
		}

		private void buttonInfo_Click(object sender, EventArgs e)
		{
			OnInfo(this, _troop);
		}

		private void buttonLeader_Click(object sender, EventArgs e)
		{
			if (_troop != null && _troop.Leader != null)
			{
				if (_point >= _leaderUpPoint)
				{
					_point -= _leaderUpPoint;
					_troop.Leader.LevelUp(1);
					_troop.LeaderHp = _troop.Leader.HitPoint;
					_troop.MaxNumber += _troop.Leader.UnitGrow;
					//_troop.TroopNumber += _troop.Leader.UnitGrow;
					//_troop.CurrentNumber += _troop.Leader.UnitGrow;
					_troop.LeaderSp = _troop.Leader.SkillPoint;

					Troop = _troop; // Refresh.
					OnPointUpdated(this, _point);
				}
			}
		}

		private void buttonUnit_Click(object sender, EventArgs e)
		{
			if (_troop != null && _troop.Unit != null)
			{
				if (_point >= _unitUpPoint)
				{
					_point -= _unitUpPoint;
					_troop.Unit.LevelUp(1);
					_troop.UnitHp = _troop.Unit.HitPoint;
					_troop.UnitSp = _troop.Unit.SkillPoint;
					Troop = _troop; // Refresh.
					OnPointUpdated(this, _point);
				}
			}
		}

		private void buttonHeal_Click(object sender, EventArgs e)
		{
			if (_troop != null && _troop.Unit != null)
			{
				if (_isHire)
				{
					if (_point >= _healPoint)
					{
						_point -= _healPoint;
						_troop.CurrentNumber = _troop.TroopNumber = _troop.MaxNumber;
						Troop = _troop; // Refresh.
						OnPointUpdated(this, _point);
					}
				}
				else
				{
					_point -= _hirePoint;
					IsHire = true;
					OnPointUpdated(this, _point);
					OnHire(this, _troop);
				}
			}
		}

		private void buttonSp_Click(object sender, EventArgs e)
		{
			if (_troop.Leader != null)
				_troop.LeaderSp = _troop.Leader.SkillPoint;
			if (_troop.Unit != null)
				_troop.UnitSp = _troop.Unit.SkillPoint;
			_point -= SP_HEAL;
			Troop = _troop; // Refresh.
			OnPointUpdated(this, _point);
		}

		private void buttonDisband_Click(object sender, EventArgs e)
		{
			OnDisband(this, _troop);
		}

		public void SetHire(bool canHire)
		{
			if (canHire)
				Troop = _troop;
			else if (!_isHire)
				buttonHeal.Enabled = canHire;
		}
	}
}
