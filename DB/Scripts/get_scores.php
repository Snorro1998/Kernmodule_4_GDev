<?php
include 'db_connect.php';

if (isset($_GET["game_id"]) and isset($_GET["n_scores"]))
{
    $NScores = $_GET["n_scores"];
    $result = $mysqli->query("SELECT * FROM HighScores ORDER BY score DESC LIMIT ".$NScores."");
    //$emparray = array();
    while($row = mysqli_fetch_assoc($result))
    {
        $user_name_query = $mysqli->query("SELECT * FROM RegistredUsers WHERE id = '".$row["user_id"]."' LIMIT 1");
        $user_name_row = $user_name_query->fetch_assoc();
        echo($user_name_row["username"].",".$row["score"].".");
        //echo("".$user_name_row["username"].",".$row["score"]."|");
    }
}

else 
{
    echo("ERROR_MISSING_CREDENTIALS");
}

$mysqli->close();

?>