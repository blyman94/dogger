using UnityEngine;

namespace Dogger.Control
{
	/// <summary>
	/// An interface for player input.
	/// Declares getter methods for all possible player input.
	/// </summary>
	public interface IPlayerInput
	{
		float Horizontal { get; }
		bool MouseLeftClicked { get; }
		Vector3 MousePosition { get; }
		bool PauseButtonPushed { get; }
	}
}