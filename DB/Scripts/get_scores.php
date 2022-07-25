<?php
include 'db_connect.php';

// Session expired
ExitIfSessionExpired();

// Credentials are provided
if (isset($_GET["game_id"]) and isset($_GET["n_scores"]))
{
	$NScores = $_GET["n_scores"];
    $result = $mysqli->query("SELECT * FROM HighScores WHERE date >= DATE_SUB(CURDATE(), INTERVAL 31 DAY) ORDER BY score DESC LIMIT ".$NScores."");
	$number_of_scores = $result->num_rows;
	$scores_sum = 0;
	
	// Convert query result to php array
	$result_array = array();
    while($row = mysqli_fetch_assoc($result))
    {
        $result_array[] = $row;
		$scores_sum += $row["score"];
    }
	
	$average_score = $scores_sum;
	
	// Don't divide by 0
	if ($number_of_scores > 0)
	{
		$average_score = round($scores_sum /= $number_of_scores);
	}
	
	echo("n_scores_last_month=".$number_of_scores.",average_score=".$average_score);
	echo json_encode($result_array);
}

else 
{
    echo "0";
}

$mysqli->close();

?>