using UnityEngine;

namespace Dogger.Control
{
	/// <summary>
	/// Accepts four types of input from the player.
	/// </summary>
	public class PlayerInput : IPlayerInput
	{
		public float Horizontal { get => Input.GetAxis("Horizontal"); }
		public bool MouseLeftClicked => Input.GetMouseButton(0);
		public Vector3 MousePosition => Input.mousePosition;
		public bool PauseButtonPushed
		{
			get
			{
				bool buttonPressed = Input.GetKeyDown(KeyCode.Escape);
				if (buttonPressed)
				{
					pauseGame?.Invoke();
				}
				return buttonPressed;
			}
		}

		public delegate void PauseGame();
		public static PauseGame pauseGame;
	}
}