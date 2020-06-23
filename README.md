GitViz
======

> GitViz is now available as a .NET Core 3.1 application

### Type commands. See their effect in real time. Perfect rendering for presentations.

Among all things we do at Readify (now [Telstra Purple](https://purple.telstra.com)) to help people build better software, we teach people about Git.

For newbies, the concept of commit graphs, references, branches and merges can be a bit hard to visualize.

Instead of trying to do all this on slides, or in sync on a whiteboard, we wanted to be able to run real commands and see a presentation quality diagram in real-time. This tool does that.

We also intelligently render unreachable commits to help people understand hard resets and rebases.

![Screenshot animation](https://raw.github.com/Readify/GitViz/master/SuperHighTechAssets/AnimatedGifTour.gif)

### Where do I get it?

Pre-built binaries are available at https://github.com/Readify/GitViz/releases

### 

### Release Quality

__Beta.__ This was built by one guy sitting at the back of a training course and tapping away for a few hours, no more. Since then it's been well used and hasn't seen many issues, so we're calling it 'beta'. Will that ever change? ¯\_(ツ)_/¯

### What is 'presentation quality'?

Large. Clear. Projector-optimized and tested colors. Just enough information to be useful.

### What this tool does NOT do

This is not a day-to-day visualizing tool for big, active repositories. It's optimised for repositories with less than 20 commits, for very specific training scenarios in live presentations.

### How it works

Shells out to `git.exe`, and then renders it with the excellent [GraphShape](https://github.com/KeRNeLith/GraphShape).

### FAQ
#### How can I remove dangling commits?
GitViz shows dangling commits to make it easier to visualise rebases and other history rewrites. To remove those commits to clean things up run (this will delete *all* unreachable objects in the repo, be careful):

```
git reflog expire --expire-unreachable=now --all
git gc --prune=now
```
