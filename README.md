GitViz
======

### Type commands. See their effect in real time. Perfect rendering for presentations.

Among all things we do at [Readify](http://readify.net) to help people build better software, we teach a lot of teams about Git.

For newbies, the concept of commit graphs, references, branches and merges can be a bit hard to visualize.

Instead of trying to do all this on slides, or in sync on a whiteboard, we wanted to be able to run real commands and see a presentation quality diagram in real-time. This tool does that.

We also intelligently render unreachable commits to help people understand hard resets and rebases.

![Screenshot animation](https://raw.github.com/Readify/GitViz/master/SuperHighTechAssets/AnimatedGifTour.gif)

### Where do I get it?

Pre-built binaries are available at https://github.com/Readify/GitViz/releases

### Release Quality

__Alpha.__ This entire project so far consists of one Readify guy sitting down the back of a training course and tapping away for a few hours, no more.

### What is 'presentation quality'?

Large. Clear. Projector-optimized and tested colors. Just enough information to be useful.

### What this tool does NOT do

This is not a day-to-day visualizing tool for big, active repositories. It's optimised for repositories with less than 20 commits, for very specific training scenarios in live presentations.

### How it works

Shells out to `git.exe`, and then renders it with the excellent [GraphSharp](http://graphsharp.codeplex.com).

### FAQ
#### How can I remove dangling commits?
GitViz shows dangling commits to make it easier to visualise rebases and other history rewrites. To remove those commits to clean things up run (this will delete *all* unreachable objects in the repo, be careful):

```
git reflog expire --expire-unreachable=now --all
git gc --prune=now
```
