using UnityEngine;

namespace Watermelon
{
	public interface IScrollBehavior
    {
        bool IsHorizontalScroll { get; }

        GameObject ScrollGameObject { get; }
	}
}
