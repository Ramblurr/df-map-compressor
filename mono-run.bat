@echo off
set path=C:\Program Files\Mono-1.2.6\bin;%path%
cd DwarfFortressMapViewer\bin\Release
mono --debug DwarfFortressMapCompressor.exe | more
pause