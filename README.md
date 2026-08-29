# Daltonmonitor
Free and open source management tool to visualize DALTON lessons in a static html file. Perfect for schools with a dalton like system

<img width="100%" alt="grafik" src="https://github.com/user-attachments/assets/c17e3fc0-9465-418d-b6e1-3fb311979bef" />


## Dalton
In dalton, students have multiple dalton lessons over the day, one for each lesson in which they have dalton. During dalton lessons, students can choose the room they go to freely. That way they can also choose the teacher they go to, which in theory could be used to ask questions outside of normal lessons if you don't understand a specific assignment. During dalton lessons, students work on their dalton tasks, each teacher gives gives a specific dalton task for each week, for each relevant signature. 
They are free in the choice of when and where they want to do the assignment.

## Why
As our school is a DALTON school, we needed a way to display where students can go during DALTON lessons. Normal school software doesn't support such complex systems like dalton, where students can choose their room & the teacher freely, this project solves this problem by generating a static html file from the untis timetable data. The generated html file can than be used on monitors in the school, so students can go to those monitors to plan their dalton lesson. Of course, the generated static html file can also be hosted on a special webserver, so students can access the plan from where ever they want, from their personal device or even school devices.

## Features
- Untis Compatibility: Read [UNTIS](https://www.untis.at/manual/ti_allgemeine-schnittstellen.htm) files with your timetable data
- Timetable: Everything a timetable needs, from substitution, to room changes to complete free days. Everything is supported out of the box!
- Workshops: mark specific lessons as workshop lessons and have them displayed in a special way
- Complete customizability: everything is changeable in the config. Change everything, like the whole look of the html page, what is shown as dalton lessons, which rooms belong to which floor and much more. Everything is customizable.
> [!NOTE]
> If you lack a feature, before suggesting, look into the config, as it's often already there in a config option!

## How to use
To use this application, either compile the application yourself by cloning the git repository or download the precompiled binaries. There are multiple versions of the precompiled binaries
- Installer (recommended): this will install your application cleanly with a setup manager. After installing the app you can just start it.
- Zip file: download the raw zip file and find `Daltonmonitor.Desktop.exe` to start the app.

After starting the app for the **first time**, a `config1.ini` is generated. This is your configuration file where you can configure how the tool behaves. To see the first generated html file, you **need to** set the GPU0xx path files, **otherwise you cannot generate the html**. Which GPU0xx files are which can be read on the [official UNTIS documentation](https://www.untis.at/manual/ti_allgemeine-schnittstellen.htm). Theoretically only the GPU002 is needed, the others are recommended to use all the features of the app. GPU014 for example is required for substitution to work.
> [!CAUTION]
> Without setting the GPU paths correctly, the tool won't generate any static html page.

After setting the correct GPU paths, you can hit `Start` to start the tool, in the background, it will automatically scan for updates to the GPU paths, when there's an update, it will update the html file.
<img width="100%" alt="Captura de pantalla 2026-08-29 192013" src="https://github.com/user-attachments/assets/5798168a-8929-4d0e-bc4d-89ce5bf6d651" />

## Who
This software was written by 2 students attending a DALTON school, so we know exactly what problems we had to solve with this project. We also had close contact with the school IT management to get insides into the IT management's workflow to optimize this tool to be as easy as possible, while still giving enough control to the IT service.

Development was divided into the frontend (design) and the backend (the actual tool & logic) part of the application.
- Backend: [ArhiGames](https://github.com/ArhiGames)
- Frontend: [Finnick4](https://github.com/Finnick4)
