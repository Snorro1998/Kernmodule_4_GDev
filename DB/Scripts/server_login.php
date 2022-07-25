<?php
include 'db_connect.php';


// Print session id if session is still valid
if (ServerSessionValid()) 
{
	echo session_id();
}

// Session non-existant or expired
else
{
	// Credentials are provided in url
	if (isset($_GET["server_id"]) && isset($_GET["server_password"]))
	{
		$ServerID = $_GET["server_id"];
		$ServerPassword = $_GET["server_password"];

		$result = $mysqli->query("SELECT * FROM Servers WHERE id = '".$ServerID."' AND password = '" .$ServerPassword. "' LIMIT 1");
		
		// Credentials are wrong
		if($result->num_rows == 0)
		{
			echo "0";
		}
		
		// Set the variables for this session if they are provided and right and print the session id
		else
		{
			$row = $result->fetch_assoc();
			$_SESSION["server_id"] = $row["id"];
			$_SESSION["server_password"] = $row["password"];
			echo session_id();
		}
	}
	
	// Everything failed
	else
	{
		echo "0";
	}
}

$mysqli->close();

?>