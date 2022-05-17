# Steam achievement viewer

<div>
  <p align="center">
    <img src="Logo.png" width="400"> 
  </p>
</div>

Steam achievement viewer (SAV) is an application that helps players to keep track of their achievement progress for different games.

## Table of Contents
* [General Info](#general-information)
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
- .NET Core - version 3.1
- WPF


## Features
SAV provides user with functionality like:
- Viewing achievements for a specific game.
- Checking player's last achievements.
- Viewing player's rarest achievements.
- Checking how far are a player from completing all achievements for a game.
- Checking player's closest achievements for a specific game.
- Checing player's closest achievements across all of games in the library.


## Login
In order to see all achievements user have to provide a steam id. It can be found By checking your steam account details.
>Simply click on your profile icon in upper right-corner and then click account details, right beneath your account's name you will find steam id.

<img src="images\steam_id_example_SteamProfile.png" width="550"> 

>(id is marked by green rectangle)
>

## Setup
> For developers:
- Install Visual Studio 2022 with .Net Development Pack (SDK)
- Clone repository
- Open cloned solution in VS 2022
- Build the SAV solution
- ????
- profit

>For users
- Download latest release from  project page on github
- unarchive donwloaded rar file
- run SteamAchievementViewer.exe


## Usage

Once a user has entered steam id, we can see a process of loading all of player's achievements:

<img src="images\image_1.png" width="700"> 

Lets say we want to check Closest Achievements for Witcher game. We can do it by accessing CLOSEST ACHIEVEMENTS tab and searching for a game like so:

<img src="images\image_2.png" width="700">

If we want to check all closest achievements we have to click ALL CLOSEST ACHIEVEMENTS tab:

<img src="images\image_3.png" width="700">

In order to check our last achievements we can click on the LAST ACHIEVED tab:

<img src="images\image_4.png" width="700">

To view all rarest achievements we have to click on the RAREST ACHIEVED tab:

<img src="images\image_5.png" width="700">

## Project Status
Project is: _in progress_ 


## Room for Improvement
Core functionality of the project is built. But we are planning to improve current functionality and develop new features.
You can check main issues here: https://github.com/Ko1ors/Steam-Achievement-Viewer/issues

## Acknowledgements
- Many thanks to store.steampowered.com


## License 
This project is open source and available under the MIT license
