# convert2TraceSM
Extract SIP messages and convert them to Session Manager traceSM format

## Version

2.0.1.1

## Features:
- Extract the SIP messages from Workplace client for Windows and convert them to a file that can be opened using traceSM

## Supported platforms

- Microsoft Windows x64
- macOS x64 (Intel)
- macOS ARM
- Linux x64

The .net binaries are packed with the executable

## Documentation
- `convert2TraceSM` -h for details

```console
Usage:
	 -a	 Client type the debug logs belong to. [Mandatory!] Options are:
	 		windows - for Workplace for Microsoft Windows

	 -i	 input log file name. [Mandatory!]
	 	 For Windows Client: the filename is UccLog.log- e.g., UccLog.log, or UccLog.2019-02-19.2.log
	 	 For iOS Client: the filename is Workplacexxx.log- e.g., Workplace 2019-02-22 23-25.log
	 	 If filename contains spaces, use "" - see example below

	 -o	 output traceSM filename. [Mandatory!]
	 	 If filename contains spaces, use "" - see example below

	 -h	 print help information. Cannot be used with any other option
	 -v	 print version information. Cannot be used with any other option

Examples:
	convert2TraceSM -a windows -i UccLog.log -o ucclogtraceSM
	convert2TraceSM -a windows -i "UccLog Workplace.log" -o "ucclogtraceSM.1"
	convert2TraceSM -a windows -i "c:\temp\UccLog.log" -o "c:\temp\ucclogtraceSM"
```

## Usage
There is no installer. 
	- Either copy the convert2TraceSM.exe to where the UccLogs are located at or
	- pleace the convert2TraceSM.exe in a folder and add the path to the Windows PATH environment variable

To use:
	- convert2TraceSM -a windows -i UccLog.log -o UccLog.traceSM
	- copy the UccLog.traceSM to a Session Manager
	- open the UccLog.traceSM with traceSM (traceSM UccLog.traceSM)

Note: if the filenames contain spaces, or paths, then enclose them with ""

## Environment
The code is written in C# and .Net 10. The .Net is embedded in the executable to minimize the need to have it installed prior to running

## Icon
<a href="https://www.flaticon.com/free-icons/convert" title="convert icons">Convert icons created by iconixar - Flaticon</a>