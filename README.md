# DF Map Compressor / Viewer

This program was made by SL (shadowlord13@gmail.com) and powers the maps
viewable at the [DF Map Archive](http://mkv25.net/dfma/)


[Source](https://raw.githubusercontent.com/Ramblurr/df-map-compressor/master/DwarfFortressMapViewer/dfmap-readme.html "Permalink to Dwarf Fortress Map Compressor (3.3.4)")

# Dwarf Fortress Map Compressor (3.3.4)

This program was made by SL (shadowlord13@gmail.com), and you can find it on
the Dwarf Fortress File Depot or at <http: shadowlord13.googlepages.com=""
dfmap-index.html="">. It was made in C#, and requires that you have at least
the .NET framework 2.0 or Mono 1.2.6 (or newer - run with mono --debug
DwarfFortressMapCompressor.exe) installed to use it.

The basic purpose of this program is to losslessly compress an image which is
made up of many fixed-size images which are used many times, into a format
which is far smaller than anything you could do with a normal image format
(like png, jpg, gif, or tif). The program was written for Dwarf Fortress's
fortress map images, but also works on DF's world map images, and should
theoretically work on other tiled images as well (if you tell it the right tile
size - though it can usually figure out your DF images' tilesizes itself, if
they are in your DF folder).

The program does not need to be installed - it should run where-ever you put it
(as long as you don't move it away from the other files which are in the zip/7z
file with it).

In simple mode, the program shows some simple instructions and checkboxes and
four buttons: [Compress image to .fdf-map file], [Visit Markavian's DF Map
Archive], [Preview .fdf-map (before uploading)] (preview won't work in mono)
and [Switch to Advanced Interface]. It basically comes down to clicking the
first two buttons in order and then uploading the .fdf-map file at the Archive
website (the second button takes you there). You can use the third button to
preview your fdf-map after you've made it, of course.

There are several things this program can do:

1. Encode (Create) a df-map file (which you could then send to a friend who has this program, and they could make an image from it) from any of these:
    * a world or fortress map BMP, or
    * a fortress map BMP written by DF 0.27.169.32a or newer (or possibly PNGs), which can be one of a set of several images representing different z levels. The DF Map Compressor will identify other images from other z-layers of the same map and time (the filenames are identical except for the z-layer number), and will compress them all together as a multi-level map. It will also rename them once it is done so that their names begin with "Done-", so you can more easily see what you've already compressed.
    * a fdf-map, or
    * a png. You should keep it at full color, preferably. Using 256 colors, 16 colors, 2 colors, or greyscale is not recommended. Do not rescale, dither, or antialias the image, either, as any of those will destroy the duplicate image tiles which are needed for this program to be able to compress the image. (Previous versions of this program supported making df-maps from *some* PNGs, but it was somewhat of a hidden feature since it didn't work unless the PNG was saved as 24bpp RGB. Now the program can convert any kind of PNG to that in memory, so it should be able to load any kind of PNG now - unless you've damaged the image data).
2. Decode a df-map or fdf-map file into a PNG (or a zip file holding multiple PNGs (this may be changed to just write images)), which will be much larger than if you had saved it with an image editing program like Photoshop, Paint Shop Pro, etc. (Why? Because I used microsoft's png writing stuff, instead of looking for and trying third-party png writing libraries for .NET),
3. Display the map from a df-map, fdf-map, or png, and let you scroll around and zoom in and out (If this uses the flash viewer, it is as fast as the DF Map Archive, but if you open something which it can't handle, it'll use the slowish .net-based viewer instead). You could also force it to open any image file, if you really wanted to. I wouldn't suggest trying to open a BMP this way, but it can be done.
4. Create a fdf-map from a df-map, bmp, zip, or png (same restrictions as encoding has). This is really just the same as encoding, except that the resulting file is slightly larger than a df-map, but readable by Markavian's flash map viewer.

Possible future ideas:

* Possibly a dll+lib along with sample code that exports the map from Kobold
Quest or Battle Champs. "<toadyone> if whatever you did worked with kobold
quest, it'd probably work with df". Functions: `beginWritingFdfMap(char * filename,
int tileWidth, int tileHeight, int permanentImages, int
duplicateImages), beginWritingZLevel(int zlevel, int tilesWide, int tilesHigh),
writeTile(unsigned char * image (32bpp tileWidth * tileHeight image data),
finishedWritingFdfMap())`. I'm pretty busy with other things though and it seems
to be working fairly well as-is (except for problems with mono on macs? Hmm.
DLLs would be useless on macs, though, but if I had a mac, which I don't, I
could perhaps compile them as the equivalent for a mac.)

Notable changes in 3.3.4:

* Fixed a bug which resulted in parts of some rough floors (such as obsidian floors) becoming blackness in exported maps.
* Fixed a bug which resulted in a "we were able to identify many tiles which were the same images with small differences. ... We identified 0 more tiles!" message when a color damage check was done on a map which turned out to have no color damage - such as speargroove, which is on a very tiny map.

Notable changes in 3.3.3:

* Changes to support big-endian computers (mainly for PowerPC macs running this in Mono).
* Updated included flash map viewer to latest version of DFMA flash viewer (although that's from june 2008).

Notable changes in 3.3.2:

* Summary: Fixed lots of bugs which were making the compressor not work in linux, and fixed problems with the GUI in linux. More detail:

I discovered [andLinux][1], installed it, installed [Mono 1.2.6][2] (from the .bin installer on the Mono website - it installed, and then it declared that there were 20 or so libraries missing and maybe I can't install thiings with the installer because of it, and then it said it was done installing. I went "Whaaa?" but mono seems to work.), and then tested the compressor in it. The compressor crashed with an exception caused by my having forgotten to convert s to / in one particular place before searching a string. I found more bugs after that. This release fixes all the bugs I found which were preventing it from working on linux (most of which were places where I tried to load files with \ in the path instead of /, causing it to be unable to find files), and cleans up the GUI bizarreness on linux (checkbox labels were unreadable due to what's presumably a bug in mono - I found a workaround, text overlapped other things because a different font was used due to linux not having the font that is normally used on windows for forms, button text ran over for the same reason, some text went off the edge of dialogs for the same reason, and popup message boxes showed all their text on one big honking huge line which made it entirely unreadable after a point - I wrote a simple function to split the text into multiple lines; Windows was doing that automatically, but linux wasn't). That said, I have only run it in andLinux, which uses ubuntu with KDE and runs it in windows, so I can't be certain that there aren't different GUI bugs in gnome which don't appear in KDE, or different ones in different versions of KDE, etc etc. I'd say poke me if you find any, but nobody even reported that it didn't work on linux at all (I'm not sure what to conclude from that. Maybe it's too hard to contact me (PM on the DF forums, or take the username you see in the URL of this website and slap @gmail.com after it to email me), or maybe nobody wants to go through the hassle of downloading and running the Mono 1.2.6 .bin installer, or maybe there just aren't that many people who would use the compressor on linux?).

Notable changes in 3.3.1:

Notable changes in 3.3:

* New automatic feature: Black Space Conversion (BSC): The compressor now automatically (if tile identification is enabled and it can find your font and color information) identifies all the random tiles which are placed in black space and converts them to black space tiles (they are originally dark grey %',.` on black background, or a red , on black background). This is automatically enabled (always), and takes almost no extra time to do.
* There have been additional tests done, sanity checks added for testing purposes, and bugfixes made with the tile-identification, duplicate tile checking, duplicate tile deletion, tile index reassignment, and so forth.
* The compressor allows you to change the color match sensitivity of the tile identification algorithm now. This defaults to 12, which is what it always was in previous versions. 12 usually works fine for most maps written by DF. If your maps are written out by DF with screwy non-identical colors (you will have a relatively high unique tile percentage - probably above 0.1% but below 2%) you may need to turn this up to get things to match regularly. Fedor's 1055 Mountainbanners map export, for example, needs a value of 150 or so (150 or higher gets them all, 100 doesn't) to identify all the non-graphical tiles in that map.
* The compressor checks to see if your unique tile percentage is above 0.1% now, and if so, it will ask if you would like it to turn up the color match sensitivity to 200 for this map in order to make sure to get all the tiles identified. I tried to have it explain the reason for that as best I could, and to make this as simple as possible. It will also notify you about how successful it was after it has finished identifying stuff and pruning duplicates.
* The compressor updates the progress bar while removing duplicate tiles now. It didn't used to because this ordinarily happened almost instantly, due to there normally being 0-6 duplicates. Fedor's Mountainbanners, however, had 2900 duplicates, when all the tiles that could be identified were being identified. It took a noticeable amount of time to clean up all the duplicates.
* Updated the included version of the flash viewer (for the preview feature) to version g2a.
* Here are some example filesizes from combinations of features from maps I tested this on. The green is what's standard in this version of the map compressor. The red is what was standard in the previous version. (features which require TileID, if (which is font/color information) are not listed without it). You can toggle TileID yourself. RLE will be in the next version, the DFMA doesn't support it yet. BSC is in this version and is always on. AC is not included because it wasn't as good as RLE, and together they gave no added benefit (both required different file format changes, I picked the best one and discarded the other to save effort (for Markavian, mainly)).

Test map 1, 288 x 288 tiles x 19 layers, 16 x 16 pixel font and graphics tileset: None(286 KB), TileID(284 KB), RLE(260 KB), RLE+TileID(258 KB), BSC+TileID(181 KB), AC+BSC+TileID(176 KB), RLE+BSC+TileID(170 KB), AC+RLE+BSC+TileID(170 KB).

Another map I tested this on was Fedor's mountainbanners, which has odd graphical color irregularities, seemingly due to fuzzy math in whatever determined the color of each pixel of each colored tile (but not the graphical dwarf/pet/etc art). The original fdf-map uploaded to the DFMA was a whopping 354 KB and had a unique tile ratio a couple orders of magnitude greater than a normal map. It takes a while to run tests on it because I have to turn the color match sensitivity way the hell up to correctly match all the 'duplicate' tiles which have different pixel colors but which were based on the same font character, background color, and foreground color. This map is 96 x 240 tiles x 55 layers, also with a 16 x 16 pixel font and graphics tileset. The filesize was chiefly due to the color weirdness. For this one, where TileID was enabled, I ran it with a color match sensitivity of 150. None(354 KB), TileID(141 KB), RLE(308 KB), RLE+TileID(121 KB), BSC+TileID(93 KB), AC+BSC+TileID(90 KB), RLE+BSC+TileID(76 KB), AC+RLE+BSC+TileID(77 KB). The one Fedor had uploaded to the DFMA was 354 KB. However, it didn't contain tile ID information at all (did he use the java version? an old version of the map compression? disable tile identification to save time? I don't know). However, the old version of the map compressor, if it was allowed to scan for font and color information (that's the TileID feature), would compress that to a file size of around 247 KB. That's mainly because the color match sensitivity threshold was locked to 12, so the TileID couldn't identify most of the tiles on this map due to whatever caused the color irregularities.

RLE is Run Length Encoding. BSC is Black Space Combining. TileID is the font/color information (adding it presumably only reduced the filesize from 286 to 284 KB on my test map because it enabled the compressor to identify and correct tiles which had black vertical bars covering part of them).

* Note: The RLE feature, which gives additional filesize reductions, is not included in this version of the DF map compressor because it requires fdf-map file format changes which the map viewer on the map archive cannot yet read (and Markavian doesn't have time to update it to read them in the near future, and I want to get the other sizable enhancements out in the meantime). It is fully implemented in the compressor, but the code for it is disabled. The next version of the compressor will be released with RLE enabled once the map archive can read the fdf-map files which are written with the RLE feature. About this feature: The RLE encoding is run-length encoding in the map index data for each map layer. RLE was already being used in the actual unique image pixel data to reduce its size (deflate mostly takes care of the rest of it), but not in the map (tile) index data. This cuts down the file size for large sections of identical tiles (which enhances the benefit from the black space conversion, in particular). This would be automaticaly enabled (always), and adds almost no time to the processing time (it occurs while writing out the map data at the end). Even if font/color information is missing, the RLE encoding still reduces the filesize a bit (from 286 KB to 260 KB for my test map, for example).

Notable changes in 3.2.2:

* Fixed the multi-layer filename parsing routine to handle filenames like 'Pineoaks the Woods o-10-pineoaks-spr-1064-1064-0.bmp', which are ones exported by DF's autosaver. It now looks at the first two and the last two dash sections ("Pineoaks the Woods o", "10", "1064", and "0") instead of the last 5 to determine whether it's correct, and requires that the filename have at least 5 dash sections to be considered a multi-level candidate.
* Fixed a bug in the tile identification code which caused the black-bar search to crash if the tile width and height were not the same (I accidentally declared the bool rowsBlanked and columnsBlanked arrays with tile width and tile height size, instead of tile height and tile width (respectively)).

Notable changes in 3.2.1:

* Fixed a bug which made the flash viewer not load.

Notable changes in 3.2:

* The compressor should be able to identify if you are using fullscreen now.
* The compressor will, if it can find your init file and font bmp, read in your font bitmap, load the color definitions from your init.txt, and analyze the unique tile images it finds to identify what font character and colors they are. (It won't screw up images from graphical tilesets, though) This information is then written into the fdf-map along with everything else. (This may enable Markavian to implement some interesting new features for the flash viewer)
* The compressor can identify and fix tile images which have vertical or horizontal black bars over them, by comparing them (except for the black bar part) to the other known tiles (This is much faster than comparing them to all the possible combinations of character, background color, and foreground color).
* Changed the unique-tile-ratio warnings to be more informative, and if you tell it to compress anyways, it will ask you if you actually read the warning message.
* Disabled the image renaming code, and added a checkbox to enable deleting images after they're compressed. Check it if you want the images deleted. If you don't, they won't be renamed or deleted.
* Added a checkbox for disabling the tile identification (It is run after every layer has been loaded and processed, and it adds a fair amount of time to the compression process for a small map (say, 3x3) with few layers, but for the 6x6 map I tested it on, compressing N images with the identification stage takes about as long as compressing N+1 images without the identification stage, where N is any number you like). If you don't care about possible advanced features for the map archive, you can just check this checkbox and it won't be done.
* The compressor will, in addition to looking for the init file in data/init and the font in data/art, if it cannot find one or both of those files in those folders, will also look for them in the folder the image(s) are in. So you should be able to copy the init.txt and font bmp into a folder with your images in it, if you aren't putting the images in your DF folder.
* In addition to noticing whether you have the init file set to go fullscreen or not, it also checks for it being set to ASK or PROMPT (I don't remember if both or just one of those are valid, so it checks for both), and if it is, it will ask you whether you exported the image from fullscreen DF.

Notable changes in 3.1:

* If the flash map viewer fails to load because flash9.ocx wasn't found by windows/.net/whatever, you should get a message box with some possible tips now. The message box basically says to look in c:windowssystem32MacromedFlash, and if flash9.ocx is not there, look for NPSWF32_FlashUtil.exe there and run it if it exists. If flash9.ocx IS there, then you can try running 'regsvr32 c:windowssystem32MacromedFlashflash9.ocx' from Windows' [Start]-&gt;[Run] instead.
* Fixed bugs that were preventing converting fdf-map files to df-map files.
* The compressor should now be able to identify map images written by the new DF as being such even if their save file names dont't include "region."
* If you decode an fdf-map or df-map, the filenames of the images written now have the proper names ('Flightwheeled-15-region4-1057-15492.png') instead of being named '1.png', '2.png', etc. They'll still all be stored in a zip file (you can extract them when you want to do something with them).

Notable changes in 3.0:

* The program now checks your window width/height from init.txt (if run on a map in your DF folder) against your font size, and warns you if the width or height are too small (because if they are, (old versions of) DF bugs out and puts black banding and tile image squishing in the outputted bmp). Note that the program does NOT directly detect black banding - it detects what causes it in your init file. That means that if the program has to ask you the tile size, it won't notice banding.
* Note: There is a new black vertical bar bug in DF 0.27.169.32a (existing up to at least 33g) which is not detected by the previously mentioned check. However, it is covering part of a column of tiles instead of adding pixels, so it doesn't cause problems (besides being unsightly). Version 3.2 of the compressor can, if tile identification is enabled, usually identify what these tiles were supposed to be, and put the proper tile image for them into the fdf-map or df-map instead of the damaged tile image.
* Implemented multi-z-level map format support:
    * Multi-z-level image input format is currently expected to be one image or a set of images which have hopefully not been renamed by the user. The images can be any image format.
    * Multiple images for different z levels of the same fort at the same time are identified when you specify one of the images to compress, and all of them are then compressed as a multi-level fdf-map or df-map. The integrated (slow) viewer can display it with a up/down level selector for switching viewed levels.
    * The file format for the fdf-map and df-map files has been changed to support multi-z-level maps. All fdf-map and df-map files created by this version of the compressor will be written in the updated format (unless converted directly from another df-map or fdf-map file). Older versions of this program will be unable to read those, and may crash if you try. This version and newer versions should be able to read older df-map and fdf-map files just fine (The first variable in the file is repurposed into a (negative) version number for the new format, whereas it held the number of unique tile images in the old format).
    * Fdf-map or df-map files can be converted back into either a zip file (this may change) or a single image - which one is done depends on whether there is more than one z-level in the file. The program determines which it should be before asking you for the filename of the output file, so you'll find that the file save dialog indicates whether it will be a zip or png file.
* The program can now rapidly convert fdf-map files to df-map files and vice versa - it now decompresses the file's contents through the one decompressor (LZMA or LZW) and writes it back out to the new file with the other one. Previously it decompressed, converted it to an image, and then converted that back into the df-map/fdf-map, and wrote it out with the chosen compression, which was a needless waste of memory and CPU time. Note: This will NOT re-analyze the data, so it will NOT add new features (such as tile identification) to it. You would need to decompress the fdf-map/df-map, extract the images from the zip file, copy your init.txt and font bmp into the folder with the images, and then compress them again with tile identification enabled (there's a checkbox to disable it, so don't check it if you want this).
* A local copy of the DFMA flash viewer is included with the compressor, so if you want to preview an fdf-map file before you upload it, you can. If you try to preview a png, bmp, or df-map file (basically, anything the flash viewer can't read) it'll open in a slow c#-based viewer instead. The compressor will attempt to inform you of possible solutions if it cannot load the flash9.ocx file.
* I tested varying .zip compression levels containing .net-written png and bmp files for the fdf-map/df-map to zip conversion, and determined that the best one was to write out pngs inside a compression level 1 zip file. The reason for that is primarily that it was fastest and still gave fairly decent compression - getting better compression required writing bmps, and took about three times as long. I chose speed over size here because it would be fairly pointless to store your maps as zip files or transfer them as such when you can just use the much smaller df-map or fdf-map formats. I tested this on a 500 KB fdf-map which was made from six fortress map images, with the png zip being around 19 MB, and the 3-times-longer-to-write BMP zip being around 15 MB. The [results of that testing are here][3].
* I also tested several different ways of having RLE-encoding allowed in the tile index data, and decided against using any of them in the fdf-map/df-map format. One of them used -20 10 to indicate 20 copies of tile 10, and the others used a flag on the tile # to indicate that an extra variable of a different data size (I tested 1, 2, and 4 bytes) followed it and indicated the number of duplicates. For fdf-map output, I found that a tiny benefit could be obtained by using the second method with 1 byte RLE lengths, or an even smaller benefit from using 2 bytes or the first method, and a size increase with 4 bytes. However, for df-map, 2 bytes and method 1 resulted in no notable size change (or possibly up to 1 KB size reduction), 4 bytes produced a slight size increase, and 2 bytes produced a more notable but still small size increase. I also determined that LZW and LZMA both already gave a far more notable size reduction in the fdf-map and df-map files when they contained many repeating tiles. The best performance for the RLE addition in index data gave, for fdf-map files, about a 5.26% size decrease in the test map with many identical tiles (It had differentiated grass turned off), or a 3.12% size decrease in the same test map with differentiated grass turned on. [Results of that testing are here][4]. Note: These tests were not done with multi-level images since the 3d DF wasn't released yet at the time. Results of re-doing the testing on actual multi-level images may indicate that this would be a useful change.
* Revised how the progress dialog's status text is shown. Now it doesn't show a (frequently inaccurate) "Step 2/5" indicator, but just describes what the current step is. If it is working on a multi-z-level map or images, however, it will show the current and total amount of z-levels in the status, in addition to the step description.
* Implemented some improvements which should free memory more rapidly after it is no longer needed, which helps keep multi-z-level work responsive and quick. I'd recommend you have 1 GB of RAM (which is what I have) or more, and not have any big memory hogs open while using the program to convert images to fdf-maps, to df-maps, or from those, or while viewing a map. It should work with less memory, theoretically, or if you have some memory hogs open, but will probably be slower, and if you have too much open, at some point Windows or .NET says "Oh, no more memory is available" instead of swapping stuff to virtual memory (on the hard drive). If that happens, you'll probably see an out of memory exception. Oh, and you will very likely be unable to compress a max-size map unless you have helluva lot of RAM.
* The program will complain now if you try to convert a df-map file into a df-map file, or an fdf-map file into an fdf-map file (since it would do nothing). If you have fdf-map files in the old format, there's no need to convert them to the new one - this program, and the DFMA flash viewer, can read both the old and new formats. Of course, now that there's new data (tile identification) being stored in the fdf-map files, you may actually want to do this. If so: You would need to decompress the fdf-map/df-map, extract the images from the zip file, copy your init.txt and font bmp into the folder with the images, and then compress them again with tile identification enabled (there's a checkbox to disable it, so don't check it if you want this).
* You can compress CMV movies into either the CCMV or FCMV formats now. (FCMV is smaller, but at the time this was written, is not yet supported by the DFMA movie viewer, which is why the option to make CCMVs is still available) For example, the tragedy mule CMV is 1637 KB, the CCMV is 616 KB, and the FCMV is 292 KB. However, at present, the Map Archive lets you upload only CMVs, and will compress them to CCMV itself, so this is mostly for demonstration purposes.

Notable changes in 2.0:

* A default simple interface separate from the advanced one.
* If you ask the program to encode an image that isn't in your root DF folder, it will now ask you what size the tiles are. If it IS in your DF folder, it still gets the tile size from reading your init file and looking at the font bitmap. 1.0.* required the init file and font file to work - this version doesn't.
* If you have no idea what size the tiles in a map image are, you can hit 'guess' when the program asks you. It will try to figure it out by looking at the 3 rightmost columns of tiles on your map, but it's slow and might not get the right answer. It almost certainly won't get the right answer if you use it on anything other than a local fortress map (e.g. it won't work well on a world map).
* The program's buttons in the advanced interface actually show what types of files they read, and what they output.
* The file format was modified slightly, rendering this version generally incapable of reading old df-fort-map files, but a particular bug fix would have broken compatibility with some of them anyways. You can use 1.0.1 to turn the df-fort-maps into pngs, and then have 2.0 read the pngs to make new df-maps or fdf-maps, if you like. About the bug: The skew problem which was patched (badly, it turned out) in 1.0.1, should now be completely fixed. It turns out that the "fix" (in the bitmap input and splitting routines) for skewing in images with improper widths in 1.0.1 did not do what it was intended to, but APPEARED to because it counteracted (without fixing) another bug in the bitmap output routines, making the resulting image look correct when drawn or converted to PNG by this program. However, the df-fort-map files written from those images were actually flawed. With both bugs fixed, df-fort-map files written from images with improper widths by 1.0.1 would appear skewed if you could view them in 2.0. The reason is that those files were written in such a manner that the bitmap-output routine's skew bug was necessary for them to be drawn correctly.
* A new file type, fdf-map, was created specifically for Markavian's flash viewer. This format is fundamentally identical to the df-map format, except that they use different compression algorithms. (He couldn't read LZMA compression in a Flash app, even with ActionScript 3, due to lack of an LZMA library for AS3)
* The viewer part of this program might be slightly faster than before, but if so, not by much. Markavian's flash viewer is much faster.
* This should theoretically be able to compress any tiled image now, as long as you enter the correct tile width and height. (The person viewing it from the df-map or fdf-map does NOT need to enter a tile width or height, since it is saved in the df-map/fdf-map file.)
* The program will ask for confirmation to continue encoding if you use it on an image which appears to have had its compressibility damaged (or which isn't really tile-based). If you want a demonstration, drop a fortress map to 8 bpp with dithering enabled, and save it as a png. Then feed that to the encoder. Even doing it with dithering off seems to damage it some (raising the final filesize of the df-map or fdf-map file). It's best if you feed the program full-color (24-bpp) images, which is what the BMPs are that Dwarf Fortress writes.

How to figure out tile sizes of images when you don't know them: You can look for a lonely square tile in the image with the GIMP or some other image editor, or a couple identical tiles next to each other, and measure their size, to find out. Or, if it's an image you asked DF to make, you can look at the font file that DF used (divide the font file's width and height by 8 to get the tile width and height).

This CAN NOT read your saved games, or your world map files, or DF's memory. Toady hasn't posted these file formats, and has said that he is still changing them. It does, however, read your DF init file and look at your font image to find out what size your tiles are (if it can find them). This enables it to compress the map more effectively. If it can't find those files, it will ask you what size the tiles are. It should be able to find them if they are in the same folder as the images, or if the init file is at imagefolder/data/init and the font is at imagefolder/data/art, where imagefolder is the folder that the image(s) you are compressing is in.

As of version 3.2, the compressor can check your init file to determine if you saved a map in fullscreen mode or windowed mode - if it can find the init file.

This program uses the unmodified [LZMA SDK][5] for compressing df-map files, and [#ZipLib][6] for writing out .fdf-map files (for Markavian's flash viewer). The fdf-map files are slightly larger than the same map saved as a df-map, but fdf-map is the only one readable by Markavian's flash DF map viewer (because we couldn't find an LZMA decompressor for Flash's ActionScript 3). (The fdf-map is identical to the df-map except for how it is compressed)

Older versions of DF has been observed to sometimes write out malformed images - this program handles ones which have extra columns of pixels (which seems to be fairly common), but cannot fix images with large black bars in between the visible map data, or other malfunctions. If your encoded map doesn't display properly, first check the original bmp to make sure the problem isn't in that. If the problem IS in the original bmp, then it's not a bug in this program or the viewer, it's a bug in DF. (The newer DF versions may or may not write out malformed images)

About tile identification: Unless you check the 'Do not try to identify tiles' checkbox, if the compressor can find your init and font files, it will analyze the tile images to determine what font character and foreground and background color they are. This information is then (a) written to the fdf-map or df-map file to enable applications like the DF Map Archive's flash viewer to make decisions based on what the tile is, and (b) used to auto-correct tile images which have black bars over part of them (black vertical bars have been over parts of some tiles in ALL of the images I've exported since 32a, personally). This information could in theory also be used to allow anyone to specify their own font image and colors to view a map in (if it includes tile identification information), regardless of what the font and colors the map was originally in.

When you tell the compressor to compress an image which is one of a set of images from a fort for different z-levels, the compressor automagically identifies the other images that go along with it, and will process and encode all of them together into the fdf-map with their proper z-levels denoted in the file.

The compressor needs approximately the same amount of RAM regardless of whether you are encoding 2 z-levels or 20. The actual size of an image determines how much RAM it needs, not the number of images being processed (because it's smart about how it does the work).

Back in version 1.0, I tested this with the stock DF font without graphics mode, in both windowed and fullscreen, and also with the object tileset and font which I normally use (Dystopian Rhetoric's, with the SuperFoulEggFont which comes with it, but slightly corrected to fix a couple mistakes in the font). It worked for all three. Results of my testing are below (all using the same saved game):

Filesizes and times from testing, using Dystopian Rhetoric's object tileset and the (slightly corrected) SuperFoulEggFont which came with it:

* local_map-1-1067-11046.bmp written by Dwarf Fortress - 150,481 KB
* local_map-1-1067-11046.df-map written by this program - 81 KB (took about 13 seconds to load the BMP, process it, encode it, compress everything with LZMA, and write it out as a df-map)
* local_map-1-1067-11046.png written by this program - 4061 KB (took 8 seconds to load the df-map, process it, and write the png) (unless there wasn't enough free RAM, in which case it took about 40 seconds due to windows having to move stuff to virtual memory)
* local_map-1-1067-11046.png re-saved by the GIMP on max PNG compression - 2857 KB (took about 22 seconds to write a new png - this does not include the time needed to load it, which was a couple seconds for a PNG or a long time for a BMP - also note that the PNG written by this program was almost twice as large as the same png re-saved by the GIMP. I wouldn't use this program to make pngs to send your friends.)

Those times were from my testing the program on my computer, which has an Athlon 64 X2 4600+ CPU and 1 GB of RAM. DF was not running at the time. The program only used one CPU core. (I could make it use two threads for some of the process, but it's already pretty fast, and that wouldn't speed it up on non-multi-core or non-hyperthreading CPUs)

Filesizes from the stock font with no graphics on that same map:

* Windowed mode BMP: 56,431 KB
* Fullscreen mode BMP: 70,538 KB
* Windowed mode df-map: 74 KB
* Fullscreen mode df-map: 162 KB
* Windowed mode PNG: 1849 KB
* Fullscreen mode PNG: 2056 KB

Due to the way the map image is encoded into the df-map file, the detail of the object tileset and font has very little effect on the final size of the df-map file.

Note: After writing down the above numbers, I did a couple further experiments, one of which resulted in a slight reduction in the df-map file sizes. The other one, which isn't included in the final version of the program, resulted in approximately a 43% reduction in the tile data size BEFORE compression, but INCREASED the size AFTER compression. In short, I had it pack the data as tightly as possible, spanning tile IDs across multiple bytes and using only as many bits per tile ID as was needed (which was 9 for my map), and as a result LZMA was unable to compress it as efficiently. What I have the program actually doing currently is to use either 1, 2, or 4 bytes per tile ID, whichever is needed. If there are 65536 or less unique tile images, it uses 2. This is the normal situation (I had around 400 in my map). If there were somehow more than 65536, it would fall back to using 4 bytes. If you only had 256 or less unique tile images, perhaps if you had just started playing a new fortress, it would use only one byte for each tile.

The improvement which did work was essentially to RLE-encode the tile image data prior to handing it to LZMA (instead of just handing the raw image data to LZMA). That RLE-encoding it prior to running it through LZMA gives any improvement at all, actually, I find interesting.

File sizes of the df-map files after that improvement:

* Windowed mode df-map, standard font: 73.0 KB (1 KB less)
* Fullscreen mode df-map, standard font: 155 KB (7 KB less)
* DR's object tileset and SFEF font: 77.9 KB (3 KB less)

It's not much of an improvement, but I implemented each attempted improvement mostly because I was just curious whether the changes would result in a larger or smaller size - That is, how well LZMA would handle the changes.

[1]: http://lifehacker.com/358208/seamlessly-run-linux-apps-on-your-windows-desktop
[2]: http://www.go-mono.com/mono-downloads/download.html
[3]: http://shadowlord13.googlepages.com/ziptestresults.txt
[4]: http://shadowlord13.googlepages.com/rleresults.txt
[5]: http://www.7-zip.org/sdk.html
[6]: http://www.icsharpcode.net/OpenSource/SharpZipLib/
  </toadyone></http:>

## License

Copyright (c) 2009, shadowlord13@gmail.com
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation
and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors
may be used to endorse or promote products derived from this software without
specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
