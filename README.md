## ResxMergeTool
Originally forked from https://www.codeproject.com/Articles/1145375/ResX-Merge-Tool as of version published on 24 Oct 2016, then modified and extended with a better GUI a.o.t.

### What does it do?
It compares typical merge conflicted .NET Resource (.resx) files through BASE, LOCAL and REMOTE. Conflicts arise when either:

- a key was deleted in one copy and modified in the other copy,
 - the same key was added by both copies with different values,
 - the same key was modified by both copies.

In case of conflict, both versions are shown in the grid, and it is your responsibility to resolve the conflict by deleting the version you don't want to keep.

### Integration with Git
1. Build the solution with the Release configuration,
2. Copy ResXMergeTool.exe in a location where git can find it, typ. in `C:\Program Files\Git\usr\bin`,
3. In your .gitconfig, define a new merge tool by adding the following lines:
   ```
   [merge "ResXMergeTool"]
       name = ResX Merge Tool
       driver = ResXMergeTool.exe %O %A %B
       recursive = binary
   ```
4. In the repository where you want to use this tool, set it as the merge tool for the ResX files by creating or modifying the file `.git/info/attributes`:
   ```
   *.resx merge=ResXMergeTool
   ```
5. That's it, as soon as git attempt to merge two ResX files in this repository, ResXMergeTool.exe will be invoked, and the UI will be displayed when a conflict cannot be automatically resolved.

### Why?
Again: Have a look at the original codeproject article, the author wrote it very well.
Basically, Visual Studio or other external tools messes up Resource files sometimes, which makes a normal diff unpossible and often results in a unresolvable merge conflict.

### To Do
 - Clean the UI by removing unnecessary buttons and UI elements,
 - Show only the conflicts in the grid,
 - Prevent closing the windows as long as some conflicts remain,
 - Display some warnings, such as when duplicate keys are found in the copies,
 - Find a better way to resolve the conflicts than suppressing lines,
 - Completely avoid poping a window for a few milliseconds when no conflicts are detected.
