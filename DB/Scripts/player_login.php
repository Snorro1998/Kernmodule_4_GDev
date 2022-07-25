<?php
include 'db_connect.php';

// Session expired
ExitIfSessionExpired();

if (isset($_GET['username']) and isset($_GET['password']))
{
	$Username = $_GET['username'];
	$Password = $_GET['password'];
    $result = $mysqli->query("SELECT id FROM RegistredUsers WHERE username = '".$Username."' AND password = '" .$Password. "' LIMIT 1");
	
    if(mysqli_num_rows($result) == 0)
	{
        echo("0");
	}
	
    else
	{
		$row = $result->fetch_assoc();
        $_SESSION["server_id"] = $row["id"];
        echo $row["id"];
    }	
}

else
{
    echo "0";
}


$mysqli->close();

?>