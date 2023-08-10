<h1 align="center">MarkDialogue</h1>
<p align="center">MarkDown to Dialogue system for Unity</p>

<p align="center">
 <a href="https://github.com/NovaDawn-Studios/MarkDialogue/stargazers">
  <img src="https://img.shields.io/github/stars/NovaDawn-Studios/MarkDialogue?color=FFD700" alt="GitHub Repo Stars" />
 </a>
 <a href="https://github.com/NovaDawn-Studios/MarkDialogue/graphs/contributors">
  <img src="https://img.shields.io/github/contributors-anon/NovaDawn-Studios/MarkDialogue?color=009a00" alt="GitHub Contributors" />
 </a>
 <a href="https://github.com/NovaDawn-Studios/MarkDialogue/commits/main">
  <img src="https://img.shields.io/github/last-commit/NovaDawn-Studios/MarkDialogue" alt="Latest Commit" />
 </a>
</p>

----

**_Note: This product isn't ready or usable yet, but the documentation below corresponds with the overall plans of the project. Once it's in a workable state, this message will disappear._**

----

A [MarkDown](https://daringfireball.net/projects/markdown/)-to-dialogue parser for Unity. Write complex narritives in a common, easy to read format using your favourite tools and play them out in-engine!

This tool was designed for use in games developed by NovaDawn Studios, such as [Exim](https://exim.novadawnstudios.co.uk/). One of the considerations is that it's to be used in parallel with [Obsidian.md](https://obsidian.md/) combined with a Git plugin (Although feel free to use your own Markdown tool of choice). This way, any writer on the project can access a reasonable script writing format and not have to worry too hard about the intricacies of code, alongside utilizing fantastic Obsidian features such as the graph view and canvases and whatever plugins they desire to easily visualize branching narratives and aid in the script writing process. 

As the writers check in their changes, the Unity project can then pull in these changes as they're made, reimport the files using MarkDialogue and they're good to test from the get go. No need to fiddle about with in-Unity tools, node graphs or what have you, just simple text!

MarkDialogue was deveoped in Unity 2022.3 LTS and Unity 2023.1, but the underlying code doesn't rely too much on Unity APIs and it should be trivially compatible with older versions.

## MarkDialogue Syntax

**_[Explanations and details of the full syntax can be seen here](Documentation~/syntax.md)_**


MarkDialogue uses the Obsidian.md subset of MarkDown which is almost entirely compatible to MarkDown itself, with an extra subset of context to specifically handle dialogue and logic. Be sure to name your files so that they end in .dlg.md (In Obsidian, make sure any dialogue note names end in `.dlg`, such as `Buy an Apple.dlg` and it'll work) for your file to be picked up by MarkDialogue. 

Below is an example of a script featuring the core parts of MarkDialogue's syntax.

_Note: More examples are available in the [Samples](https://github.com/NovaDawn-Studios/MarkDialogue/tree/main/Samples~) folder_

```md
> John walks up to Pete

JOHN
How are you today?

PETE
Good, thanks! How about you?

#if globalFlagSet("JohnWasShot") 
  %% Developer Note: globalFlagSet is a potential method implemented by the game, not MarkDialogue. %%

  JOHN (Pained)
  I'm doing alright, my knee is healing, although walking is still a pain.
#else
  JOHN
  Doin' fine, glad I dodged that shot!
#endif

PETE (Happy)
Glad to hear. Hey, have you heard about Don Lenzo?

> At that moment, the Don walks in

CRIMEBOSS
You ain't talkin' 'bout me, are ya lads?

PETE (Worried)
Oh, no sir!
Well...

#if visited(Talked about stolen money)
  CRIMEBOSS (Dangerous)
  So you know about the cash, eh?
#else
  CRIMEBOSS
  You have nothing on me, kid. Shut ya trap!
#endif
```

The empty lines between character dialogue are purely preference. You could just as easily do

```md
JOHN
How are you today?
PETE
Good, thanks! How about you?
```

or even

```md
JOHN

How are you today?

PETE

Good, thanks! How about you?
```

if that suits your style.

## Installing MarkDialogue

To install MarkDialogue in your project, you will need to use the Unity Package Manager. At some point in the future, a dedicated Unity Store package may be available for a small fee to simplify the process (And help fund us in the process).

To open the Unity Package Manager, in Unity, first go to Window -> Package Manager, then click the small + icon at the top left of the window that pops up and click 'Install package from git URL...'. In the window that pops up, paste in https://github.com/NovaDawn-Studios/MarkDialogue.git and you should be good to go.

## Using MarkDialogue

When installed, MarkDialogue will parse all MarkDown files that end in `.dlg.md` and convert them into the appropiate dialogue assets. This is a restriction due to Unity already having an importer for `.md` files that can't be overridden. In Obsidian, make sure to name all your notes with `.dlg` (For example, `Buy an Apple.dlg`) and they'll be picked up by MarkDialogue.

Next, you want to apply a MarkDown Player component onto an entity in your scene, add the relevant Script Collection and set the `Script To Run` if required, and finally hook up the associated events. Either that, or inherit from `BaseMarkdownPlayer` and implement your own handling. Then either mark `Auto Start`, or trigger it yourself from your own code, and you'll be good to go!

**_TODO: Provide more details._**

## Contributing

Are we missing a feature? Find a bug? Just want to contribute? Go ahead! Raise issues, create PRs, spread the word. If your code gets added and is used in a NovaDawn Studios game, you'll even appear in the credits (Unless you don't want to be, of course)!
