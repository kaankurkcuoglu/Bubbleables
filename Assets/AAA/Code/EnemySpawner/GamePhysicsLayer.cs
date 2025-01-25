namespace Game
{
	public enum GamePhysicsLayer
	{
		Default = 0,
		Player = 1,
		Enemy = 2,
		Ground = 3,
		Projectile = 4,
	}

	public enum GamePhysicsMask
	{
		Default = 1 << GamePhysicsLayer.Default,
		Player = 1 << GamePhysicsLayer.Player,
		Enemy = 1 << GamePhysicsLayer.Enemy,
		Ground = 1 << GamePhysicsLayer.Ground,
		Projectile = 1 << GamePhysicsLayer.Projectile,
	}
}