A.Q.S.S. - AutoQuickSave System

This is a small mod to do quicksaves on an automatic, scheduled basis.  It can also do a quicksave when you launch a vessel.

Features

Selectable interval for quicksaves
Quicksave on Launch
Templated names for the quicksave files
Automatic expiration of old quicksaves based on specified criteria
Audio file playable when a quicksave is done

Usage

Click the button [buttonImage]

There are two buttons, one for the quicksave options, and one for the Sound Options

The filenames are created using a template.  The template for both Launch and Quicksave are shown, and the line right below each is showing the final result.  Click the Template Info button to read about the available tokens which can be used in the templates

Automatic Purging of old files

The bottom three lines are the criteria to determine when to delete files.  Two things to note:

1. Launch quicksaves are never purged.
2. The mod will only delete files which match the prefix AutoQSave_ and which fit the rest of the criteria


Sound Options

An audio file can be played automatically whenever a quicksave is done.  The available sounds are listed in a list, and you can add more.  To add your own sounds, copy the audio file into the folder:  AutoQuickSaveSystem/Audio, close and open the window and the new files will be shown in the list.
The currently selected audio file is highlighted with green letters.  Click the little triangle at the right of each line to hear the sound the audio file plays


Special note regarding Quicksave on launch

This depends on the initial staging event of a newly launched vessel.  What this means is that if you launch a rover and drive it away, no quicksave will be done of that launch

Note regarding the voices
There are a number of website which n do a text-to-speech output.  A few which provide good output are:
		https://www.naturalreaders.com/online/
		http://www.fromtexttospeech.com/
		https://ttsmp3.com/
		https://www.text2voice.org/

Special note regarding the S.A.V.E mod
The S.A.V.E. mod renames save files and quicksave files.  You need to disable the autosaves to prevent it from interfering with this mod
