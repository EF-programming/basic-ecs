# Basic ECS

Basic ECS is an Entity-Component System (ECS) library for C#. It provides a framework for managing entities and components, allowing for the creation of highly flexible and modular systems.

## What is an Entity-Component System?
An Entity-Component System (ECS) is a design pattern commonly used in game development. It allows for the separation of data (components) and behavior (systems), enabling more modular and efficient code. For more information, see [entity-systems.wikidot.com](http://entity-systems.wikidot.com/).

## Usage
1. Define your components by implementing the `IComponent` interface.
2. Create systems that process entities with specific components by inheriting from `EntityProcessingSystem`.
3. Manage entities and components using `ECSWorld`.

Example:
```csharp
public class PositionComponent : IComponent {
    public float X { get; set; }
    public float Y { get; set; }
}

public class MovementSystem : EntityProcessingSystem {
    public override void Process(Entity entity) {
        var position = entity.GetComponent<PositionComponent>();
        // Update position logic here
    }
}

// Initialize ECS world
ECSWorld world = new ECSWorld();
MovementSystem movementSystem = new();
movementSystem.setECSWorld(world);

// Add an entity composed of one or several components
IComponent[] entity = new[] {
  new PositionComponent(),
};
world.AddEntity(entity);
movementSystem.processEntities();
```
