/// <summary>
/// Global constants for RG BANG prototype
/// </summary>
public static class GameConstants
{
    // Game Modes
    public const string GAME_MODE_STAR_ARENA = "StarArena";
    public const string GAME_MODE_TEAM_ARENA = "TeamArena";
    public const string GAME_MODE_SURVIVAL = "Survival";

    // Game Settings
    public const float MATCH_DURATION = 180f; // 3 minutes in seconds
    public const int MAX_PLAYERS_PER_MATCH = 4;
    public const int INITIAL_PLAYER_LEVEL = 1;
    public const int INITIAL_PLAYER_XP = 0;

    // Character Stats
    public const float DEFAULT_MOVEMENT_SPEED = 5f;
    public const float DEFAULT_SPRINT_SPEED = 8f;
    public const float DEFAULT_DASH_SPEED = 12f;
    public const float DEFAULT_MAX_HEALTH = 100f;
    public const float DEFAULT_MAX_ENERGY = 100f;
    public const float ENERGY_REGEN_PER_SECOND = 15f;

    // Combat
    public const float BASIC_ATTACK_COOLDOWN = 0.5f;
    public const float ABILITY_COOLDOWN_BASE = 5f;
    public const float ULTIMATE_COOLDOWN_BASE = 12f;
    public const float BASIC_ATTACK_ENERGY_COST = 10f;
    public const float ABILITY_ENERGY_COST = 30f;
    public const float ULTIMATE_ENERGY_COST = 50f;

    // Stars
    public const int STAR_PICKUP_POINTS = 1;
    public const int STAR_RESPAWN_TIME = 3;
    public const int ELIMINATION_ASSIST_POINTS = 0;

    // Audio
    public const float MASTER_VOLUME = 0.8f;
    public const float MUSIC_VOLUME = 0.6f;
    public const float SFX_VOLUME = 0.7f;

    // UI
    public const string COLOR_PRIMARY_YELLOW = "#FFD700";
    public const string COLOR_SECONDARY_PURPLE = "#6A0572";
    public const string COLOR_ACCENT_CYAN = "#00D9FF";

    // Scene Names
    public const string SCENE_MAIN_MENU = "MainMenu";
    public const string SCENE_CHARACTER_SELECT = "CharacterSelect";
    public const string SCENE_LOBBY = "Lobby";
    public const string SCENE_ARENA_01 = "Arena_01";
    public const string SCENE_RESULTS = "Results";

    // Layer Names
    public const string LAYER_PLAYER = "Player";
    public const string LAYER_ENEMY = "Enemy";
    public const string LAYER_BOT = "Bot";
    public const string LAYER_OBSTACLE = "Obstacle";
    public const string LAYER_STAR = "Star";
    public const string LAYER_PROJECTILE = "Projectile";
}

public static class CharacterIds
{
    public const string KAIRO = "kairo";
    public const string REX = "rex";
    public const string FROST = "frost";
    public const string TITAN = "titan";
}

public enum GameState
{
    MainMenu,
    CharacterSelect,
    Lobby,
    Loading,
    Playing,
    Paused,
    ResultsScreen,
    Disconnected
}

public enum CharacterClass
{
    Speedster,
    Attacker,
    Controller,
    Tank
}

public enum RankTier
{
    Unranked,
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond,
    Master,
    Champion
}
