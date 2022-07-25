-- phpMyAdmin SQL Dump
-- version 4.9.4
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jul 25, 2022 at 06:15 PM
-- Server version: 10.5.16-MariaDB
-- PHP Version: 7.4.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `tjaardvanverseveld`
--

-- --------------------------------------------------------

--
-- Table structure for table `Games`
--

CREATE TABLE `Games` (
  `id` tinyint(6) UNSIGNED NOT NULL,
  `name` varchar(60) NOT NULL,
  `description` varchar(128) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `Games`
--

INSERT INTO `Games` (`id`, `name`, `description`) VALUES
(1, 'Dangerous Dungeon', 'Silly multiplayer game'),
(2, 'Dancing with Dave', 'Amazing rhythm game'),
(3, 'Topple Towers', 'Balancing game'),
(4, 'Super Sloth', 'Swinging game'),
(5, 'Cat Cuddler', 'Cute tamagochi game');

-- --------------------------------------------------------

--
-- Table structure for table `HighScores`
--

CREATE TABLE `HighScores` (
  `id` int(6) UNSIGNED NOT NULL,
  `game_id` int(6) DEFAULT NULL,
  `user_id` int(6) DEFAULT NULL,
  `score` int(6) DEFAULT NULL,
  `date` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `HighScores`
--

INSERT INTO `HighScores` (`id`, `game_id`, `user_id`, `score`, `date`) VALUES
(1, 1, 1, 9, '2021-08-31 14:45:39'),
(2, 2, 7, 8, '2021-08-31 14:46:16'),
(3, 2, 3, 10, '2021-08-31 14:54:02'),
(4, 1, 5, 15, '2021-08-31 15:16:35'),
(5, 2, 4, 11, '2021-08-31 15:16:35'),
(6, 1, 1, 5, '2022-05-04 20:26:14'),
(7, 1, 2, 7, '2022-06-20 08:26:12'),
(8, 1, 3, 4, '2022-06-20 08:40:56'),
(9, 1, 1, 1, '2022-06-20 08:52:56'),
(10, 1, 3, 7, '2022-06-20 10:07:27'),
(11, 1, 1, 1, '2022-06-20 10:15:26'),
(12, 1, 3, 11, '2022-06-20 10:23:56'),
(13, 1, 1, 1, '2022-06-20 10:28:49'),
(14, 1, 12, 1, '2022-06-20 10:31:03'),
(15, 1, 7, 5, '2022-07-09 11:30:18'),
(16, 1, 2, 9, '2022-07-25 11:35:01'),
(17, 1, 17, 3, '2022-07-25 14:53:12');

-- --------------------------------------------------------

--
-- Table structure for table `RegistredUsers`
--

CREATE TABLE `RegistredUsers` (
  `id` int(6) UNSIGNED NOT NULL,
  `username` varchar(30) NOT NULL,
  `email` varchar(60) NOT NULL,
  `password` varchar(30) NOT NULL,
  `reg_date` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `RegistredUsers`
--

INSERT INTO `RegistredUsers` (`id`, `username`, `email`, `password`, `reg_date`) VALUES
(1, 'Stals', 'bstals@scheikunde.nl', 'AHUM', '2021-05-27 09:29:57'),
(2, 'Harry', 'harry@potter.uk', 'Potter', '2020-09-06 12:29:55'),
(3, 'Paul', 'paul@pvloon.tk', 'Griezelbus', '2020-09-06 12:29:55'),
(4, 'Heidi', 'heidi@windmayer.de', 'Windmayer', '2020-09-06 12:29:55'),
(5, 'Ton', 'tmarkus@hyves.nl', 'Markus', '2021-06-20 19:42:46'),
(6, 'Sietse', 'speedboot@kameleon.nl', 'Klinkhamer', '2021-06-20 19:43:20'),
(7, 'Tjaard', 'tjaaaaaard@vv.nl', 'VV', '2021-06-21 14:26:42'),
(8, 'Meessie', 'poezebeest@kat.tw', 'Miauw', '2021-06-21 14:38:47'),
(9, 'ShinChan', 'koeliejoelie@foxkids.jp', 'Nohara', '2021-08-31 17:50:11'),
(10, 'Ardynell', 'ardy@snor.nl', 'Wauw', '2022-04-17 12:02:43'),
(11, 'Samson', 'mwoahgertjuh@studio100.be', 'Hond', '2022-04-17 14:50:54'),
(12, 'Alberto', 'hetisalbertoooo@studio100.be', 'Dagiedereeeeeeeen', '2022-04-17 15:02:46'),
(13, 'Mark', 'minsterpresident@overheid.nl', 'Rutte', '2022-04-17 15:11:02'),
(14, 'Philip', 'golflengte@natuurkunde.nl', 'Habing', '2022-04-17 15:13:21'),
(15, 'Gert', 'neesamson@studio100.be', 'XoxoMarleneke', '2022-04-17 15:14:59'),
(16, 'Aang', 'appa@yipyip.fr', 'Avatar', '2022-04-17 15:20:44'),
(17, 'Folkert', 'medicijnman@woah.nl', 'Burp', '2022-07-09 11:35:56'),
(18, 'Sinterklaas', 'stoomboot@kado.nl', 'Pepernoot', '2022-07-25 17:49:03'),
(19, 'Garfield', 'ihateodie@arbuckle.us', 'Lasagna', '2022-07-25 17:59:06'),
(20, 'Geert', 'geertwiltwatwilders@tweedekamer.nl', 'Wilders', '2022-07-25 18:02:34'),
(21, 'MrWolf', 'yesyesyesyes@awolf.ca', 'Awoe', '2022-07-25 18:06:09');

-- --------------------------------------------------------

--
-- Table structure for table `Servers`
--

CREATE TABLE `Servers` (
  `id` int(11) NOT NULL,
  `password` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `Servers`
--

INSERT INTO `Servers` (`id`, `password`) VALUES
(1, 'SuperGeheim'),
(2, 'WerkMeeAjb');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Games`
--
ALTER TABLE `Games`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `HighScores`
--
ALTER TABLE `HighScores`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `RegistredUsers`
--
ALTER TABLE `RegistredUsers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `Servers`
--
ALTER TABLE `Servers`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Games`
--
ALTER TABLE `Games`
  MODIFY `id` tinyint(6) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `HighScores`
--
ALTER TABLE `HighScores`
  MODIFY `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `RegistredUsers`
--
ALTER TABLE `RegistredUsers`
  MODIFY `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT for table `Servers`
--
ALTER TABLE `Servers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
