<?php
include 'db_connect.php';

if (isset($_GET["username"]) and isset($_GET["game_id"]) and isset($_GET["score"]))
{
    $Username = $_GET['username'];
    $GameID = $_GET["game_id"];
    $UserID = -1;
    $Score = $_GET["score"];
    $result = $mysqli->query("SELECT * FROM RegistredUsers WHERE username = '".$Username."' LIMIT 1");
    if($result->num_rows == 0)
    {
        echo("ERROR_UNKNOWN_USERNAME");
    }
    else
    {
        $row = $result->fetch_assoc();
        $UserID = $row["id"];
        $result = $mysqli->query("INSERT INTO HighScores (id, game_id, user_id, score, date) VALUES (NULL, '".$GameID."', '".$UserID."', '".$Score."', current_timestamp())");
         
    }
}

else 
{
    echo("ERROR_MISSING_CREDENTIALS");
}

$mysqli->close();

?>