using System.Runtime.InteropServices;

namespace TXServer.ECSSystem.Types
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DiscreteTankControl
	{
		private const int BIT_LEFT = 0;
		private const int BIT_RIGHT = 1;
		private const int BIT_DOWN = 2;
		private const int BIT_UP = 3;
		private const int BIT_WEAPON_LEFT = 4;
		private const int BIT_WEAPON_RIGHT = 5;

		public byte Control { get; set; }

		public int MoveAxis
		{
			get => GetBit(BIT_UP) - GetBit(BIT_DOWN);
			set => SetDiscreteControl(value, BIT_DOWN, BIT_UP);
		}

		public int TurnAxis
		{
			get => GetBit(BIT_RIGHT) - GetBit(BIT_LEFT);
			set => SetDiscreteControl(value, BIT_LEFT, BIT_RIGHT);
		}

		public int WeaponControl
		{
			get => GetBit(BIT_WEAPON_RIGHT) - GetBit(BIT_WEAPON_LEFT);
			set => SetDiscreteControl(value, BIT_WEAPON_LEFT, BIT_WEAPON_RIGHT);
		}

		private int GetBit(int bitNumber)
		{
			return Control >> bitNumber & 1;
		}

		private void SetBit(int bitNumber, int value)
		{
			int num = ~(1 << bitNumber);
			Control = (byte)((Control & num) | (value & 1) << bitNumber);
		}

		private void SetDiscreteControl(int value, int negativeBit, int positiveBit)
		{
			SetBit(negativeBit, 0);
			SetBit(positiveBit, 0);
			if (value > 0)
			{
				SetBit(positiveBit, 1);
			}
			else if (value < 0)
			{
				SetBit(negativeBit, 1);
			}
		}
	}
}
