using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IFlowStack
	{
		event FlowStack.ChangedDelegate Changed;

		IFlow Top { get; }

		void Push(IFlow c);

		IEnumerator PushAndWait(IFlow c);

		T Find<T>() where T : IFlow;

		IEnumerable<IFlow> TraverseStack();

		bool IsFlowInStack<T>() where T : IFlow;

		void TerminateAll();
	}
}
