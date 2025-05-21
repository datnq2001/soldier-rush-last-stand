// GameEntity.cs - Base abstract class for all entities with position and lane info
namespace SoldierRushGame
{
    // Enum representing the vertical lanes where entities can be placed
    public enum Lane
    {
        Top,      // Upper lane
        Bottom    // Lower lane
    }

    // Abstract base class for all game entities (soldiers, zombies, power-ups, etc.)
    public abstract class GameEntity
    {
        public Lane Lane { get; set; }   // Lane that the entity occupies
        public float Y { get; set; }     // Y-coordinate based on lane
        public float X { get; set; }     // X-coordinate for horizontal movement

        // Update logic for the entity (movement, animation, etc.)
        public abstract void Update();

        // Draw the entity on screen
        public abstract void Draw();
    }
}
