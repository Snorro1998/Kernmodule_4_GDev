-- phpMyAdmin SQL Dump
-- version 4.9.4
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jun 22, 2021 at 03:55 PM
-- Server version: 10.2.32-MariaDB
-- PHP Version: 5.5.14

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
-- Table structure for table `HighScores`
--

CREATE TABLE `HighScores` (
  `id` int(6) UNSIGNED NOT NULL,
  `Username` varchar(30) NOT NULL,
  `Score` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `RegistredUsers`
--

CREATE TABLE `RegistredUsers` (
  `id` int(6) UNSIGNED NOT NULL,
  `username` varchar(30) NOT NULL,
  `email` varchar(30) NOT NULL,
  `password` varchar(30) NOT NULL,
  `reg_date` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `RegistredUsers`
--

INSERT INTO `RegistredUsers` (`id`, `username`, `email`, `password`, `reg_date`) VALUES
(1, 'Stals', '', 'Hum', '2021-05-27 09:29:57'),
(2, 'Harry', '', 'Potter', '2020-09-06 12:29:55'),
(3, 'Pietje', '', 'Puk', '2020-09-06 12:29:55'),
(4, 'Klaas', '', 'Baas', '2020-09-06 12:29:55'),
(14, 'Kees', '', 'Oost', '2021-06-20 19:42:46'),
(15, 'Otto', '', 'Hans', '2021-06-20 19:43:20'),
(16, 'Tjaard', '', 'VV', '2021-06-21 14:26:42'),
(17, 'Kulau', '', 'Snep', '2021-06-21 14:38:47');

-- --------------------------------------------------------

--
-- Table structure for table `SeminarOnderzoeker`
--

CREATE TABLE `SeminarOnderzoeker` (
  `id` int(6) UNSIGNED NOT NULL,
  `name` varchar(30) NOT NULL,
  `nAsked` int(11) NOT NULL,
  `nRating0` int(11) NOT NULL,
  `nRating1` int(11) NOT NULL,
  `nRating2` int(11) NOT NULL,
  `nRating3` int(11) NOT NULL,
  `nRating4` int(11) NOT NULL,
  `nRating5` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `SeminarOnderzoeker`
--

INSERT INTO `SeminarOnderzoeker` (`id`, `name`, `nAsked`, `nRating0`, `nRating1`, `nRating2`, `nRating3`, `nRating4`, `nRating5`) VALUES
(1, 'Peter', 5, 1, 0, 1, 2, 0, 0),
(2, 'Lisa', 5, 0, 1, 2, 3, 1, 0),
(3, 'Bart', 5, 0, 0, 2, 1, 2, 0),
(4, 'Bastiaan', 5, 0, 0, 1, 1, 1, 0),
(5, 'Denise', 4, 0, 0, 1, 1, 3, 0),
(6, 'Dirk', 4, 0, 1, 2, 0, 1, 0),
(7, 'Pedro', 4, 0, 1, 0, 2, 0, 0),
(8, 'Kaitlyn', 4, 1, 1, 0, 1, 0, 0);

--
-- Indexes for dumped tables
--

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
-- Indexes for table `SeminarOnderzoeker`
--
ALTER TABLE `SeminarOnderzoeker`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `HighScores`
--
ALTER TABLE `HighScores`
  MODIFY `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `RegistredUsers`
--
ALTER TABLE `RegistredUsers`
  MODIFY `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `SeminarOnderzoeker`
--
ALTER TABLE `SeminarOnderzoeker`
  MODIFY `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
