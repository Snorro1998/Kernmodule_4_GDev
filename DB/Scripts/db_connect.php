<?php
   $db_user = 'tjaardvanverseveld';
   $db_pass = 'waiQuoh0Th';
   $db_host = 'localhost';
   $db_name = 'tjaardvanverseveld';

session_start();

// Open a connection
$mysqli = new mysqli("$db_host","$db_user","$db_pass","$db_name");

// Check connection
if ($mysqli->connect_errno) {
   echo "Failed to connect to MySQL: (" . $mysqli->connect_errno() . ") " . $mysqli->connect_error();
   exit();
}

function ServerSessionValid()
{
    return (isset($_SESSION["server_id"]) && isset($_SESSION["server_password"]));
}

function ExitIfSessionExpired()
{
	if (!ServerSessionValid()) 
	{
		echo("0");
		$mysqli->close();
		exit();
	}
}

?>