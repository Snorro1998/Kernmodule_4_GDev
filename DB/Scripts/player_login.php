<?php
include 'db_connect.php';

$Username = $_GET['username'];
$Password = $_GET['password'];

if (isset($Username) and isset($Password)) {
    $result = $mysqli->query("SELECT id FROM RegistredUsers WHERE username = '".$Username."' LIMIT 1");
    if($result->num_rows == 0) {
        echo("ERROR_USERNAME_UNKNOWN");
    } else if ($mysqli->query("SELECT id FROM RegistredUsers WHERE username = '".$Username."' AND password = '" .$Password. "' LIMIT 1")->num_rows > 0) {
        echo("SUCCES");
    }
    else {
        echo("ERROR_USERNAME_WRONG_PASSWORD");
    }
}
else {
    echo("ERROR_DEFAULT");
}

$mysqli->close();

?>