<?php
include 'db_connect.php';

$Username = $_GET['username'];
$Email = $_GET['email'];
$Password = $_GET['password'];

if (isset($Username) and isset($Password)) {
    $result = $mysqli->query("SELECT id FROM RegistredUsers WHERE username = '".$Username."'");
    if($result->num_rows == 0) {
        $mysqli->query("INSERT INTO RegistredUsers (id, username, email, password, reg_date) VALUES (NULL, '" .$Username. "', '" .$Email. "', '" .$Password. "', CURRENT_TIMESTAMP)");
        echo("SUCCES");
    } else {
        echo("ERROR_USERNAME_ALREADY_EXISTS");
    }
}
else {
    echo("ERROR_DEFAULT");
}

$mysqli->close();
?>