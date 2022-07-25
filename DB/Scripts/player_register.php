<?php
include 'db_connect.php';

// Session expired
ExitIfSessionExpired();

// Credentials provided
if (isset($_GET['username']) and isset($_GET['password']) and isset($_GET['email']))
{
	
	$Username = $_GET['username'];
	$Email = $_GET['email'];
	$Password = $_GET['password'];
	
	// Email isn't as it should be
	if (!filter_var($Email, FILTER_VALIDATE_EMAIL))
	{
		echo "0";
	}
	
	// Email is correct
	else
	{
		$result = $mysqli->query("SELECT id FROM RegistredUsers WHERE username = '".$Username."' OR email = '".$Email."'");
		if($result->num_rows == 0)
		{
			$mysqli->query("INSERT INTO RegistredUsers (id, username, email, password, reg_date) VALUES (NULL, '" .$Username. "', '" .$Email. "', '" .$Password. "', CURRENT_TIMESTAMP)");
			echo("1");
		}
		// Username or email already used
		else
		{
			echo "0";
		}
	}   
}

// Everything failed
else
{
    echo "0";
}

$mysqli->close();
?>