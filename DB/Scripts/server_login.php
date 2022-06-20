<?php
include 'db_connect.php';

session_start();

// overwrite the vars if they are provided and right
if (isset($_GET["server_id"]) && isset($_GET["server_password"]))
{
    $ServerID = $_GET["server_id"];
    $ServerPassword = $_GET["server_password"];

    $result = $mysqli->query("SELECT * FROM Servers WHERE id = '".$ServerID."' AND password = '" .$ServerPassword. "' LIMIT 1");
    if($result->num_rows == 0)
    {
      echo("ERROR_INVALID_CREDENTIALS");
    }
    
    else
    {
        $row = $result->fetch_assoc();
        $_SESSION["server_id"] = $row["id"];
        $_SESSION["server_password"] = $row["password"];
        echo session_id();
    }
}

// Use session variables if they are not provided
else
{
    if (isset($_SESSION["server_id"]) && isset($_SESSION["server_password"])) 
    {
        echo session_id();
       // echo("SESSION_STILL_VALID");
    }
    else
    {
        echo("ERROR_SESSION_EXPIRED");
    }
}



$mysqli->close();

?>