

using CSharpWars.Common.Extensions;
using CSharpWars.WebApi.Contracts;
using System.Net.Http.Json;


#region SCRIPT

const string WalkAround =
        "var step = LoadFromMemory<int>(\"STEP\");\r\n" +
        "if( step % 3 == 0 )\r\n" +
        "{\r\n" +
        "    TurnLeft();\r\n" +
        "}\r\n" +
        "else\r\n" +
        "{\r\n" +
        "    WalkForward();\r\n" +
        "}\r\n" +
        "step++;\r\n" +
        "StoreInMemory<int>(\"STEP\", step);\r\n";

const string WalkBackAndForth =
    "var step = LoadFromMemory<int>(\"STEP\");\r\n" +
    "if( step % 5 == 0 )\r\n" +
    "{\r\n" +
    "    TurnAround();\r\n" +
    "}\r\n" +
    "else\r\n" +
    "{\r\n" +
    "    WalkForward();\r\n" +
    "}\r\n" +
    "step++;\r\n" +
    "StoreInMemory<int>(\"STEP\", step);\r\n";

const string LookAroundAndSelfDestruct =
    "var selfDestructed = false;\r\n" +
    "foreach( var enemy in Vision.EnemyBots )\r\n" +
    "{\r\n" +
    "    var distanceX = Math.Abs(enemy.X - X);\r\n" +
    "    var distanceY = Math.Abs(enemy.Y - Y);\r\n" +
    "    if( distanceX < 4 && distanceY < 4 )\r\n" +
    "    {\r\n" +
    "        SelfDestruct();\r\n" +
    "        selfDestructed = true;\r\n" +
    "        break;\r\n" +
    "    }\r\n" +
    "}\r\n" +
    "\r\n" +
    "if( !selfDestructed )\r\n" +
    "{\r\n" +
    "    TurnLeft();\r\n" +
    "}\r\n";

const string LookAroundAndRangeAttack =
    "var attacked = false;\r\n" +
    "foreach( var enemy in Vision.EnemyBots )\r\n" +
    "{\r\n" +
    "    RangedAttack( enemy.X , enemy.Y );\r\n" +
    "    attacked = true;\r\n" +
    "    break;\r\n" +
    "}\r\n" +
    "\r\n" +
    "if( !attacked )\r\n" +
    "{\r\n" +
    "    TurnLeft();\r\n" +
    "}\r\n";

const string TeleportAround =
    "var r = new Random();\r\n" +
    "var destinationX = r.Next(0, Width);\r\n" +
    "var destinationY = r.Next(0, Height);\r\n" +
    "Teleport( destinationX , destinationY );\r\n";

const string HuntDown =
    "bool inMyFace = false;\r\n" +
    "foreach (var enemyBot in Vision.EnemyBots)\r\n" +
    "{\r\n" +
    "    if (IsInMyFace(X, Y, enemyBot))\r\n" +
    "    {\r\n" +
    "        MeleeAttack();\r\n" +
    "        inMyFace = true;\r\n" +
    "        break;\r\n" +
    "    }\r\n" +
    "}\r\n" +
    "\r\n" +
    "if (!inMyFace)\r\n" +
    "{\r\n" +
    "    Bot targetBot = null;\r\n" +
    "\r\n" +
    "    foreach (var enemyBot in Vision.EnemyBots)\r\n" +
    "    {\r\n" +
    "        if (targetBot == null || CalculateDistance(X, Y, enemyBot) < CalculateDistance(X, Y, targetBot))\r\n" +
    "        {\r\n" +
    "            targetBot = enemyBot;\r\n" +
    "        }\r\n" +
    "    }\r\n" +
    "\r\n" +
    "    if (targetBot == null)\r\n" +
    "    {\r\n" +
    "        TurnLeft();\r\n" +
    "        StoreInMemory(\"DISTANCE\", 0);\r\n" +
    "    }\r\n" +
    "    else\r\n" +
    "    {\r\n" +
    "        double distance = LoadFromMemory<double>(\"DISTANCE\");\r\n" +
    "        if (distance == 0)\r\n" +
    "        {\r\n" +
    "            WalkForward();\r\n" +
    "        }\r\n" +
    "        else if (CalculateDistance(X, Y, targetBot) < distance)\r\n" +
    "        {\r\n" +
    "            WalkForward();\r\n" +
    "            StoreInMemory(\"DISTANCE\", CalculateDistance(X, Y, targetBot));\r\n" +
    "        }\r\n" +
    "        else\r\n" +
    "        {\r\n" +
    "            TurnRight();\r\n" +
    "        }\r\n" +
    "    }\r\n" +
    "}\r\n" +
    "\r\n" +
    "public bool IsInMyFace(int myX, int myY, Bot target)\r\n" +
    "{\r\n" +
    "    switch (Orientation)\r\n" +
    "    {\r\n" +
    "        case NORTH:\r\n" +
    "            return myX == target.X && myY == target.Y + 1;\r\n" +
    "        case EAST:\r\n" +
    "            return myY == target.Y && myX == target.X - 1;\r\n" +
    "        case SOUTH:\r\n" +
    "            return myX == target.X && myY == target.Y - 1;\r\n" +
    "        case WEST:\r\n" +
    "            return myY == target.Y && myX == target.X + 1;\r\n" +
    "    }\r\n" +
    "\r\n" +
    "    return false;\r\n" +
    "}\r\n" +
    "\r\n" +
    "public double CalculateDistance(int myX, int myY, Bot target)\r\n" +
    "{\r\n" +
    "    return Math.Sqrt(Math.Pow(myX - target.X, 2) + Math.Pow(myY - target.Y, 2));\r\n" +
    "}\r\n";

string[] Scripts = { WalkAround, WalkBackAndForth, LookAroundAndSelfDestruct, LookAroundAndRangeAttack, TeleportAround, HuntDown };

#endregion


WriteLine("CSharpWars Bot Generator");
WriteLine("------------------------");
WriteLine();



for (int count = 1; count <= 25; count++)
{
    var scriptIndex = count % Scripts.Length;

    using var client = new HttpClient();

    var loginRequest = new LoginRequest { Username = $"Player {count:D2}", Password = $"Player {count:D2}" };
    using var response1 = await client.PostAsJsonAsync("https://api.csharpwars.com/players", loginRequest);
    var loginResponse = await response1.Content.ReadFromJsonAsync<LoginResponse>();

    client.DefaultRequestHeaders.Add("Authorization", loginResponse.Token);
    var createBotRequest = new CreateBotRequest
    {
        ArenaName = "default",
        BotName = $"Bot {count:D2}",
        MaximumHealth = 100,
        MaximumStamina = 100,
        PlayerName = $"Player {count:D2}",
        Script = Scripts[scriptIndex].Base64Encode()
    };
    using var response2 = await client.PostAsJsonAsync("https://api.csharpwars.com/arena/default/bots", createBotRequest);
    var debug = await response2.Content.ReadAsStringAsync();
    var createBotResponse = await response2.Content.ReadFromJsonAsync<CreateBotResponse>();

    WriteLine($"Player {count:D2} created!");
}