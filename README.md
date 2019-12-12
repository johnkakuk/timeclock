# timeclock
A small command line utility used for tracking work hours.
<br>
<br>

## About
I downloaded Mora for my Mac for the same purpose, but wasn't able to get it installed, so I wrote this instead. It's pretty unsophisticated for now, but it works perfectly for what I need.
<br>
<br>

## CLI Commands
The entire app functions from the console command line (very useful for people who spend a lot of time in terminal). The commands to operate the app are very simple for the time being:

### clock
Simply type clock to start the utility with no arguments. If you are not clocked in, this command will print out a four-line report with the following information: Your clock status (in this case, clocked out); the last time you clocked in; the last time you clocked out; and the total time clocked during this session.

If you are clocked in, the app will report your clock status (in this case, clocked in); the time you clocked in; how many hours you've been working (rounded to the nearest hundredth of an hour).

### clock in
This will begin tracking your time and mark your clock status as *clocked in.*

### clock out
This will end your tracked time, mark your clock status as *clocked out*, calculate the time you added during this work session and add those hours to the master log.

### clock total
This command will add up the total amount of time currently stored in the master log, rounded to the nearest hundredth, and print it on screen.

### clock reset
This command clears the master log and resets your hours worked counter to zero.
<br>
<br>

## Installing - MacOS
### Copying Files
Copy all files located in:

`/bin/Release/netcoreapp3.0/`

Paste these files in your desired install directory. The utility will work wherever you put it.

### Creating Terminal Commands
In order to use this utility properly, console commands need to be provided for. To accomplish this, follow these steps:

1. Create a file of custom aliases and functions for your terminal.<br>
   `$ touch ~/.myCommands.sh`<br>
   `$ nano ~/.myCommands.sh`<br><br>
2. Add the following function to your new fies.<br>
   `# timeclock command`<br>
   `function clock() {`<br>
   `/Applications/timeclock/timeclock $1`<br>
   `# Replace the path your install dir`<br>
   `}`<br><br>
3. Update/create the .bashrc file to point to your new functions file. (If using zsh, swap for .zshrc)<br>
   `$ nano ~/.bashrc`<br>
   New information:<br>
   `source ~/.myCommands.sh`<br><br>
4. If using bash, update/create the .bash_profile file. Not sure why it's needed, but it is.<br>
   `$ nano ~/.bash_profile`<br>
   New information:<br>
   `if [ -f ~/.bashrc ]; then`
   `   . ~/.bashrc`
   `fi`

## Upcoming Features/Requirements
### Script to Add Console Commands
The next order of business is to write a script that will automatically create the necessary files to add these console commands to the user's machine (or append the relevant commands to existing files, if applicable). This entails creating a file containing a console function, as well as creating or updating the user's .bash_profile and .bashrc files. However, this does not account for users who have switched to zsh, the new standard console for MacOS since the Catalina update.

### Multiple Projects
I hope to add support for tracking time on multiple projects in the future.
