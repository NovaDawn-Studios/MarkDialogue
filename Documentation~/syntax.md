# MarkDialogue Syntax

MarkDialogue uses the Obsidian.md subset of MarkDown which is almost entirely compatible to MarkDown itself, with an extra subset of context to specifically handle dialogue and logic. Be sure to name your files so that they end in .dlg.md (In Obsidian, make sure any dialogue note names end in `.dlg`, such as `Buy an Apple.dlg` and it'll work) for your file to be picked up by MarkDialogue, then use the following syntax:

## Ignored Content
Empty lines or lines consisting of whitespace are ignored, with the exclusion of counting line numbers for reporting errors.

[Obsidian-style comments](https://help.obsidian.md/Editing+and+formatting/Basic+formatting+syntax#Comments) are also completely ignored outside of counting line numbers. There is one exception to this:
  * Comments as line IDs. These are short strings such at `%% 3TSGXZOL2C %%` will be automatically inserted onto dialogue lines when a script is imported, and is used to allow extra features such as attaching voice dialogue and other external features to a particular line. These comments appear at the end of a line and for all intents and purposes should be ignored, although if the line moves posistion, this comment should go with it. Changing the contents of this comment could break links between assets and their respective line.

MarkDown script tags are also ignored in the event that you want to force your MarkDown editor to display raw MarkDown, even in preview mode. As a result, you could do the below without affecting how your script works (This is done in the [Examples](https://github.com/NovaDawn-Studios/MarkDialogue/tree/main/Examples) directory to force GitHub to display raw MarkDown):  
````md
```md
JOHN
Hi there!
```
or
```markdown
PETE
Hello!
```
````

## Characters

A word in ALL CAPS at the start of the line indicate the character that's speaking. This must be a single word of Unicode uppercase characters (Matching the regular expression class `\p{Lu}`) consisting of no whitespace, although underscores and hyphens are allowed. This tag represents a single unique character in your scene, and can be used by your game to focus the camera on a particular entity in the world, show a sprite, whatever. This isn't necessarily the characters name as you'd want to display it, just a unique identifier.

Following the character identifier with `as John Smith` will tell your game that the character's name should be displayed as 'John Smith' rather than whatever their default is. This can be used to, say, display a character as '???' before they are introduced.

Follow this, and after a hyphen, any text afterwards will be considered 'modifiers', such as changing the sprite or selecting an animation to play. What these modifiers do is up to your discretion.

The entire raw unparsed line will also be returned in case you want to handle it in your own way. It will still be detected as a 'character' though, and this character data will persist for all following lines until the character changes.

If your have a dialogue line where the first word is in all caps and contains no punctuation, prefix it with a `.`` to have it not be detected as a character. For example:
```md
JOHN
.WHAT

JOHN
.WHAT are you doing?
```
will say that the character `JOHN` said `WHAT` and then `WHAT are you doing?`. If your dialogue contains punctuation, though, this escape hatch isn't required. For example:
```md
JOHN
WHAT!?
```
will be parsed correctly.

As a full example, `ALEX as ??? - Moody, Anim:Cross_arms` will tell the game to focus on the character marked as `ALEX` but display their name as `???`, as well as pass back two data elements consisting of `Moody` and `Anim:Cross_arms`, presumably for the game to set their sprite, play a face animation animation or something else related to `Moody` and play an arms crossing animation or something similar.

## Regular Dialogue

Any normal sentence after a character line will be considered standard dialogue. This text is trimmed of whitespace, so feel free to indent if it helps with readability (although the raw unparsed text is also provided if that suits you too). It's up to you to parse this into your appropiate UIs rich text format (if any). You could also just - say - stick `<color=#FF00FF>some text</color>` or whatever formatting you use directly inside your MarkDown files and it'll work out of the gate. New lines separate out dialogue into separate blocks. To force a newline in a dialogue, write '\n', for example: `One\nTwo\nThree!` will be provided to the game as a single dialogue element of 
`One`  
`Two`  
`Three`

As mentioned in the [Characters](#characters) section, if your dialogue starts with a word in all uppercase and contains no punctuation, then prefix it with a `.` to not have the line be parsed as a character.

## Linking to Other Scripts

* [Obsidian-style internal links](https://help.obsidian.md/Linking+notes+and+files/Internal+links) (Or [WikiLinks](https://en.wikipedia.org/wiki/Help:Link)) are used to connect dialogue scripts together and provide branching and choice list options. In this case, internal script links use the WikiLink format, for example, `[[Another Script Name.dlg]]`. Don't forget the `.dlg` suffix! If you use Obsidian, this also proves a trivial way of navigating between scripts, as well as allowing the graph view to show how scripts are connected. By default, these links will connect to scripts of that name in the same directory as the current script, so include relative paths if you need to go to a different directory. These links should exist on their own line - links in dialogue won't be detected. If you use the pipe character to provide an alternate display version, this can be used as the displayed text to the user - for example `[[QuestionAboutMoney.dlg|What about the money?]]`

    * Having no links in your script will mean when the script ends, the dialogue interaction is considered concluded. At this point, your game should return to normal gamplay. Scripts also terminate if they reach a [MarkDown Heading](https://help.obsidian.md/Editing+and+formatting/Basic+formatting+syntax#Headings) with no previously defined link to jump to.
    * A single link in your script will count as a continuation. You can use this to break long scripts into multiple shorter segments. With the use of branching logic below, you can also use this to switch which dialogue you progress to upon conclusion - if something has happpened, go to this script, otherwise go to this one, for example.
    * Multiple links will result in a choice. Use this to display a dialogue choice menu or similar. In truth, MarkDialogue simply returns all links in one big collection, so you could just as easily take the first choice or pick a random one in the game suits it.
    * Internal links are also supported. Say you have a short dialogue with some branches, but don't want to create new files for each branch. Simply create a [MarkDown Heading](https://help.obsidian.md/Editing+and+formatting/Basic+formatting+syntax#Headings) at the start of the branch - say, `# Asked For Apple`, then link to it by beginning the link with a `#` symbol, such as `[[#Asked For Apple]]`.

## Logic Tags/Tag Commands

[Obsidian-style tags](https://help.obsidian.md/Editing+and+formatting/Tags) - similar to [Headings](https://help.obsidian.md/Editing+and+formatting/Basic+formatting+syntax#Headings) but without the space between the `#` and the text - are used for logic handling in a visually similar way to a C-like preprocessor. Admittedly, this is abuse of the feature, but it provides handy highlighting in Obsidian and allows you to, say, search in the script to find out where you gave a particular item to the player or modified some core state. There are some core tags built into MarkDialogue, but you can easily create your own to integrate with your particular game. Built in tags include:

  * `#if`, `#else`, `#elseif` and `#endif` provide the typical branching logic. Branching logic can be nested, although consider breaking these out into different scripts if able to simplify the script's structure. `#if` and `#elseif` statements evaluate the text afterwards to determine whether to enter the branch or not. There are some built in methods, and again you can also provide your own. As an example, the following script checks if you've seen the dialogue in another script named `Talked about stolen money` and changes what's said as a response:  
  ```md
  #if visited(Talked about stolen money)
    CRIMEBOSS
    So you know about the cash, eh?
  #else
    CRIMEBOSS
    You have nothing on me, kid. Shut ya trap!
  #endif
  ``` 
  * `#start` indicates the start point in the script. It's not necessary, and if not supplied, the top of the file (or heading if specified) is an implicit start. Still, it can be useful to highlight graph nodes in Obsidian when you have a complex web, or as a debugging step to start the dialogue partway down a long script.
  * `#end` indicates the termination of a script, even if the script contains `[[links]]`. You can use this in an `#if` block to determine whether to cancel the dialogue early, or continue onto another script. For scripts that lack any `[[links]]`, it can be considered that an implicit `#end` exists at the end of the script.
  * `#TODO` flags that this particular dialogue file is in progress. This prints a warning to the console, and can optionally be handled however you want in your game. This tag is removed when creating a build. Part of this reason is - again - for Obsidian use, in that it can be used to find incomplete scripts using the search function.
  * `#debug`, `#warn` and `#error` will all log at their respective levels the following text to the Unity console. Script name and line number will also be reported. For example, `#debug Should we say this here?`
  * `#throw`, aside from acting like the logging statements, will terminate the script and throw the provided message as an exception, potentially breaking your game. Can be useful to detect and flag impossible scenarios or just things that shouldn't happen. Your game will be able to catch thrown exceptions, but please don't use them for logic flow.
  * Aside from these, your game can create your own tags to handle them as needed. You could add, say, `#giveitem` to support `#giveitem apple`, or `#playsound` to play a sound effect.

## Quotes for Extra Detail

MarkDown quotes are used to provide in-engine comments. Unlike Obsidian `%%comments%%`, these will be sent to your game and can be handled in some way. You could use them to output system or thought style messages, or - as is used by NovaDawn Studios - display in-game debug text to indicate stage directions. There's no hard rule for these, but they will be considered separate from characters and speech. For example:

```md
> John walks up to Pete
JOHN
How are you today?
PETE
Good, thanks!
```

## Full Example

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