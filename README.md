# LargeLibVersionControl

LLVC is a version control software for large libraries (> multiple GigaBytes), written in C#.

LLVC is very minimalistic in the sense that it only tracks when a file has been changed.
It does not track how the file has been changed i.e. it does not track file deltas and cannot
recover a previous state of the library.

LLVC is intended for large libraries with multiple GigaBytes on data (like usic libraries for example)
where classical version control systems like Git are unsuitable.

To detect file changes, LLVC uses the LastWritten-date of files.
If this value changes, LLVC computes the SHA256-value of
the corresponding file, to check if the file has been altered since the last time LLVC looked at that file.

## Usage

To start, unpack the release and start LLVC.exe from the command line.

When LLVC is started, you can enter the following commands.

```
Help
Displays this message.

Select [path]
Select will open the library at the given path, if possible.

Init [path]
Init will create a new library at the given path.

Get [full?]
Get will make a quick check if there have been any file changes.
Get full will make a full check if there have been any changes by computing SHA256 values of all files in the current library.
Commit
After calling Get, you can enter Commit to commit the detected file changes.

CopyTo [path]
CopyTo will copy the whole content of the current library to the given path.
Existing files will be overwritten in the process!

CompareTo [path]
CompareTo will compare the state of the current library with the state of a library at the given path.
Sync
After calling CompareTo, if one of the given libraries is ahead by a number of commits, you can enter Sync to update the library that is behind.
```

