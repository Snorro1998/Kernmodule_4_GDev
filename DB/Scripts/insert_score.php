<?php
include 'db_connect.php';

// Session expired
ExitIfSessionExpired();

if (isset($_GET["username"]) and isset($_GET["game_id"]) and isset($_GET["score"]))
{
    $Username = $_GET['username'];
    $GameID = $_GET["game_id"];
    $UserID = -1;
    $Score = $_GET["score"];
	
	$result = $mysqli->query("SELECT * FROM Servers WHERE id = '".$GameID."' LIMIT 1");
	
	// Invalid game_id
	if ($result->num_rows == 0)
	{
		echo "0";
	}
	
	else
	{
		$result = $mysqli->query("SELECT * FROM RegistredUsers WHERE username = '".$Username."' LIMIT 1");	

		// Invalid username
		if($result->num_rows == 0)
		{
			echo "0";
		}
		else
		{
			$row = $result->fetch_assoc();
			$UserID = $row["id"];
			$result = $mysqli->query("INSERT INTO HighScores (id, game_id, user_id, score, date) VALUES (NULL, '".$GameID."', '".$UserID."', '".$Score."', current_timestamp())");
			echo "1";
		}
	} 
}

// Missing credentials
else 
{
    echo "0";
}

$mysqli->close();

?>