
/// <summary>
/// DISCLAIMER: I may go through the same class twice or triple times, a context-based documentation :)
/// 
/// Okkay, here we go, 
/// 
/// Let's try to knock off some easy things out of our way .. To be specific I will start with <see cref="GameProperties"/> & the Start scene
/// 
/// <see cref="GameProperties"/> Acts like a cloud info storage that runs across the game, it contains info like isGamePaused, Score, player Level, and initial snake length.
/// This plays a great role while prototyping, it enables you to set default variables as well as very quick access to them through (Ctrl+, in both vs and mono)
/// 
/// 
/// 
/// Start scene has been handled almost completely from the inspector
/// Only one script is running in the scene, <see cref="StartManager"/>.
/// Simply it's called by clicking buttons of the level to point to different levels by setting <see cref="GameProperties.levelFilePath"/>  through <see cref="StartManager.SetLevelTo(int)"/>
/// levelFilePath later is read by in the Main scene
/// 
/// The animations (show/hide levels) as I have said above has been handled completely from the inspector, you can see this by going through Canvas > StartMenuItems > Start/Quit/Levels(1,2,3)
/// They are using the animator component assigned to Canvas > StartMenuItems
/// 
/// Now let's head to the real thing, the Main scene
/// 
/// 
/// 
/// There are 2 key classes here we need to get a glimpse of, at the beginning, in order to understand the whole code base. <see cref="GameplayManager"/>  & <see cref="PlaygroundController"/>
/// 
/// The final game scene consists of:
///     1- A very simple 2D-array based game that runs all the logic
///     2- A 3D representation of whats in that 2D Array
/// 
/// 
/// <see cref="PlaygroundController"/> is the class that have that 2D-array (List of List) <see cref=PlaygroundController.gameGrid"/> and runs the game logic
///     * <see cref="PlaygroundController.gameGrid"/> is a 2D-array of <see cref="TileType"/> that represents the current state of the game, each tile can be a Snake, Empty, Obstacle or a Fruit <see cref="Pickup"/>
///     
///     * For each <see cref="PlaygroundController.UpdateBoard(Direction)"/> call : It updates the snake on the grid using the Direction parameter sent to it
///         That is: you call UpdateBoard 3 times, the snake moves 3 steps (hence the game's speed is not controlled by the PlaygroundController at all)
///     
///     * The class also provide some important information like:
///         - <see cref="PlaygroundController.snakeGridTiles"/> Which is a list of the indices that represents a snake, this is important in order to identify which snakeTile follows which tile 
///             in case they all got together, this is used by <see cref="SnakeController"/> to Draw the snake 3D object in the world
///         - <see cref="PlaygroundController.obstaclesGridTiles"/> Which is a list of the indices that represents the obstacles in the levels (used by <see cref="EnvironmentManager"/> to instantiate the obstacles
///         - <see cref="PlaygroundController.pickupPosition"/> Which follows the same pattern, used to place the actual 3D object of the Fruit (Pickup)
///     
///     * The class as well provide 2 Actions outer classes can subscribe to, to know what's happening on the board like <see cref="PlaygroundController.OnSnakeCrashed"/> and <see cref="PlaygroundController.OnItemPickedup"/>
/// 
/// 
/// 
/// The Second key class here is <see cref="GameplayManager"/> : The class that handles most of the gameplay scene
///     * It works as follow:
///         * START():
///         - Ask the <see cref="LevelLoader"/> class to load the level selected from the Start Scene
///         - Command the playgroundController to construct the gameGrid from the data fetched from levelLoader and allows it to add a snake to the grid
///         - Command the EnvironmentManager to construct the 3D World using info fetched from playgroundController
///         - Initializes the snake object in both playgroundController and the representer class <see cref="SnakeController"/> (it gives both the random direction it should be starting with)
///         - Resets the player score
///         - Setup the listeners where it needs to hear from components classes about what's going on in the world
///         
///         *UPDATE():
///         - Then we have two states.
///            Either the game is paused => Nothing happens
///         -  Or the game is running, now depending on <see cref="SnakeController.SnakeSpeed"/> the class determines how often it should call <see cref="PlaygroundController.UpdateBoard(Direction)"/>
///         
///         *A number of functions follows which are invokes as soon as the corresponding events happens
///         - <see cref="GameplayManager.OnItemPickedup"/>: Activates as <see cref="PlaygroundController.OnItemPickedup"/> actions fires, of course when the snake head tile overlaps a Fruit tile 
///             Since playgroundController by then would have regenerated a new position for the fruit (pickup), gameplayManager will just use this info from <see cref="PlaygroundController.pickupPosition"/> to replace the fruit
///             And based on the level : it awards the player and increases it's speed
///         - <see cref="GameplayManager.OnGoHome"/>: Activates when the player press go home button from the Pause popup (Canvas > PausePopup) or from Gameover popup (Canvas > GameoverPopup)
///         - <see cref="GameplayManager.OnGamePaused"/>: Activates when the player press Pause button, it lowers the main music level and turns off <see cref="GameProperties.isGamePaused"/> which in turn blocks <see cref="GameplayInputManager"/> from accepting any gamepla input
///         - <see cref="GameplayManager.OnSnakeCrashed"/>: Activates when snake crashes with boarders, obstacles or itself, it stops the game and activates the death animation only !!
///         - <see cref="GameplayManager.OnGameOver"/>:  Activates when snake is actually finished from running the death animation, it shows then the Gameover popup
///         - <see cref="GameplayManager.OnGameRestart"/>:  Activates when player presses Restart button in the Pause popup, restarts the scene :D
///         - <see cref="GameplayManager.OnGameQuit"/>: Activates when player press X button in the pause popup
///         - <see cref="GameplayManager.OnGameResumed"/>: Activates when player press on resume button or click on an empty area in the pause menu 
///         
///You see very simple :D
///
/// Now let's return to a number of classes in detail:
/// 
/// The <see cref="GameplayInputManager"/> acts heavily on its own, it's a simple version of command pattern:
///     - It Provides 4 actions available for subscribtion and handles two methods for input, either through Keyboard (WASD and the Arrows) or throug swipe on the Mobiles
///     - The 4 actions are OnMoveRight/Down/Left/Up, the actions necessary for the game
///     - It fetches a swipe manually
///         * It records the main touch (Touch(0)) start position, and the end position (where Touch(0) left the screen) 
///           Then checks which direction has been the most effective in the touch vector (from the endpoint to the startpoint)
///     - According to the <see cref="GameplayInputManager.detectedSwipe"/> and the Keyboard input it fires the corresponding event
///       That is: if the detectedSwipe is Vector2.Right or the Right Arrow key has been pressed or the D key has been pressed, <see cref="GameplayInputManager.OnMoveRight"/> Action is fired
///       the same goes for the rest of the 3 Actions
///     - Typically a class that's intersted in these actions would listen to them like <see cref="SnakeController"/> to change the SnakeDirection
///     
/// The <see cref="SnakeController"/> 
///     - Uses list of snake tiles positions from <see cref="PlaygroundController"/> passed through <see cref="GameplayManager"/> to create 3D Objects based <see cref="SnakeController.snakeTilePrefab"/>
///     - These 3D objects are stored in <see cref="SnakeController.snakeTiles"/>
///     - It controls which animation should be played by each tile, a newly added tile should play the BirthAnimation, while a normal tile should play the Straight Animation
///     - It listens to the inputManager and registers the input commands (in case player wanted to quickly navigate Down-Right for example)
///     - Hence the direction of the snake can be aknowledged through <see cref="SnakeController.GrabNextDirection"/> which looks at the <see cref="SnakeController.registeredDirections"/> and picks the top of the queue
/// 
/// The <see cref="EnvironmentManager"/> is a very simple class as well
///     - Holds prefabs and references to the objects that constructs the 3D Scene
///     - <see cref="EnvironmentManager.playgroundBackground"/> the 3D Floor Plane, it resizes it based on the level width/height as well as changing it's main texture tiling
///     - Uses obstaclePrefab and boarderTilePrefab to instantiate copies of them and place them based on info passed from playgroundController
///     - Adjust the camera height based on the level width/height, values of <see cref="EnvironmentManager.LevelLengthToCameraHeightRatio"/> and <see cref="EnvironmentManager.LevelWidthToCameraHeightRatio"/>
///       have been obtained by calibrating the camera on a 20x20 and 20x40 level
///     - All of the above are get executed by calling <see cref="EnvironmentManager.UpdateEnvironment(int, int, List{Tile})"/> that's called by GameplayManager
///     
/// The <see cref="AudioManager"/> also acts on it's own
///     - You simply reference all the audio files in the <see cref="AudioManager.audioObjects"/> from the inspector
///         *This can be furhter improved by attaching all the audio objects as childs to it
///     - Then you give them a corresponding titles in <see cref="Track"/> enum
///     - Then these sounds are easily accessed by Play, Stop and SetVolume functions from the class
///     
/// The <see cref="UIManager"/> controls the visibility of Pause/Gameover menus, as well as providing Actions corresponding to each button being clicked, Whether it is Pause, Resume, Restart ... etc
/// </summary>
