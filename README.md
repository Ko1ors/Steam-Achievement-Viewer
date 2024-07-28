# Steam achievement viewer

<div>
  <p align="center">
    <img src="Logo.png" width="400"> 
  </p>
</div>

Steam achievement viewer (SAV) is an application that helps players to keep track of their achievement progress for different games.

## Table of Contents
* [General Information](#general-information)
* [Technologies Used](#technologies-used)
* [Features](#features)
* [Login](#login)
* [Setup](#setup)
* [Usage](#usage)
* [Project Status](#project-status)
* [Room for Improvement](#room-for-improvement)
* [Acknowledgements](#acknowledgements)
* [License](#license)


## General Information
The application makes it easier to view all player's achievemts by gathering information about achievements for all of Steam library games in one convenient UI.


## Technologies used
- .NET 8.0
- WPF (Windows Presentation Foundation)


## Features
SAV provides users with functionality like:
- Viewing achievements for a specific game.
- Check the latest achievements of the player.
- View the player's rarest achievements.
- Track progress towards completing all achievements for a game.
- View closest achievements for a specific game.
- View closest achievements across all games in the library.
- View the list of completed games.

## Login
In order to see all achievements user have to provide their Steam ID. It can be found By checking your steam account details:
1. Click on your profile icon in the upper right corner.
2. Click on "Account details".
3. Your Steam ID is displayed beneath your account name (highlighted in green).

<img src="images\steam_id_example_SteamProfile.png" width="550"> 

## Setup
> For developers:
- Install Visual Studio 2022 with .Net Development Pack (SDK).
- Clone the repository.
- Open the cloned solution in Visual Studio 2022.
- Build the SAV solution.

>For users
- Download the latest release from the project page on Github
- Unarchive the downloaded RAR file.
- Run SteamAchievementViewer.exe.


## Usage

Once a user has entered a steam id, we can see a process of loading all of player's achievements:

<img src="images\image_1.png" width="700"> 

Let's say we want to check Closest Achievements for the Witcher game. We can do it by accessing the CLOSEST ACHIEVEMENTS tab and searching for a game like so:

<img src="images\image_2.png" width="700">

If we want to check all closest achievements we have to click ALL CLOSEST ACHIEVEMENTS tab:

<img src="images\image_3.png" width="700">

In order to check our last achievements we can click on the LAST ACHIEVED tab:

<img src="images\image_4.png" width="700">

To view all rarest achievements we have to click on the RAREST ACHIEVED tab:

<img src="images\image_5.png" width="700">

## Project Status
The project is currently in progress.


## Room for Improvement
The core functionality of the project is built. Future plans include enhancing existing features and developing new ones.
Check the [main issues](https://github.com/Ko1ors/Steam-Achievement-Viewer/issues) to see current and upcoming tasks.

## Acknowledgements
- Many thanks to [Steam](https://store.steampowered.com/)


## License 
This project is open-source and available under the MIT license
