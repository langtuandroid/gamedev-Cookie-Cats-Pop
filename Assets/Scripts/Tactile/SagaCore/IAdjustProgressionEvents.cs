using System;

namespace Tactile.SagaCore
{
	public interface IAdjustProgressionEvents
	{
		string ADJUST_IO_LEVEL_3_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_LEVEL_5_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_LEVEL_7_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_LEVEL_10_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_LEVEL_20_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_LEVEL_30_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_LEVEL_40_COMPLETED_EVENT_TOKEN { get; }

		string ADJUST_IO_GATE_1_UNLOCKED_EVENT_TOKEN { get; }
	}
}
