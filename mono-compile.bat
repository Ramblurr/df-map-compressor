@echo off
set path=C:\Program Files\Mono-1.2.6\bin;%path%
cd DwarfFortressMapViewer
gmcs -out:DFMapCompressorMono.exe *.resx -warn:2 -d:MONO -debug Properties/AssemblyInfo.cs Common/CRC.cs LZ/*.cs LZMA/*.cs RangeCoder/*.cs *.cs -pkg:dotnet -reference:ICSharpCode.SharpZipLib -reference:AxInterop.ShockwaveFlashObjects -win32icon:dfmvicon.ico -target:exe | more
pause
