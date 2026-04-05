classDiagram
    class Game {
        -bool isRunning
        +Run()
        -HandleInput()
        -Update()
        -Render()
    }

    class Map {
        +int Width
        +int Height
        +List~Entity~ Entities
        +Draw()
    }

    class Entity {
        <<abstract>>
        +string Name
        +int Health
        +int X
        +int Y
        +Move(int dx, int dy)
    }

    class Player {
        +int Experience
        +LevelUp()
    }

    class Enemy {
        +int Damage
        +Attack(Entity target)
    }

    class Item {
        +string ItemName
        +Use(Player player)
    }

    Game "1" -- "1" Map : contains
    Map "1" -- "*" Entity : manages
    Entity <|-- Player : inheritance
    Entity <|-- Enemy : inheritance
    Player "1" -- "*" Item : has
    Game ..> Player : uses