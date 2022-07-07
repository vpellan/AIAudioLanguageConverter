# Alien Isolation Audio Files Language Converter

Tool that changes the language of Alien Isolation .pck files by replacing .wem files while keeping their ID, and updating .pck header.

## Introduction

This tool was made right after making [Alien Isolation Cutscenes Language Changer](https://github.com/JeanBombeur45/AICutscenesLanguageChanger)

I gained knowledge while coding these tools, and i know that there is much likely way simpler ways to change the language of both audio and cutscenes files without having to change the whole OS language.
But I often realized after the project was completed. And the original goal of these tools were just for me to play Alien Isolation with original voices without changing my OS language,
which was met after making these tools, so I do not think I will make more optimized tools for this purpose.

All the files in `Language(Country)` folders does not contains any audio except `Player_Vocalisations.bnk`, but every audio file in this sound bank are the same for every language.
These `.bnk` sound banks contains what I believe to be spatial sound effects informations, and WEM IDs (and you cannot change them). Only `.pck` files are modified.

### Changing the language
```
 AIAudioLanguageConverter change <InputPath> [options]
```

| Arguments   |                                                                        Description |
|:------------|-----------------------------------------------------------------------------------:|
| InputPath   | File or folder containing .pck files<br/> (usually `<path-to-game>/data/UI/sound`) |

| Options                           |                                                                                 Description |
|:----------------------------------|--------------------------------------------------------------------------------------------:|
| -s &#124; --sys-lang <SYS_LANG>   |                                                 Specify system/game language **(required)** |
| -d &#124; --dest-lang <DEST_LANG> |                                                     Specify desired language **(required)** |
| --no-backup                       | Do not make a backup of files. Warning: You will not be able to change the language anymore |
| -y &#124; --yes                   |                             Automatically accept backup overwrite *(prioritized over --no)* |
| -n &#124; --no                    |                                                       Automatically refuse backup overwrite |
| -? &#124; -h &#124; --help        |                                                                      Show help information. |


| `<SYS_LANG>` and `<DEST_LANG>` |               Language |
|:-------------------------------|-----------------------:|
| 0                              |                English |
| 1                              |                 French |
| 2                              |                Italian |
| 3                              |                 German |
| 4                              |                Spanish |
| 5                              | Portuguese (Brazilian) |
| 6                              |                Russian |


### Restoring original .pck files

**Warning** : if you have deleted your backup files, or used `--no-backup`, you cannot restore original files.
```
 AIAudioLanguageConverter restore <InputPath>
```

| Arguments   |                                                                                                      Description |
|:------------|-----------------------------------------------------------------------------------------------------------------:|
| InputPath   | File or folder containing pck files<br/> (usually `<path-to-game>/data/UI/sound`) and containing `backup` folder |

## Known Issues

Some files are missings from certain languages (they were not dubbed, or forgot to add them during compilation as they are not even present in `SoundbanksInfo.xml`.
In these cases, the audio from the original language is kept. It is still a very rare case.

Here is the list of missing audios :

**Italian** : `A1_SS2_S_SFC_ALI_D.WAV` </br>
**Spanish** : `A1_CV5_DEALIEN_E.WAV` and `A1_M2201_RIP_2911.WAV` </br>
**Portuguese** : `A1_M2201_RIP_2911.WAV` </br>
**Russian** : `A1_M3401_RIP_4927.WAV`

## License

AIAudioLanguageConverter follows the MIT License. It uses code from [RingingBloom](https://github.com/Silvris/RingingBloom).
